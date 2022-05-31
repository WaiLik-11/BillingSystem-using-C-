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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        loginBLL l = new loginBLL();
        loginDAL dal = new loginDAL();
        public static string loggedIn;

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pboxClose_Click(object sender, EventArgs e)
        {
            // close this form 
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            l.username = txtUsername.Text.Trim();
            l.password = txtPassword.Text.Trim();
            l.user_type = cmbUserType.Text.Trim();

            //check the login credentials 
            bool success = dal.loginCheck(l);
            if(success==true)
            {
                //login success
                MessageBox.Show("Login Successful.");
                loggedIn = l.username;
                //redirect to open respective form based on user 
                switch(l.user_type)
                {
                    case "Admin":
                        {
                            // display admin dashboard
                            frmAdminDashboard admin = new frmAdminDashboard();
                            admin.Show();
                            this.Hide();
                        }
                        break;

                    case "User":
                        {
                            //display user dash board 
                            frmUserDashboard user = new frmUserDashboard();
                            user.Show();
                            this.Hide();
                        }
                        break;

                    default:
                        {
                            //Display error message
                            MessageBox.Show("Invalid User Type.");
                        }
                        break;
                }
            }
            else
            {
                //login fail 
                MessageBox.Show("Login Failed. Please ensure that u have entered the correct username/password.");
            }
        }
    }
}
