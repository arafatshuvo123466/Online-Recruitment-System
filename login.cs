using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Employee_Management_System
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();

            ForcePasswordMask();
        }

        private void ForcePasswordMask()
        {
            // Works even if Multiline = true
            tbPassword.UseSystemPasswordChar = false;
            tbPassword.PasswordChar = '*';         // or '•'
        }

        private void lblHomepage_Click(object sender, EventArgs e)
        {

            welcomePage w = new welcomePage();
            w.Show();
            this.Hide();
        }

        private void lblHomepage_MouseEnter(object sender, EventArgs e)
        {
            lblHomepage.Font = new Font(lblHomepage.Font, FontStyle.Underline); // underline
            this.Cursor = Cursors.Hand; // change cursor like a button
        }

        private void lblHomepage_MouseLeave(object sender, EventArgs e)
        {
            lblHomepage.Font = new Font(lblHomepage.Font, FontStyle.Regular); // remove underline
            this.Cursor = Cursors.Default;
        }

        private void lblSignup_Click(object sender, EventArgs e)
        {
            signUp s = new signUp();
            s.Show();
            this.Hide();
        }

        private void lblSignup_MouseEnter(object sender, EventArgs e)
        {
            lblSignup.Font = new Font(lblSignup.Font, FontStyle.Underline); // underline
            this.Cursor = Cursors.Hand; // change cursor like a button
        }

        private void lblSignup_MouseLeave(object sender, EventArgs e)
        {
            lblSignup.Font = new Font(lblSignup.Font, FontStyle.Regular); // remove underline
            this.Cursor = Cursors.Default;
        }

        private void lblEmailPassword_Click(object sender, EventArgs e)
        {
            recoverPassword r = new recoverPassword();
            r.Show();
            this.Hide();
        }

        private void lblEmailPassword_MouseEnter(object sender, EventArgs e)
        {
            lblEmailPassword.Font = new Font(lblEmailPassword.Font, FontStyle.Underline); // underline
            this.Cursor = Cursors.Hand; // change cursor like a button
        }

        private void lblEmailPassword_MouseLeave(object sender, EventArgs e)
        {
            lblEmailPassword.Font = new Font(lblEmailPassword.Font, FontStyle.Regular); // remove underline
            this.Cursor = Cursors.Default;
        }





        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();
            string password = tbPassword.Text.Trim();

            // Step 1: Validation
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Login failed : Both fields cannot be empty.",
                                "Login error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your registered email address.",
                                "Login error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password.",
                                "Login error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Step 2: Query database
            try
            {
                dataAccess da = new dataAccess();
                string sql = $"SELECT userID, Role FROM UsersTable WHERE Email='{email}' AND Password='{password}'";

                DataTable dt = da.ExecuteQueryTable(sql);

                if (dt.Rows.Count > 0)  // ✅ Match found
                {
                    string userId = dt.Rows[0]["userID"].ToString();
                    string role = dt.Rows[0]["Role"].ToString();

                    // Step 3: Block pending accounts
                    if (role.Equals("pending", StringComparison.OrdinalIgnoreCase) || userId.StartsWith("P-"))
                    {
                        MessageBox.Show("Your account has not yet been approved. Please wait for admin verification before logging in.",
                                        "Account Pending",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    // ✅ NEW: Record login session
                    string sessionId = SessionTracker.RecordLogin(userId);

                    // Step 4: Redirect based on Role
                    if (role == "Employer")
                    {
                        employerDashboard emp = new employerDashboard(userId);
                        emp.Tag = sessionId; // Pass session ID to dashboard
                        emp.Show();
                        this.Hide();
                    }
                    else if (role == "Admin")
                    {
                        adminDashboard ad = new adminDashboard();
                        ad.Tag = userId + "|" + sessionId; // Pass both user ID and session ID
                        ad.Show();
                        this.Hide();
                    }
                    else if (role == "Job seeker")
                    {
                        jsDashboard js = new jsDashboard();
                        js.Tag = sessionId; // Pass only session ID
                        js.Show();
                        this.Hide();
                    }

                    MessageBox.Show("Login successful! Role : " + role,
                                   "Success",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Invalid email or password.",
                                    "Login Failed",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

    }
}
