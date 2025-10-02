using DocumentFormat.OpenXml.Wordprocessing;
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
    public partial class jsApplicationForm : Form
    {
        // Public properties to receive data from parent form
        public string UserId { get; set; }
        public string JobId { get; set; }
        private dataAccess da;
        public jsApplicationForm()
        {
            InitializeComponent();
            da = new dataAccess();
        }

        private void jsApplicationForm_Load(object sender, EventArgs e)
        {
            LoadJobSeekerProfileData();
        }

        private void LoadJobSeekerProfileData()
        {
            try
            {
                string sql = $@"
SELECT education, skills, experiencelevel, languages, website
FROM JobSeekersTable 
WHERE jsid = '{UserId}'";

                DataTable dt = da.ExecuteQueryTable(sql);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Pre-fill form with existing data
                    if (row["education"] != DBNull.Value)
                        cbEducation.Text = row["education"].ToString();

                    if (row["skills"] != DBNull.Value)
                        tbSkills.Text = row["skills"].ToString();

                    if (row["experiencelevel"] != DBNull.Value)
                        cbExperienceLevel.Text = row["experiencelevel"].ToString();

                    if (row["languages"] != DBNull.Value)
                        tbLanguages.Text = row["languages"].ToString();

                    if (row["website"] != DBNull.Value)
                        tbPersonalWebsite.Text = row["website"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile data: " + ex.Message);
            }
        }

        private void btnSubmitForm_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(cbEducation.Text) ||
                    string.IsNullOrWhiteSpace(tbSkills.Text) ||
                    string.IsNullOrWhiteSpace(cbExperienceLevel.Text) ||
                    string.IsNullOrWhiteSpace(tbExpectedSalary.Text) ||
                    string.IsNullOrWhiteSpace(tbLanguages.Text))
                {
                    MessageBox.Show("Please fill all mandatory fields (Education, Skills, Experience Level, Expected Salary, Languages)",
                                    "Validation Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Validate expected salary is a number (e.g., "25000")
                if (!IsValidSalaryNumber(tbExpectedSalary.Text.Trim()))
                {
                    MessageBox.Show("Please enter expected salary as a valid number\nExample: 25000",
                                    "Invalid Salary Format",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Update JobSeekerTable with the 5 attributes (will replace old values)
                string updateJobSeekerSql = $@"
UPDATE JobSeekersTable 
SET education = '{cbEducation.Text.Trim()}',
    skills = '{tbSkills.Text.Trim()}',
    experiencelevel = '{cbExperienceLevel.Text.Trim()}',
    languages = '{tbLanguages.Text.Trim()}',
    personalwebsite = '{tbPersonalWebsite.Text.Trim()}'
WHERE jsid = '{UserId}'";

                int updateResult = da.ExecuteUpdateQuery(updateJobSeekerSql);

                if (updateResult <= 0)
                {
                    MessageBox.Show("Failed to update profile information.");
                    return;
                }

                // Generate application ID in format "AP-234454"
                string applicationId = GenerateApplicationId();

                // Format application date as "07 Dec, 2001"
                string formattedDate = DateTime.Now.ToString("dd MMM, yyyy");

                // Insert into ApplicationsTable with generated applicationId and formatted date
                string insertApplicationSql = $@"
INSERT INTO ApplicationsTable (applicationId, jobid, jsid, expectedSalary, applieddate, status)
VALUES ('{applicationId}', '{JobId}', '{UserId}', '{tbExpectedSalary.Text.Trim()}', '{formattedDate}', 'Submitted')";

                int insertResult = da.ExecuteUpdateQuery(insertApplicationSql);

                if (insertResult > 0)
                {
                    MessageBox.Show("Application submitted successfully!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to submit application.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting application: " + ex.Message);
            }
        }

        private bool IsValidSalaryNumber(string salary)
        {
            // Check if salary is a valid number (e.g., "25000")
            return int.TryParse(salary, out int result) && result > 0;
        }

        private string GenerateApplicationId()
        {
            try
            {
                // Get the next available application number
                string countSql = "SELECT COUNT(*) FROM ApplicationsTable";
                int count = Convert.ToInt32(da.ExecuteScalarQuery(countSql));

                // Format as AP-000001, AP-000002, etc.
                return $"AP-{(count + 1).ToString("D6")}";
            }
            catch
            {
                // Fallback if counting fails
                return $"AP-{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            cbEducation.SelectedIndex = -1;
            tbSkills.Clear();
            cbExperienceLevel.SelectedIndex = -1;
            tbExpectedSalary.Clear();
            tbPersonalWebsite.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
