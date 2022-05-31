using DGVPrinterHelper;
using RPKJAYA.BLL;
using RPKJAYA.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace RPKJAYA.UI
{
    public partial class frmPurchaseAndSales : Form
    {
        public frmPurchaseAndSales()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
        DeaCustDAL dcDAL = new DeaCustDAL();
        productsDAL pDAL = new productsDAL();
        DataTable transactionDT = new DataTable();
        userDAL uDAL = new userDAL();
        transactionDAL tDAL = new transactionDAL();
        transactionDetailDAL tdDAL = new transactionDetailDAL();

        private void frmPurchaseAndSales_Load(object sender, EventArgs e)
        {
            // get the transaction type value from frmUserDashboard 

            string type = frmUserDashboard.transactionType;

            lblTop.Text = type;

            //specify the columns for transactions dt
            transactionDT.Columns.Add("Product Name");
            transactionDT.Columns.Add("Rate");
            transactionDT.Columns.Add("Quantity");
            transactionDT.Columns.Add("Total");

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text;

            if (keyword == "")
            {
                //clear all the textboxes
                txtName.Text = "";
                txtEmail.Text = "";
                txtContact.Text = "";
                txtAddress.Text = "";
                return;
            }

            DeaCustBLL dc = dcDAL.SearchDealerCustomerForTransaction(keyword);

            txtName.Text = dc.name;
            txtEmail.Text = dc.email;
            txtContact.Text = dc.contact;
            txtAddress.Text = dc.address;

        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            //get keyword from search 
            string keyword = txtSearchProduct.Text;

            if (keyword == "")
            {
                //clear all the textboxes
                txtProductName.Text = "";
                txtInventory.Text = "";
                txtRate.Text = "";
                txtQty.Text = "";
                return;
            }

            productsBLL p = pDAL.GetProductsForTransactions(keyword);

            txtProductName.Text = p.name;
            txtInventory.Text = p.qty.ToString();
            txtRate.Text = p.rate.ToString();
            
        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            //get product name, rate, qty 
            string productName = txtProductName.Text;
            decimal Rate = decimal.Parse(txtRate.Text);
            decimal Qty = decimal.Parse(txtQty.Text);

            decimal Total = Rate * Qty;

            //display sub total
            decimal subTotal = decimal.Parse(txtSubTotal.Text);
            subTotal = subTotal + Total;

            //check if product is selected anot
            if(productName == "")
            {
                //display error
                MessageBox.Show("Select Product first. Try again.");
            }
            else
            {
                //add product to datagrid view
                transactionDT.Rows.Add(productName, Rate, Qty, Total);

                //display in datagridview
                dgvAddedProducts.DataSource = transactionDT;

                //display sub total 
                txtSubTotal.Text = subTotal.ToString();

                txtSearchProduct.Text = "";
                txtProductName.Text = "";
                txtInventory.Text = "0.00";
                txtRate.Text = "0.00";
                txtQty.Text = "0.00";
            }
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            //get value for discount 
            string value = txtDiscount.Text;

            if(value == "")
            {
                MessageBox.Show("Please add discount first");
            }
            else
            {
                //get the discount in decimalvalue
                decimal subTotal = decimal.Parse(txtSubTotal.Text);
                decimal discount = decimal.Parse(txtDiscount.Text);

                //calculate the grand total based on discount
                decimal grandTotal = ((100 - discount) / 100) * subTotal;

                //display grand total in text box
                txtGrandTotal.Text = grandTotal.ToString();
            }
        }

        private void txtVat_TextChanged(object sender, EventArgs e)
        {
            //check if grand total has value anot
            string check = txtGrandTotal.Text;

            if(check == "")
            {
                //display message
                MessageBox.Show("Enter the discount first.");
            }
            else
            {
                //calcualte SST
                decimal previousGT = decimal.Parse(txtGrandTotal.Text);
                decimal vat = decimal.Parse(txtVat.Text);
                decimal grandTotalWithVat = ((100 + vat) / 100) * previousGT;

                //display new GT with VAT (SST)
                txtGrandTotal.Text = grandTotalWithVat.ToString();
            }
        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            //get the paid amount and grnad total 
            decimal grandTotal = decimal.Parse(txtGrandTotal.Text);
            decimal paidAmount = decimal.Parse(txtPaidAmount.Text);

            decimal returnAmount = paidAmount - grandTotal;

            txtReturnAmount.Text = returnAmount.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //get the details 
            transactionsBLL transaction = new transactionsBLL();

            transaction.type = lblTop.Text;

            //get id of dealer or customer
            string deaCustName = txtName.Text;

            DeaCustBLL dc = dcDAL.GetDeaCustIDFromName(deaCustName);

            transaction.dea_cust_id = dc.id;
            transaction.grandTotal = Math.Round(decimal.Parse(txtGrandTotal.Text),2);
            transaction.transaction_date = DateTime.Now;
            transaction.tax = decimal.Parse(txtVat.Text);
            transaction.discount = decimal.Parse(txtDiscount.Text);

            //get user name of logged in user 
            string username = frmLogin.loggedIn;

            userBLL u = uDAL.GetIDFromUsername(username);

            transaction.added_by = u.id;
            transaction.transactionDetails = transactionDT;

            //make a boolean variable and set the value to false
            bool success = false;

            using(TransactionScope scope = new TransactionScope())
            {
                int transactionID = -1;
                bool w = tDAL.Insert_Transaction(transaction, out transactionID);

                //use for loop to insert Transactions Detials 
                for(int i=0;i<transactionDT.Rows.Count;i++)
                {
                    //get all details fo products
                    transactionDetailBLL transactionDetail = new transactionDetailBLL();

                    //get product name and convert to id
                    string ProductName = transactionDT.Rows[i][0].ToString();
                    productsBLL p = pDAL.GetProductIDFromName(ProductName);

                    transactionDetail.product_id = p.id;
                    transactionDetail.rate = decimal.Parse(transactionDT.Rows[i][1].ToString());
                    transactionDetail.qty = decimal.Parse(transactionDT.Rows[i][2].ToString());
                    transactionDetail.total = Math.Round(decimal.Parse(transactionDT.Rows[i][3].ToString()), 2);
                    transactionDetail.dea_cust_id = dc.id;
                    transactionDetail.added_date = DateTime.Now;
                    transactionDetail.added_by = u.id;

                    //Increase or decrease product Qty here based on purchase or sales
                    string transactionType = lblTop.Text;

                    //check if it is on purchase or sales 
                    bool x = false;
                    if(transactionType == "Purchase")
                    {
                        //increase the product 
                        x = pDAL.IncreaseProduct(transactionDetail.product_id, transactionDetail.qty);
                    }
                    else if(transactionType == "Sales")
                    {
                        //decrease the Product Qty 
                        x = pDAL.DecreaseProduct(transactionDetail.product_id, transactionDetail.qty);
                    }

                    //insert transaction details inside db 
                    bool y = tdDAL.InsertTransactionDetail(transactionDetail);
                    success = w && x && y;
                }
                
                if (success == true)
                {
                    //transaction completed
                    scope.Complete();

                    //Code to print bill 
                    DGVPrinter printer = new DGVPrinter();

                    printer.Title = "\r\n\r\n RPK JAYA ENTERPRISE \r\n\r\n";
                    printer.SubTitle = "No, 1005, Taman Samudera,Seri Manjung,\r\n 32040 Sitiawan, Perak, Malaysia. \r\n Phone(MAS): +60 16-4147446 \t\t\t\t Phone(SIG): +65-90869648 \r\n\r\n";
                    printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    printer.PageNumbers = true;
                    printer.PageNumberInHeader = false;
                    printer.PorportionalColumns = true;
                    printer.HeaderCellAlignment = StringAlignment.Near;
                    printer.Footer = "Discount: "+txtDiscount.Text + "% \r\n"+ "SST:"+txtVat.Text + "% \r\n" + "Grand Total: RM"+txtGrandTotal.Text + "\r\n\r\n" + "Thank You for doing business with us.";
                    printer.FooterSpacing = 15;
                    printer.PrintDataGridView(dgvAddedProducts);

                    MessageBox.Show("Transaction was successful.");
                    dgvAddedProducts.DataSource = null;
                    dgvAddedProducts.Rows.Clear();

                    txtSearch.Text = "";
                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtContact.Text = "";
                    txtAddress.Text = "";
                    txtSearchProduct.Text = "";
                    txtProductName.Text = "";
                    txtInventory.Text = "0";
                    txtRate.Text = "0";
                    txtQty.Text = "0";
                    txtSubTotal.Text = "0";
                    txtDiscount.Text = "0";
                    txtVat.Text = "0";
                    txtGrandTotal.Text = "0";
                    txtPaidAmount.Text = "0";
                    txtReturnAmount.Text = "0";
                }
                else
                {
                    //transaction fail 
                    MessageBox.Show("Transaction failed. Please Try again.");
                }
            }
        }
    }
}
