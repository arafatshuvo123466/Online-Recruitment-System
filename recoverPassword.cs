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
    public partial class recoverPassword : Form
    {
        public recoverPassword()
        {
            InitializeComponent();
            ForcePasswordMask();
        }

        private void ForcePasswordMask()
        {
            // Works even if Multiline = true
            tbPassword.UseSystemPasswordChar = false;
            tbPassword.PasswordChar = '*';         // or '•'
            tbRetype.UseSystemPasswordChar = false;
            tbRetype.PasswordChar = '*';         // or '•'
        }

        // back button
        private void btnBack_Click(object sender, EventArgs e)
        {
            login loginForm = new login();

            // Show the login form
            loginForm.Show();

            // Hide the current welcome page
            this.Hide();
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();
            string userId = tbUserID.Text.Trim();
            string newPassword = tbPassword.Text.Trim();
            string retypePassword = tbRetype.Text.Trim();

            // Empty field validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(retypePassword))
            {
                MessageBox.Show("All fields are required.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Password match validation
            if (newPassword != retypePassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.",
                                "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                dataAccess da = new dataAccess();

                // Check if user exists
                string checkQuery = $"SELECT * FROM UsersTable WHERE Email='{email}' AND UserID='{userId}'";
                DataTable dt = da.ExecuteQueryTable(checkQuery);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No account found with the provided email or user ID.",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Prevent reusing the old password
                string oldPassword = dt.Rows[0]["Password"].ToString();
                if (newPassword == oldPassword)
                {
                    MessageBox.Show("New password cannot be the same as the old password. Please choose a different password.",
                                    "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbPassword.Clear();
                    tbRetype.Clear();
                    return;
                }

                // Update password
                string updateQuery = $"UPDATE UsersTable SET Password='{newPassword}' " +
                                     $"WHERE Email='{email}' AND UserID='{userId}'";

                int rows = da.ExecuteUpdateQuery(updateQuery);

                if (rows > 0)
                {
                    MessageBox.Show("Your password has been successfully reset. You can now log in with your new password.",
                                    "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to login page
                    login loginForm = new login();
                    loginForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Password reset failed. Please try again.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message,
                                "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
