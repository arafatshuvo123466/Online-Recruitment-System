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
    public partial class signUp : Form
    {
        public signUp()
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

        private void lblsignIn_Click(object sender, EventArgs e)
        {
            login l = new login();
            l.Show();
            this.Hide();
        }

        private void lblsignIn_MouseEnter(object sender, EventArgs e)
        {
            lblsignIn.Font = new Font(lblsignIn.Font, FontStyle.Underline); // underline
            this.Cursor = Cursors.Hand; // change cursor like a button
        }

        private void lblsignIn_MouseLeave(object sender, EventArgs e)
        {
            lblsignIn.Font = new Font(lblsignIn.Font, FontStyle.Regular); // remove underline
            this.Cursor = Cursors.Default;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            string firstName = tbFirstName.Text.Trim();
            string lastName = tbLastName.Text.Trim();
            string email = tbEmail.Text.Trim();
            string phone = tbContactNumber.Text.Trim();
            string password = tbPassword.Text.Trim();
            string retypePassword = tbRetype.Text.Trim();

            // 1. Empty field validation
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(retypePassword))
            {
                MessageBox.Show("Please fill in all fields before signing up.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Password match validation
            if (password != retypePassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Checkbox validation
            if (!cbTerms.Checked)
            {
                MessageBox.Show("You must accept the terms of use and privacy policy to create an account.", "Agreement Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dataAccess da = new dataAccess();

                // Check if email already exists
                string checkQuery = $"SELECT * FROM UsersTable WHERE Email = '{email}'";
                DataTable dt = da.ExecuteQueryTable(checkQuery);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("This email is already registered. Please use another email to create your account.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // =============================
                // Generate new Pending ID (P-XXX)
                // =============================
                string getMaxQuery = "SELECT MAX(CAST(SUBSTRING(userID, CHARINDEX('-', userID) + 1, LEN(userID)) AS INT)) AS MaxNum FROM UsersTable";
                DataTable dtMax = da.ExecuteQueryTable(getMaxQuery);

                int newNum = 1; // default
                if (dtMax.Rows.Count > 0 && dtMax.Rows[0]["MaxNum"] != DBNull.Value)
                {
                    newNum = Convert.ToInt32(dtMax.Rows[0]["MaxNum"]) + 1;
                }

                string newUserId = "P-" + newNum.ToString("D3"); // format like P-110

                // =============================
                // Insert new user
                // =============================
                string insertQuery = $@"
                    INSERT INTO UsersTable (userID, FirstName, LastName, Email, Phone, Password, role)
                    VALUES ('{newUserId}', '{firstName}', '{lastName}', '{email}', '{phone}', '{password}', 'Pending')";

                int result = da.ExecuteUpdateQuery(insertQuery);

                if (result > 0)
                {
                    MessageBox.Show("Signup successful!\nPlease note: your account is pending admin approval and cannot be used to log in until approved.",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to login
                    login l = new login();
                    l.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Account creation failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
