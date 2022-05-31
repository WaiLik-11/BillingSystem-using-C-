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
    public partial class frmCategories : Form
    {
        public frmCategories()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        categoriesBLL c = new categoriesBLL();
        categoriesDAL dal = new categoriesDAL();
        userDAL udal = new userDAL();


        private void btnADD_Click(object sender, EventArgs e)
        {
            // get the values from category form 
            c.title = txtTitle.Text;
            c.description = txtDescription.Text;
            c.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUser);

            //pass logged in user into added_by field
            c.added_by = usr.id;

            //create boolean method to insert data into database
            bool success = dal.Insert(c);

            // if  the categories is inserted successfully, then value of success is true, else false
            if (success == true)
            {
                //new categories inserted successfully 
                MessageBox.Show("New category Inserted Successfully.");
                Clear();
                //refresh datagrid view
                DataTable dt = dal.Select();
                dgvCategories.DataSource = dt;
            }
            else
            {
                //fail to insert 
                MessageBox.Show("Failed to insert new category. Please try again.");
            }
        }

        public void Clear()
        {
            txtCategoryID.Text = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtSearch.Text = "";
        }

        private void frmCategories_Load(object sender, EventArgs e)
        {
            //display added categories 
            DataTable dt = dal.Select();
            dgvCategories.DataSource = dt;

        }

        private void dgvCategories_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //finding which row is clicked
            int RowIndex = e.RowIndex;
            txtCategoryID.Text = dgvCategories.Rows[RowIndex].Cells[0].Value.ToString();
            txtTitle.Text = dgvCategories.Rows[RowIndex].Cells[1].Value.ToString();
            txtDescription.Text = dgvCategories.Rows[RowIndex].Cells[2].Value.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //get values from form
            c.id = int.Parse(txtCategoryID.Text);
            c.title = txtTitle.Text;
            c.description = txtDescription.Text;
            c.added_date = DateTime.Now;

            //get id 
            string loggedUser = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUser);
            //pass logged in user into added_by field
            c.added_by = usr.id;

            //make 1 boolean variable to update
            bool success = dal.Update(c);

            //if category updated, value of success is true
            if(success == true)
            {
                //updated
                MessageBox.Show("Category Updated succesfully.");
                Clear();

                //refresh 
                DataTable dt = dal.Select();
                dgvCategories.DataSource = dt;
            }
            else
            {
                //fail to update
                MessageBox.Show("Failed to update category. Please Try again.");

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //get id of category to delete
            c.id = int.Parse(txtCategoryID.Text);

            //create boolean variable to delete category
            bool success = dal.Delete(c);

            //if deleted then success is true

            if(success == true)
            {
                //deleted
                MessageBox.Show("Category deleted successfully.");
                Clear();

                //refreshing new data grid view
                DataTable dt = dal.Select();
                dgvCategories.DataSource = dt;
            }
            else
            {
                //no deleted
                MessageBox.Show("Failed to delete category. Please try again.");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //get keywords
            string keywords = txtSearch.Text;

            if(keywords != null)
            {
                //use search method to display result
                DataTable dt = dal.Search(keywords);
                dgvCategories.DataSource = dt;
            }
            else
            {
                //display all categories
                DataTable dt = dal.Select();
                dgvCategories.DataSource = dt;
            }
        }
    }
}
