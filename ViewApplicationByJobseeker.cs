using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Employee_Management_System
{
    public partial class ViewApplicationByJobseeker : Form
    {

        private dataAccess da;
        private string applicationId;
        private string jobseekerId;
        private DataRow originalData;
        private bool isReadOnlyMode = false;
    
        public ViewApplicationByJobseeker(string applicationId, string jobseekerId)
        {
            InitializeComponent();
            this.da = new dataAccess();
            this.applicationId = applicationId;
            this.jobseekerId = jobseekerId;
            this.Load += ViewApplicationByJobseeker_Load;
        }

        private void RemoveFocusFromAllControls()
        {   // Use a more aggressive approach to remove focus
            Label dummyLabel = new Label();
            this.Controls.Add(dummyLabel);
            dummyLabel.Focus();
            this.Controls.Remove(dummyLabel);

            // Set TabStop to false for ALL controls that can receive focus
            foreach (Control control in this.Controls)
            {
                control.TabStop = false; // This will disable tab navigation for ALL controls
            }

            // Ensure no control has focus
            this.ActiveControl = null;
        }

        private void ViewApplicationByJobseeker_Load(object sender, EventArgs e)
        {
            LoadApplicationData();
            RemoveFocusFromAllControls();
        }

        private void LoadApplicationData()
        {
            try
            {
                string sql = @"
SELECT 
    a.applicationId, a.expectedSalary, a.status, a.interviewDate, a.interviewTime, a.appliedDate,
    j.jobtitle, e.companyname,
    js.education, js.skills, js.experiencelevel, js.languages, js.personalwebsite,
    u.firstName + ' ' + u.lastName AS candidatename
FROM ApplicationsTable a
INNER JOIN JobsTable j ON a.jobid = j.jobid
INNER JOIN EmployersTable e ON j.employerid = e.employerid
INNER JOIN JobSeekersTable js ON a.jsid = js.jsid
INNER JOIN UsersTable u ON a.jsid = u.userid
WHERE a.applicationId = @applicationId AND a.jsid = @jobseekerId";

                SqlCommand cmd = new SqlCommand(sql, da.Sqlcon);
                cmd.Parameters.AddWithValue("@applicationId", applicationId);
                cmd.Parameters.AddWithValue("@jobseekerId", jobseekerId);

                DataSet ds = da.ExecuteQuery(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    originalData = ds.Tables[0].Rows[0];
                    PopulateFormFields(originalData);

                    // Check if status allows editing
                    string status = originalData["status"].ToString();
                    isReadOnlyMode = (status != "Submitted");

                    SetFieldEditableState(isReadOnlyMode);
                    // Button remains enabled always - removed: btnUpdateApplication.Enabled = !isReadOnlyMode;
                }
                else
                {
                    MessageBox.Show("Application not found.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading application: " + ex.Message);
            }
        }

        private void PopulateFormFields(DataRow row)
        {
            // Readonly fields
            tbJobTitle.Text = row["jobtitle"].ToString();
            tbCompany.Text = row["companyname"].ToString();
            tbCandidate.Text = row["candidatename"].ToString();
            tbApplicationStatus.Text = row["status"].ToString();
            tbAppliedDate.Text = Convert.ToDateTime(row["appliedDate"]).ToString("dd MMM, yyyy");

            // Handle nullable fields
            tbInterviewDate.Text = row["interviewDate"] == DBNull.Value ? "N/A" : Convert.ToDateTime(row["interviewDate"]).ToString("dd MMM, yyyy");
            tbInterviewTime.Text = row["interviewTime"] == DBNull.Value ? "N/A" : row["interviewTime"].ToString();

            // Editable fields
            cbEducation.Text = row["education"].ToString();
            tbSkills.Text = row["skills"].ToString();
            cbExperienceLevel.Text = row["experiencelevel"].ToString();
            tbExpectedSalary.Text = row["expectedSalary"].ToString();
            tbLanguages.Text = row["languages"].ToString();
            tbPersonalWebsite.Text = row["personalwebsite"] == DBNull.Value ? "N/A" : row["personalwebsite"].ToString();
        }

        private void SetFieldEditableState(bool readOnly)
        {
            // Readonly fields (always readonly)
            tbJobTitle.ReadOnly = true;
            tbCompany.ReadOnly = true;
            tbCandidate.ReadOnly = true;
            tbApplicationStatus.ReadOnly = true;
            tbAppliedDate.ReadOnly = true;
            tbInterviewDate.ReadOnly = true;
            tbInterviewTime.ReadOnly = true;

            // Editable fields (set based on mode)
            cbEducation.Enabled = !readOnly;
            tbSkills.ReadOnly = readOnly;
            cbExperienceLevel.Enabled = !readOnly;
            tbExpectedSalary.ReadOnly = readOnly;
            tbLanguages.ReadOnly = readOnly;
            tbPersonalWebsite.ReadOnly = readOnly;

            RemoveFocusFromAllControls();
        }

        private bool HasChangesBeenMade()
        {
            return cbEducation.Text != originalData["education"].ToString() ||
                   tbSkills.Text != originalData["skills"].ToString() ||
                   cbExperienceLevel.Text != originalData["experiencelevel"].ToString() ||
                   tbExpectedSalary.Text != originalData["expectedSalary"].ToString() ||
                   tbLanguages.Text != originalData["languages"].ToString() ||
                   tbPersonalWebsite.Text != (originalData["personalwebsite"] == DBNull.Value ? "N/A" : originalData["personalwebsite"].ToString());
        }

        private void btnUpdateApplication_Click(object sender, EventArgs e)
        {
            // Check if already updated (status is not "Submitted")
            if (isReadOnlyMode)
            {
                MessageBox.Show("This application has already been updated and cannot be modified again. Current status: " + tbApplicationStatus.Text,
                                "Update Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!HasChangesBeenMade())
            {
                MessageBox.Show("No changes detected. Please make changes before updating.",
                                "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Update JobSeekersTable (profile information)
                string updateProfileSql = $@"
UPDATE JobSeekersTable 
SET education = '{cbEducation.Text}',
    skills = '{tbSkills.Text}',
    experiencelevel = '{cbExperienceLevel.Text}',
    languages = '{tbLanguages.Text}',
    personalwebsite = '{tbPersonalWebsite.Text}'
WHERE jsid = '{jobseekerId}'";

                // Update ApplicationsTable (expected salary and status)
                string updateApplicationSql = $@"
UPDATE ApplicationsTable 
SET expectedSalary = {tbExpectedSalary.Text},
    status = 'Updated'
WHERE applicationId = '{applicationId}'";

                int profileResult = da.ExecuteUpdateQuery(updateProfileSql);
                int applicationResult = da.ExecuteUpdateQuery(updateApplicationSql);

                if (profileResult > 0 && applicationResult > 0)
                {
                    MessageBox.Show("Application updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Set form to read-only mode
                    isReadOnlyMode = true;
                    SetFieldEditableState(true);

                    // Close the form after successful update
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to update application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating application: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownloadPdf_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                    saveFileDialog.FileName = $"Application_{applicationId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    // Create PDF document
                    Document document = new Document(PageSize.A4);
                    PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));

                    document.Open();

                    // Add title
                    iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    Paragraph title = new Paragraph("JOB APPLICATION DETAILS", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20f;
                    document.Add(title);

                    // Add application ID
                    iTextSharp.text.Font idFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    Paragraph idParagraph = new Paragraph($"Application ID: {applicationId}", idFont);
                    idParagraph.SpacingAfter = 15f;
                    document.Add(idParagraph);

                    // Add generation date
                    iTextSharp.text.Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    Paragraph date = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateFont);
                    date.Alignment = Element.ALIGN_RIGHT;
                    date.SpacingAfter = 15f;
                    document.Add(date);

                    // Add application details
                    iTextSharp.text.Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    iTextSharp.text.Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                    AddPdfField(document, "Job Title:", tbJobTitle.Text, headerFont, dataFont);
                    AddPdfField(document, "Company:", tbCompany.Text, headerFont, dataFont);
                    AddPdfField(document, "Candidate:", tbCandidate.Text, headerFont, dataFont);
                    AddPdfField(document, "Application Status:", tbApplicationStatus.Text, headerFont, dataFont);
                    AddPdfField(document, "Applied Date:", tbAppliedDate.Text, headerFont, dataFont);
                    AddPdfField(document, "Education:", cbEducation.Text, headerFont, dataFont);
                    AddPdfField(document, "Skills:", tbSkills.Text, headerFont, dataFont);
                    AddPdfField(document, "Experience Level:", cbExperienceLevel.Text, headerFont, dataFont);
                    AddPdfField(document, "Expected Salary:", tbExpectedSalary.Text, headerFont, dataFont);
                    AddPdfField(document, "Languages:", tbLanguages.Text, headerFont, dataFont);
                    AddPdfField(document, "Personal Website:", tbPersonalWebsite.Text, headerFont, dataFont);
                    AddPdfField(document, "Interview Date:", tbInterviewDate.Text, headerFont, dataFont);
                    AddPdfField(document, "Interview Time:", tbInterviewTime.Text, headerFont, dataFont);

                    document.Close();

                    MessageBox.Show($"PDF exported successfully!\nSaved as: {saveFileDialog.FileName}",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddPdfField(Document document, string label, string value, iTextSharp.text.Font headerFont, iTextSharp.text.Font dataFont)
        {
            // Handle empty values by showing "N/A"
            string displayValue = string.IsNullOrWhiteSpace(value) || value == "N/A" ? "N/A" : value;

            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Chunk(label + " ", headerFont));
            paragraph.Add(new Chunk(displayValue, dataFont));
            paragraph.SpacingAfter = 8f;
            document.Add(paragraph);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
