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
using System.Windows.Forms;

namespace RPKJAYA.UI
{
    public partial class frmDeaCust : Form
    {
        public frmDeaCust()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        DeaCustBLL dc = new DeaCustBLL();
        DeaCustDAL dcDal = new DeaCustDAL();

        userDAL uDal = new userDAL();
        
        private void btnADD_Click(object sender, EventArgs e)
        {
            // get the values from category form 
            dc.type = cmbDeaCust.Text;
            dc.name = txtName.Text;
            dc.email = txtEmail.Text;
            dc.contact = txtContact.Text;
            dc.address = txtAddress.Text;
            dc.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = uDal.GetIDFromUsername(loggedUser);

            //pass logged in user into added_by field
            dc.added_by = usr.id;

            //create boolean method to insert data into database
            bool success = dcDal.Insert(dc);

            // if  the categories is inserted successfully, then value of success is true, else false
            if (success == true)
            {
                //new categories inserted successfully 
                MessageBox.Show("Dealer or Customer added Successfully.");
                Clear();
                //refresh datagrid view
                DataTable dt = dcDal.Select();
                dgvDeaCust.DataSource = dt;
            }
            else
            {
                //fail to insert 
                MessageBox.Show("Failed to add dealer or customer. Please try again.");
            }
        }

        public void Clear()
        {
            txtDeaCustID.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //get values from form
            dc.id = int.Parse(txtDeaCustID.Text);
            dc.type = cmbDeaCust.Text;
            dc.name = txtName.Text;
            dc.email = txtEmail.Text;
            dc.contact = txtContact.Text;
            dc.address = txtAddress.Text;
            dc.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = uDal.GetIDFromUsername(loggedUser);
            //pass logged in user into added_by field
            dc.added_by = usr.id;

            //make 1 boolean variable to update
            bool success = dcDal.Update(dc);

            //if category updated, value of success is true
            if (success == true)
            {
                //updated
                MessageBox.Show("Dealer or Customer Updated succesfully.");
                Clear();

                //refresh 
                DataTable dt = dcDal.Select();
                dgvDeaCust.DataSource = dt;
            }
            else
            {
                //fail to update
                MessageBox.Show("Failed to update Dealer or Customer. Please Try again.");

            }
        }

        private void frmDeaCust_Load(object sender, EventArgs e)
        {
            //Refresh Data Grid View
            DataTable dt = dcDal.Select();
            dgvDeaCust.DataSource = dt;
        }

        private void dgvDeaCust_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int RowIndex = e.RowIndex;
            txtDeaCustID.Text = dgvDeaCust.Rows[RowIndex].Cells[0].Value.ToString();
            cmbDeaCust.Text = dgvDeaCust.Rows[RowIndex].Cells[1].Value.ToString();
            txtName.Text = dgvDeaCust.Rows[RowIndex].Cells[2].Value.ToString();
            txtEmail.Text = dgvDeaCust.Rows[RowIndex].Cells[3].Value.ToString();
            txtContact.Text = dgvDeaCust.Rows[RowIndex].Cells[4].Value.ToString();
            txtAddress.Text = dgvDeaCust.Rows[RowIndex].Cells[5].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //get id of category to delete
            dc.id = int.Parse(txtDeaCustID.Text);

            //create boolean variable to delete category
            bool success = dcDal.Delete(dc);

            //if deleted then success is true

            if (success == true)
            {
                //deleted
                MessageBox.Show("Dealer or Customer has been deleted successfully.");
                Clear();

                //refreshing new data grid view
                DataTable dt = dcDal.Select();
                dgvDeaCust.DataSource = dt;
            }
            else
            {
                //no deleted
                MessageBox.Show("Failed to delete Dealer or Customer. Please try again.");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //get keywords
            string keywords = txtSearch.Text;

            if (keywords != null)
            {
                //use search method to display result
                DataTable dt = dcDal.Search(keywords);
                dgvDeaCust.DataSource = dt;
            }
            else
            {
                //display all categories
                DataTable dt = dcDal.Select();
                dgvDeaCust.DataSource = dt;
            }
        }
    }
}
