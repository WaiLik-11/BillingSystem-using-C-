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
    public partial class frmProducts : Form
    {
        public frmProducts()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        categoriesDAL cdal = new categoriesDAL();
        productsBLL p = new productsBLL();
        productsDAL pdal = new productsDAL();
        userDAL udal = new userDAL();

        private void frmProducts_Load(object sender, EventArgs e)
        {
            //create data table to hold the categories from db
            DataTable categoriesDT = cdal.Select();

            cmbCategory.DataSource = categoriesDT;

            cmbCategory.DisplayMember = "title";
            cmbCategory.ValueMember = "title";

            //Load all the products in datagrid view
            DataTable dt = pdal.Select();

            dgvProducts.DataSource = dt;
        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            // get the values from category form 
            p.name = txtName.Text;
            p.category = cmbCategory.Text;
            p.description = txtDescription.Text;
            p.rate = decimal.Parse(txtRate.Text);
            p.qty = 0;
            p.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUser);

            //pass logged in user into added_by field
            p.added_by = usr.id;

            //create boolean method to insert data into database
            bool success = pdal.Insert(p);

            // if  the categories is inserted successfully, then value of success is true, else false
            if (success == true)
            {
                //new categories inserted successfully 
                MessageBox.Show("Product Inserted Successfully.");
                Clear();
                //refresh datagrid view
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;
            }
            else
            {
                //fail to insert 
                MessageBox.Show("Failed to insert Product. Please try again.");
            }
        }

        public void Clear()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtDescription.Text = "";
            txtRate.Text = "";
            txtSearch.Text = "";
        }

        private void dgvProducts_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int RowIndex = e.RowIndex;
            txtID.Text = dgvProducts.Rows[RowIndex].Cells[0].Value.ToString();
            txtName.Text = dgvProducts.Rows[RowIndex].Cells[1].Value.ToString();
            cmbCategory.Text = dgvProducts.Rows[RowIndex].Cells[2].Value.ToString();
            txtDescription.Text = dgvProducts.Rows[RowIndex].Cells[3].Value.ToString();
            txtRate.Text = dgvProducts.Rows[RowIndex].Cells[4].Value.ToString(); 
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //get values from form
            p.id = int.Parse(txtID.Text);
            p.name = txtName.Text;
            p.category = cmbCategory.Text;
            p.description = txtDescription.Text;
            p.rate = decimal.Parse(txtRate.Text);
            p.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUser);
            //pass logged in user into added_by field
            p.added_by = usr.id;

            //make 1 boolean variable to update
            bool success = pdal.Update(p);

            //if category updated, value of success is true
            if (success == true)
            {
                //updated
                MessageBox.Show("Product Updated succesfully.");
                Clear();

                //refresh 
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;
            }
            else
            {
                //fail to update
                MessageBox.Show("Failed to update product. Please Try again.");

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //get id of category to delete
            p.id = int.Parse(txtID.Text);

            //create boolean variable to delete category
            bool success = pdal.Delete(p);

            //if deleted then success is true

            if (success == true)
            {
                //deleted
                MessageBox.Show("Product deleted successfully.");
                Clear();

                //refreshing new data grid view
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;
            }
            else
            {
                //no deleted
                MessageBox.Show("Failed to delete product. Please try again.");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //get keywords
            string keywords = txtSearch.Text;

            if (keywords != null)
            {
                //use search method to display result
                DataTable dt = pdal.Search(keywords);
                dgvProducts.DataSource = dt;
            }
            else
            {
                //display all categories
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;
            }
        }
    }
}
