using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Employee_Management_System
{
    public partial class welcomePage : Form
    {
        public welcomePage()
        {
            InitializeComponent();
            this.Shown += (s, e) =>
            {
                this.ActiveControl = null;  // Remove focus from all controls
                btnLogin.TabStop = false;    // Prevent tab navigation to buttons
                btnExit.TabStop = false;
            };
            
        }

        // login button
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Create and show login form
            login loginForm = new login();

            // Show the login form
            loginForm.Show();

            // Hide the current welcome page
            this.Hide();
        }

        // exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
