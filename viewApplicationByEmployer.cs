using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Employee_Management_System
{
    public partial class viewApplicationByEmployer : Form
    {
        private string applicationId;
        private string jobseekerName;
        public employerDashboard ParentDashboard { get; set; }
        private dataAccess Da { get; set; }

        public viewApplicationByEmployer(string appId)
        {
            InitializeComponent();
            this.applicationId = appId;
            this.Da = new dataAccess();
            LoadApplicationDetails();
            SetReadOnlyFields();

            RemoveFocusFromAllControls();
        }

        private void RemoveFocusFromAllControls()
        {
            // Remove focus from all controls
            this.ActiveControl = null;

            // Additionally, set TabStop to false for all textboxes to prevent focus
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.TabStop = false;
                }
            }
        }

        private void ViewApplicationByEmployer_Load(object sender, EventArgs e)
        {
            LoadApplicationDetails();
            SetReadOnlyFields();

            // 🔴 ADD THIS LINE to remove focus
            this.ActiveControl = null;
        }

        private void LoadApplicationDetails()
        {
            try
            {
                string sql = @"
SELECT 
    a.applicationId,
    a.status,
    j.jobtitle,
    e.companyName,
    (u.firstName + ' ' + u.lastName) AS candidate,
    a.applieddate,
    js.education,
    js.skills,
    js.experiencelevel,
    a.expectedSalary,
    js.languages,
    js.personalwebsite,
    a.interviewDate,
    a.interviewTime,
    a.jsid
FROM ApplicationsTable a
INNER JOIN JobsTable j ON a.jobid = j.jobid
INNER JOIN EmployersTable e ON j.employerid = e.employerid
INNER JOIN UsersTable u ON a.jsid = u.userID
INNER JOIN JobSeekersTable js ON a.jsid = js.jsid
WHERE a.applicationId = @applicationId";

                SqlCommand cmd = new SqlCommand(sql, this.Da.Sqlcon);
                cmd.Parameters.AddWithValue("@applicationId", applicationId);

                DataSet ds = this.Da.ExecuteQuery(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    // Store jobseeker name for success message
                    jobseekerName = row["candidate"].ToString();

                    // Populate all the textboxes with application data
                    tbJobTitle.Text = row["jobtitle"].ToString();
                    tbCompany.Text = row["companyName"].ToString();
                    tbCandidate.Text = row["candidate"].ToString();
                    tbApplicationStatus.Text = row["status"].ToString();
                    tbAppliedDate.Text = row["applieddate"].ToString();
                    tbEducation.Text = row["education"].ToString();
                    tbSkills.Text = row["skills"].ToString();
                    tbExperienceLevel.Text = row["experiencelevel"].ToString();
                    tbExpectedSalary.Text = row["expectedSalary"].ToString();
                    tbLanguages.Text = row["languages"].ToString();

                    // Handle empty personal website - show "N/A" if null or empty
                    string personalWebsite = row["personalwebsite"]?.ToString() ?? "";
                    tbPersonalWebsite.Text = string.IsNullOrWhiteSpace(personalWebsite) ? "N/A" : personalWebsite;

                    tbInterviewDate.Text = row["interviewDate"]?.ToString() ?? "";
                    tbInterviewTime.Text = row["interviewTime"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading application details: " + ex.Message);
            }
        }



        private void SetReadOnlyFields()
        {
            // Make all textboxes readonly except interview date and time
            tbJobTitle.ReadOnly = true;
            tbCompany.ReadOnly = true;
            tbCandidate.ReadOnly = true;
            tbApplicationStatus.ReadOnly = true;
            tbAppliedDate.ReadOnly = true;
            tbEducation.ReadOnly = true;
            tbSkills.ReadOnly = true;
            tbExperienceLevel.ReadOnly = true;
            tbExpectedSalary.ReadOnly = true;
            tbLanguages.ReadOnly = true;
            tbPersonalWebsite.ReadOnly = true;

            // Interview date and time remain editable
            tbInterviewDate.ReadOnly = false;
            tbInterviewTime.ReadOnly = false;

            RemoveFocusFromAllControls();
        }

        private bool ValidateInterviewDateTime()
        {
            // Both fields can be empty
            if (string.IsNullOrWhiteSpace(tbInterviewDate.Text) && string.IsNullOrWhiteSpace(tbInterviewTime.Text))
                return true;

            bool hasInvalidFormat = false;
            string errorMessage = "Invalid formats:\n";

            // Validate date format: "7 Dec, 2001"
            if (!string.IsNullOrWhiteSpace(tbInterviewDate.Text))
            {
                string datePattern = @"^\d{1,2} [A-Z][a-z]{2}, \d{4}$";
                if (!Regex.IsMatch(tbInterviewDate.Text, datePattern))
                {
                    hasInvalidFormat = true;
                    errorMessage += "- Date must be in format: \"7 Dec, 2001\"\n";
                }
            }

            // Validate time format: "10:00 a.m."
            if (!string.IsNullOrWhiteSpace(tbInterviewTime.Text))
            {
                string timePattern = @"^\d{1,2}:\d{2} (a\.m\.|p\.m\.)$";
                if (!Regex.IsMatch(tbInterviewTime.Text, timePattern))
                {
                    hasInvalidFormat = true;
                    errorMessage += "- Time must be in format: \"10:00 a.m.\"\n";
                }
            }

            if (hasInvalidFormat)
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnUpdateApplication_Click(object sender, EventArgs e)
        {
            if (!ValidateInterviewDateTime())
                return;

            try
            {
                string interviewDate = string.IsNullOrWhiteSpace(tbInterviewDate.Text) ? "NULL" : $"'{tbInterviewDate.Text.Replace("'", "''")}'";
                string interviewTime = string.IsNullOrWhiteSpace(tbInterviewTime.Text) ? "NULL" : $"'{tbInterviewTime.Text.Replace("'", "''")}'";
                string personalWebsite = string.IsNullOrWhiteSpace(tbPersonalWebsite.Text) ? "NULL" : $"'{tbPersonalWebsite.Text.Replace("'", "''")}'";

                // Update interview date/time and change status to "Interview Scheduled"
                string sql = $@"
UPDATE ApplicationsTable 
SET interviewDate = {interviewDate}, 
    interviewTime = {interviewTime},
    status = 'Interview Scheduled'
WHERE applicationId = '{applicationId}'";

                int result = this.Da.ExecuteUpdateQuery(sql);

                if (result > 0)
                {
                    MessageBox.Show($"Interview scheduled for {jobseekerName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the parent dashboard's applications grid
                    if (ParentDashboard != null)
                    {
                        ParentDashboard.LoadApplications();
                    }

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownloadPdf_Click(object sender, EventArgs e)
        {
            // Validate date and time formats before generating PDF
            if (!ValidateInterviewDateTimeForPdf())
                return;

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
                    Paragraph title = new Paragraph("APPLICATION DETAILS", titleFont);
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
                    AddPdfField(document, "Education:", tbEducation.Text, headerFont, dataFont);
                    AddPdfField(document, "Skills:", tbSkills.Text, headerFont, dataFont);
                    AddPdfField(document, "Experience Level:", tbExperienceLevel.Text, headerFont, dataFont);
                    AddPdfField(document, "Expected Salary:", tbExpectedSalary.Text, headerFont, dataFont);
                    AddPdfField(document, "Languages:", tbLanguages.Text, headerFont, dataFont);
                    string personalWebsiteValue = string.IsNullOrWhiteSpace(tbPersonalWebsite.Text) || tbPersonalWebsite.Text == "N/A"
    ? "N/A"
    : tbPersonalWebsite.Text;
                    AddPdfField(document, "Personal Website:", personalWebsiteValue, headerFont, dataFont);
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

        private bool ValidateInterviewDateTimeForPdf()
        {
            // Check if either date or time field has invalid format
            bool hasInvalidFormat = false;
            string errorMessage = "Cannot generate PDF due to invalid formats:\n";

            // Validate date format: "7 Dec, 2001"
            if (!string.IsNullOrWhiteSpace(tbInterviewDate.Text))
            {
                string datePattern = @"^\d{1,2} [A-Z][a-z]{2}, \d{4}$";
                if (!Regex.IsMatch(tbInterviewDate.Text, datePattern))
                {
                    hasInvalidFormat = true;
                    errorMessage += "- Date must be in format: \"7 Dec, 2001\"\n";
                }
            }

            // Validate time format: "10:00 a.m."
            if (!string.IsNullOrWhiteSpace(tbInterviewTime.Text))
            {
                string timePattern = @"^\d{1,2}:\d{2} (a\.m\.|p\.m\.)$";
                if (!Regex.IsMatch(tbInterviewTime.Text, timePattern))
                {
                    hasInvalidFormat = true;
                    errorMessage += "- Time must be in format: \"10:00 a.m.\"\n";
                }
            }

            if (hasInvalidFormat)
            {
                MessageBox.Show(errorMessage + "\nPlease correct the formats before downloading PDF.",
                               "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddPdfField(Document document, string label, string value, iTextSharp.text.Font headerFont, iTextSharp.text.Font dataFont)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Chunk(label + " ", headerFont));

            // Handle "N/A" for empty values in PDF too
            string displayValue = string.IsNullOrWhiteSpace(value) ? "N/A" : value;
            paragraph.Add(new Chunk(displayValue, dataFont));

            paragraph.SpacingAfter = 8f;
            document.Add(paragraph);
        }
    }
}