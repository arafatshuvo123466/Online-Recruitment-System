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
    public partial class viewApplicationByAdmin : Form
    {
        private string applicationId;
        private dataAccess Da { get; set; }

        public viewApplicationByAdmin(string appId)
        {
            InitializeComponent();
            this.applicationId = appId;
            this.Da = new dataAccess();
            LoadApplicationDetails();
            SetReadOnlyFields();
        }

        private void LoadApplicationDetails()
        {
            

            try
            {
                string sql = @"
SELECT 
    a.applicationId,
    a.status,
    (j.jobtitle + ' (' + j.jobid + ')') AS job,
    e.companyName,
    (u.firstName + ' ' + u.lastName + ' (' + a.jsid + ')') AS candidate,
    a.applieddate,
    js.education,
    js.skills,
    js.experiencelevel,
    a.expectedSalary,
    js.languages,
    js.personalwebsite,
    a.interviewDate,
    a.interviewTime
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

                    // Populate all the textboxes with application data
                    tbJobTitle.Text = row["job"].ToString();
                    tbCompany.Text = row["companyName"].ToString();
                    tbCandidate.Text = row["candidate"].ToString();
                    tbApplicationStatus.Text = row["status"].ToString();
                    tbAppliedDate.Text = row["applieddate"].ToString();
                    tbEducation.Text = row["education"].ToString();
                    tbSkills.Text = row["skills"].ToString();
                    tbExperienceLevel.Text = row["experiencelevel"].ToString();
                    tbExpectedSalary.Text = row["expectedSalary"].ToString();
                    tbLanguages.Text = row["languages"].ToString();

                    // Handle empty fields with "N/A"
                    tbPersonalWebsite.Text = string.IsNullOrEmpty(row["personalwebsite"]?.ToString()) ? "N/A" : row["personalwebsite"].ToString();
                    tbInterviewDate.Text = string.IsNullOrEmpty(row["interviewDate"]?.ToString()) ? "N/A" : row["interviewDate"].ToString();
                    tbInterviewTime.Text = string.IsNullOrEmpty(row["interviewTime"]?.ToString()) ? "N/A" : row["interviewTime"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading application details: " + ex.Message);
            }
        }

        private void SetReadOnlyFields()
        {
            // Make all textboxes readonly and remove focus
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.ReadOnly = true;
                    textBox.TabStop = false; // This prevents tab navigation to the textbox
                }
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

                    // Add title - use fully qualified iTextSharp Font
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
            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Chunk(label + " ", headerFont));
            paragraph.Add(new Chunk(value, dataFont));
            paragraph.SpacingAfter = 8f;
            document.Add(paragraph);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteApplication_Click(object sender, EventArgs e)
        {
            try
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete application {applicationId}?\nThis action cannot be undone.",
                                            "Confirm Deletion",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                string deleteSql = $"DELETE FROM ApplicationsTable WHERE applicationId = '{applicationId}'";
                int result = this.Da.ExecuteUpdateQuery(deleteSql);

                if (result > 0)
                {
                    MessageBox.Show("Application deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Close the form after successful deletion
                }
                else
                {
                    MessageBox.Show("Failed to delete application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}