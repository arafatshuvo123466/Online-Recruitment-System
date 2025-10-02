using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Employee_Management_System
{
    public partial class jsDashboard : Form
    {
        private readonly dataAccess da;
        private string _userID;   // Will be set from Tag

        public jsDashboard()
        {
            InitializeComponent();
            da = new dataAccess();

            InitializeFilterComboBox();
            cbFilterEmploymentType.SelectedIndexChanged += cbFilterEmploymentType_SelectedIndexChanged;

            this.Load += jsDashboard_Load;
        }

        private void jsDashboard_Load(object sender, EventArgs e)
        {
            // ✅ Temporary debug
            //CheckTagContent();

            // ✅ Get sessionId from Tag (passed from login form)
            string sessionId = this.Tag?.ToString();

            // ✅ Get userID from somewhere else (you might need to modify this part)
            // Since your current code expects userID in Tag, but now we're putting sessionId there
            // Let's check how you're currently getting _userID

            // If you're currently getting userID from Tag, we need to change the approach
            // Let me see how you're setting _userID currently...

            // For now, let's add a fallback to find userID from session table if needed
            if (string.IsNullOrEmpty(_userID) && !string.IsNullOrEmpty(sessionId))
            {
                _userID = GetUserIdFromSession(sessionId);
            }

            if (string.IsNullOrEmpty(_userID))
            {
                MessageBox.Show("User ID not found. Please login again.");
                this.Close();
                return;
            }

            

            // ✅ Check profile completeness and show appropriate panel
            if (IsProfileComplete(_userID))
            {
                panelDashboardJobseeker2.Visible = true;   // Complete profile panel
                panelDashboardJobseeker1.Visible = false;  // Incomplete profile panel

                // Load jobseeker dashboard statistics
                LoadJobseekerDashboardStats();
            }
            else
            {
                panelDashboardJobseeker1.Visible = true;   // Incomplete profile panel  
                panelDashboardJobseeker2.Visible = false;  // Complete profile panel
            }
        }

        

        // ✅ TEMPORARY: Add this method to check what's in the Tag
        //private void CheckTagContent()
        //{
        //    MessageBox.Show($"Tag content: {this.Tag ?? "NULL"}\nUserID: {_userID ?? "NULL"}",
        //                   "Debug Info",
        //                   MessageBoxButtons.OK,
        //                   MessageBoxIcon.Information);
        //}

        // ✅ NEW: Helper method to get userID from session table if needed
        private string GetUserIdFromSession(string sessionId)
        {
            try
            {
                dataAccess da = new dataAccess();
                string query = $"SELECT userID FROM LogsTable WHERE logid = '{sessionId}'";
                object result = da.ExecuteScalarQuery(query);
                return result?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }




        private void ClearAllDataGridViewFocus()
        {
            // Clear selection and remove focus from all DataGridViews
            if (dgvJobs != null)
            {
                dgvJobs.ClearSelection();
                dgvJobs.CurrentCell = null;
                dgvJobs.TabStop = false;
            }

            if (dgvSavedJobs != null)
            {
                dgvSavedJobs.ClearSelection();
                dgvSavedJobs.CurrentCell = null;
                dgvSavedJobs.TabStop = false;
            }

            if (dgvApplications != null)
            {
                dgvApplications.ClearSelection();
                dgvApplications.CurrentCell = null;
                dgvApplications.TabStop = false;
            }

            // Remove focus from any control
            this.ActiveControl = null;
        }



        // sidebar buttons
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (IsProfileComplete(_userID))
            {
                panelDashboardJobseeker2.Visible = true;
                panelDashboardJobseeker1.Visible = false;
                panelFindJobs.Visible = false;
                panelSavedJobs.Visible = false;
                panelApplications.Visible = false;

                // Refresh jobseeker dashboard statistics when dashboard is clicked
                LoadJobseekerDashboardStats();
            }
            else
            {
                panelDashboardJobseeker1.Visible = true;
                panelDashboardJobseeker2.Visible = false;
                panelFindJobs.Visible = false;
                panelSavedJobs.Visible = false;
                panelApplications.Visible = false;
            }

            ClearAllDataGridViewFocus(); // Add this line
        }

        private void btnSearchJob_Click(object sender, EventArgs e)
        {
            // Check if job seeker has complete basic profile information only
            if (!IsProfileComplete(_userID))
            {
                MessageBox.Show("Please complete your profile information before searching for jobs.\n\n" +
                                "Required fields: Gender, Nationality, Date of Birth, Blood Group, Address, Marital Status, Religion",
                                "Profile Incomplete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Initialize the combobox before showing the panel
            InitializeFilterComboBox();

            // If validation passes, show the jobs panel
            panelFindJobs.Visible = true;
            panelDashboardJobseeker2.Visible = false;
            panelSavedJobs.Visible = false;
            panelApplications.Visible = false;

            // Optional: Load jobs data when panel opens
            LoadAvailableJobs();

            ClearAllDataGridViewFocus();
        }

        private void btnSavedJobs_Click(object sender, EventArgs e)
        {
            if (IsProfileComplete(_userID))
            {
                // show saved jobs panel
                panelSavedJobs.Visible = true;
                panelDashboardJobseeker2.Visible = false;
                panelDashboardJobseeker1.Visible = false;
                panelFindJobs.Visible = false;
                panelApplications.Visible = false;

                // ✅ Load saved jobs into grid AFTER showing panel
                LoadSavedJobs();
            }
            else
            {
                MessageBox.Show("Please complete your profile first to view saved jobs.",
                                "Profile Incomplete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }

            ClearAllDataGridViewFocus(); // Add this line
        }

        private void btnApplicationsJs_Click(object sender, EventArgs e)
        {
            if (!IsProfileComplete(_userID))
            {
                MessageBox.Show("Please complete your profile first to view applications.",
                                "Profile Incomplete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            panelApplications.Visible = true;
            panelDashboardJobseeker2.Visible = false;
            panelDashboardJobseeker1.Visible = false;
            panelFindJobs.Visible = false;
            panelSavedJobs.Visible = false;

            LoadApplicationsForJobseeker(); // Load applications for this jobseeker

            ClearAllDataGridViewFocus(); // Add this line
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // First confirmation: Logout confirmation
            DialogResult logoutResult = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Only proceed if user clicks Yes
            if (logoutResult == DialogResult.Yes)
            {
                // Second confirmation: Review prompt
                DialogResult reviewResult = MessageBox.Show(
                    "Your feedback helps us make JobConnect better. Would you like to leave a quick review before you go?",
                    "Help us improve!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (reviewResult == DialogResult.Yes)
                {
                    // Open review form with user info (Jobseeker)
                    reviews reviewForm = new reviews(_userID, "JobSeeker");
                    reviewForm.Tag = this.Tag; // Pass session info to review form
                    reviewForm.Show();
                    this.Hide();
                }
                else
                {
                    // ✅ Record logout session before going to login page
                    PerformLogout();
                }
            }
            // If No is clicked on logout confirmation, do nothing and stay on dashboard
        }

        // ✅ NEW: Method to handle actual logout with session recording
        private void PerformLogout()
        {
            try
            {
                // Get session ID from Tag
                string sessionId = this.Tag?.ToString();
                string userId = _userID;

                if (string.IsNullOrEmpty(sessionId))
                {
                    MessageBox.Show("Warning: No session ID found. Logout time may not be recorded.",
                                  "Session Warning",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
                else
                {
                    // Record logout time
                    bool logoutRecorded = SessionTracker.RecordLogout(sessionId, userId);

                    if (!logoutRecorded)
                    {
                        MessageBox.Show("Warning: Could not record logout time.",
                                      "Session Warning",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                    }
                }

                // Go to login page
                login l = new login();
                l.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during logout: " + ex.Message,
                              "Logout Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                // Still proceed to login page even if session recording fails
                login l = new login();
                l.Show();
                this.Hide();
            }
        }


        // default panel form
        private void btnSearchForJobs_Click(object sender, EventArgs e)
        {
            // Initialize the combobox before showing the panel
            InitializeFilterComboBox();

            panelFindJobs.Visible = true;
            panelSavedJobs.Visible = false;
            panelDashboardJobseeker2.Visible = false;
            panelDashboardJobseeker1.Visible = false;
            panelApplications.Visible = false;
            LoadAvailableJobs();
            ClearAllDataGridViewFocus();
        }
        private void btnClearJs_Click(object sender, EventArgs e)
        {
            cbGenderJs.SelectedIndex = -1;
            cbNationality.SelectedIndex = -1;
            tbDateOfBirth.Clear();
            cbBloodGroup.SelectedIndex = -1;
            tbAddress.Clear();
            cbMaritalStatus.SelectedIndex = -1;
            cbReligion.SelectedIndex = -1;
        }

        private void btnContinueJs_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ FIRST: Check if all required fields are filled
                if (string.IsNullOrWhiteSpace(cbGenderJs.Text) ||
                    string.IsNullOrWhiteSpace(cbNationality.Text) ||
                    string.IsNullOrWhiteSpace(tbDateOfBirth.Text) ||
                    string.IsNullOrWhiteSpace(cbBloodGroup.Text) ||
                    string.IsNullOrWhiteSpace(tbAddress.Text) ||
                    string.IsNullOrWhiteSpace(cbMaritalStatus.Text) ||
                    string.IsNullOrWhiteSpace(cbReligion.Text))
                {
                    MessageBox.Show("Please fill in all required fields.",
                                    "Validation Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // ✅ SECOND: Validate DOB format
                DateTime dob;
                if (!DateTime.TryParseExact(tbDateOfBirth.Text.Trim(),
                                            "dd MMM, yyyy",
                                            System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None,
                                            out dob))
                {
                    MessageBox.Show("Please enter Date of Birth in the format: dd MMM, yyyy\nExample: 07 Dec, 2001",
                                    "Invalid Date Format",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // ✅ Always update gender in UsersTable
                string updateGenderSql = $@"
UPDATE UsersTable
SET gender = '{cbGenderJs.Text}'
WHERE userID = '{_userID}'";
                da.ExecuteUpdateQuery(updateGenderSql);

                // ✅ Check if JobSeeker record already exists
                string checkSql = $"SELECT COUNT(*) FROM JobSeekersTable WHERE jsid = '{_userID}'";
                int exists = Convert.ToInt32(da.ExecuteScalarQuery(checkSql));

                string sql;
                if (exists > 0)
                {
                    // Update existing JobSeekersTable record
                    sql = $@"
UPDATE JobSeekersTable
SET nationality   = '{cbNationality.Text}',
    dob           = '{dob:dd MMM, yyyy}',
    bloodgroup    = '{cbBloodGroup.Text}',
    jsaddress     = '{tbAddress.Text}',
    maritalstatus = '{cbMaritalStatus.Text}',
    religion      = '{cbReligion.Text}'
WHERE jsid = '{_userID}'";
                }
                else
                {
                    // Insert new JobSeekersTable record
                    sql = $@"
INSERT INTO JobSeekersTable (jsid, nationality, dob, bloodgroup, jsaddress, maritalstatus, religion)
VALUES ('{_userID}', '{cbNationality.Text}', '{dob:dd MMM, yyyy}',
        '{cbBloodGroup.Text}', '{tbAddress.Text}', '{cbMaritalStatus.Text}', '{cbReligion.Text}')";
                }

                int result = da.ExecuteUpdateQuery(sql);

                if (result > 0)
                {
                    MessageBox.Show("Profile saved successfully!");
                    LoadJobseekerDashboardStats();
                    panelDashboardJobseeker2.Visible = true;
                    panelDashboardJobseeker1.Visible = false;
                }
                else
                {
                    MessageBox.Show("Failed to save profile.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving profile: " + ex.Message);
            }
        }


        // applications panel
        private void LoadApplicationsForJobseeker()
        {
            try
            {
                string sql = @"
SELECT 
    j.jobtitle AS JobTitle,
    a.status AS ApplicationStatus,
    j.salaryrange AS SalaryRange,
    a.expectedSalary AS ExpectedSalary,
    a.applieddate AS AppliedDate,
    a.interviewDate AS InterviewDate,
    a.interviewTime AS InterviewTime,
    a.applicationId
FROM ApplicationsTable a
INNER JOIN JobsTable j ON a.jobid = j.jobid
WHERE a.jsid = @jobseekerID
ORDER BY a.applieddate DESC";

                SqlCommand cmd = new SqlCommand(sql, da.Sqlcon);
                cmd.Parameters.AddWithValue("@jobseekerID", _userID);

                DataSet ds = da.ExecuteQuery(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // First, clear all existing columns to avoid conflicts
                    dgvApplications.Columns.Clear();
                    dgvApplications.AutoGenerateColumns = false;

                    // Add columns one by one with exact DataPropertyName matching SQL aliases
                    DataGridViewTextBoxColumn colJobTitle = new DataGridViewTextBoxColumn();
                    colJobTitle.Name = "colJobTitle";
                    colJobTitle.HeaderText = "Job title";
                    colJobTitle.DataPropertyName = "JobTitle";
                    dgvApplications.Columns.Add(colJobTitle);

                    DataGridViewTextBoxColumn colAppStatus = new DataGridViewTextBoxColumn();
                    colAppStatus.Name = "colAppStatus";
                    colAppStatus.HeaderText = "Application status";
                    colAppStatus.DataPropertyName = "ApplicationStatus";
                    dgvApplications.Columns.Add(colAppStatus);

                    DataGridViewTextBoxColumn colSalaryRange = new DataGridViewTextBoxColumn();
                    colSalaryRange.Name = "colSalaryRange";
                    colSalaryRange.HeaderText = "Salary range";
                    colSalaryRange.DataPropertyName = "SalaryRange";
                    dgvApplications.Columns.Add(colSalaryRange);

                    DataGridViewTextBoxColumn colExpectedSalary = new DataGridViewTextBoxColumn();
                    colExpectedSalary.Name = "colExpectedSalary";
                    colExpectedSalary.HeaderText = "Expected salary";
                    colExpectedSalary.DataPropertyName = "ExpectedSalary";
                    dgvApplications.Columns.Add(colExpectedSalary);

                    DataGridViewTextBoxColumn colAppliedDate = new DataGridViewTextBoxColumn();
                    colAppliedDate.Name = "colAppliedDate";
                    colAppliedDate.HeaderText = "Applied date";
                    colAppliedDate.DataPropertyName = "AppliedDate";
                    dgvApplications.Columns.Add(colAppliedDate);

                    DataGridViewTextBoxColumn colInterviewDate = new DataGridViewTextBoxColumn();
                    colInterviewDate.Name = "colInterviewDate";
                    colInterviewDate.HeaderText = "Interview date";
                    colInterviewDate.DataPropertyName = "InterviewDate";
                    dgvApplications.Columns.Add(colInterviewDate);

                    DataGridViewTextBoxColumn colInterviewTime = new DataGridViewTextBoxColumn();
                    colInterviewTime.Name = "colInterviewTime";
                    colInterviewTime.HeaderText = "Interview time";
                    colInterviewTime.DataPropertyName = "InterviewTime";
                    dgvApplications.Columns.Add(colInterviewTime);

                    // Hidden column for application ID (for the view/update functionality)
                    DataGridViewTextBoxColumn colApplicationId = new DataGridViewTextBoxColumn();
                    colApplicationId.Name = "applicationId";
                    colApplicationId.HeaderText = "Application ID";
                    colApplicationId.DataPropertyName = "applicationId";
                    colApplicationId.Visible = false;
                    dgvApplications.Columns.Add(colApplicationId);

                    dgvApplications.DataSource = ds.Tables[0];

                    // Fix column width and text wrapping
                    dgvApplications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dgvApplications.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvApplications.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                    ClearAllDataGridViewFocus();
                }
                else
                {
                    dgvApplications.DataSource = null;
                    MessageBox.Show("No applications found.");
                    ClearAllDataGridViewFocus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading applications: " + ex.Message);
                ClearAllDataGridViewFocus();
            }
        }




        // find jobs panel
        private void InitializeFilterComboBox()
        {
            // Always clear existing items first to avoid duplicates
            cbFilterEmploymentType.Items.Clear();

            // Add the items
            cbFilterEmploymentType.Items.AddRange(new string[] { "All", "Full-time", "Part-time" });
            cbFilterEmploymentType.SelectedItem = "All";
            cbFilterEmploymentType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Force refresh
            cbFilterEmploymentType.Refresh();
        }

        private void btnApply1_Click(object sender, EventArgs e)
        {
            ApplyForJob(dgvJobs);
        }
        private void btnSaveJob_Click(object sender, EventArgs e)
        {
            try
            {
                // Make sure a row is selected
                if (dgvJobs.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a job to save.",
                                    "No Job Selected",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Retrieve jobId from hidden column (must exist in dgvJobs)
                object jobIdObj = dgvJobs.SelectedRows[0].Cells["jobid"].Value;
                if (jobIdObj == null || string.IsNullOrWhiteSpace(jobIdObj.ToString()))
                {
                    MessageBox.Show("Unable to identify the selected job. Please refresh the jobs list.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                string jobId = jobIdObj.ToString();

                // Ensure the current user is logged in (safety check)
                if (string.IsNullOrEmpty(_userID))
                {
                    MessageBox.Show("User not recognized. Please log in again.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // Check if the job is already saved
                string checkSql = $@"
            SELECT COUNT(*) 
            FROM SavedJobsTable
            WHERE jobId = '{jobId}' AND jsid = '{_userID}'";

                int exists = Convert.ToInt32(da.ExecuteScalarQuery(checkSql));

                if (exists > 0)
                {
                    MessageBox.Show("This job is already saved.",
                                    "Duplicate Save",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                // Insert the job into SavedJobsTable (savedDate defaults automatically)
                string insertSql = $@"
            INSERT INTO SavedJobsTable (jobId, jsid)
            VALUES ('{jobId}', '{_userID}')";

                int result = da.ExecuteUpdateQuery(insertSql);

                if (result > 0)
                {
                    MessageBox.Show("Job saved successfully!",
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save job. Please try again.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving job: " + ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }



        private void LoadAvailableJobs()
        {
            // Use the filtered method with no filters to load all jobs
            LoadAvailableJobsWithFilter("", "All");
        }






        // saved jobs panel

        // Method for Apply button in Saved Jobs panel  
        private void btnApply2_Click(object sender, EventArgs e)
        {
            ApplyForJob(dgvJobs);
        }

        private void btnRemoveJob_Click(object sender, EventArgs e)
        {
            try
            {
                // Make sure a row is selected
                if (dgvSavedJobs.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a job to remove.",
                                    "No Selection",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Get jobId from hidden column in dgvSavedJobs
                object jobIdObj = dgvSavedJobs.SelectedRows[0].Cells["jobid"].Value;
                if (jobIdObj == null || string.IsNullOrWhiteSpace(jobIdObj.ToString()))
                {
                    MessageBox.Show("Unable to identify the selected job.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                string jobId = jobIdObj.ToString();

                // Confirm deletion
                DialogResult confirm = MessageBox.Show("Are you sure you want to remove this job from saved jobs?",
                                                       "Confirm Removal",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                // Delete from SavedJobsTable
                string deleteSql = $@"
DELETE FROM SavedJobsTable
WHERE jobId = '{jobId}' AND jsid = '{_userID}'";

                int result = da.ExecuteUpdateQuery(deleteSql);

                if (result > 0)
                {
                    MessageBox.Show("Job removed successfully!",
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    // ✅ Refresh the saved jobs grid
                    LoadSavedJobs();
                }
                else
                {
                    MessageBox.Show("Failed to remove job.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing job: " + ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LoadSavedJobs()
        {
            try
            {
                string sql = $@"
            SELECT 
                j.jobid,
                j.jobtitle AS jobtitle,
                e.companyname AS companyname,
                e.industry AS industry,
                e.address AS address,
                e.website AS website,
                e.about AS about,
                j.employmenttype AS employmenttype,
                j.salaryrange AS salaryrange,
                j.experiencerequired AS experiencerequired,
                j.deadline AS deadline,
                s.savedDate AS savedDate
            FROM SavedJobsTable s
            INNER JOIN JobsTable j ON s.jobId = j.jobid
            INNER JOIN EmployersTable e ON j.employerid = e.employerid
            WHERE s.jsid = '{_userID}'";

                DataTable dt = da.ExecuteQueryTable(sql);

                if (dt == null || dt.Rows.Count == 0)
                {
                    dgvSavedJobs.DataSource = null;
                    MessageBox.Show("You have not saved any jobs yet.");
                    ClearAllDataGridViewFocus();
                    return;
                }

                dgvSavedJobs.AutoGenerateColumns = false;
                dgvSavedJobs.Columns.Clear();

                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "jobtitle",
                    HeaderText = "Job title",
                    DataPropertyName = "jobtitle"
                });

                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "companyname",
                    HeaderText = "Company",
                    DataPropertyName = "companyname"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "industry",
                    HeaderText = "Industry",
                    DataPropertyName = "industry"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "address",
                    HeaderText = "Location",
                    DataPropertyName = "address"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "website",
                    HeaderText = "Website",
                    DataPropertyName = "website"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "about",
                    HeaderText = "About",
                    DataPropertyName = "about"
                });
               
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "employmenttype",
                    HeaderText = "Employment Type",
                    DataPropertyName = "employmenttype"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "salaryrange",
                    HeaderText = "Salary Range",
                    DataPropertyName = "salaryrange"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "experiencerequired",
                    HeaderText = "Experience Required",
                    DataPropertyName = "experiencerequired"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "deadline",
                    HeaderText = "Deadline",
                    DataPropertyName = "deadline"
                });
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "savedDate",
                    HeaderText = "Saved On",
                    DataPropertyName = "savedDate"
                });

                // Hidden jobid column
                dgvSavedJobs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "jobid",
                    HeaderText = "Job ID",
                    DataPropertyName = "jobid",
                    Visible = false
                });

                dgvSavedJobs.DataSource = dt;

                dgvSavedJobs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvSavedJobs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvSavedJobs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                ClearAllDataGridViewFocus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading saved jobs: " + ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                ClearAllDataGridViewFocus();
            }
        }














        private void LoadJobSeekerProfile()
        {
            try
            {
                string sql = $@"
SELECT u.gender, j.nationality, j.dob, j.bloodgroup, 
       j.jsaddress, j.maritalstatus, j.religion
FROM UsersTable u
LEFT JOIN JobSeekersTable j ON u.userID = j.jsid
WHERE u.userID = '{_userID}'";

                DataTable dt = da.ExecuteQueryTable(sql);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    cbGenderJs.Text = row["gender"].ToString();
                    cbNationality.Text = row["nationality"].ToString();
                    tbDateOfBirth.Text = row["dob"].ToString();
                    cbBloodGroup.Text = row["bloodgroup"].ToString();
                    tbAddress.Text = row["jsaddress"].ToString();
                    cbMaritalStatus.Text = row["maritalstatus"].ToString();
                    cbReligion.Text = row["religion"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message);
            }
        }

        private void LoadJobseekerDashboardStats()
        {
            try
            {
                // 1. Total jobs you've applied for (total applications by this jobseeker)
                string sql1 = $"SELECT COUNT(*) AS TotalApplications FROM ApplicationsTable WHERE jsid = '{_userID}'";
                DataSet ds1 = da.ExecuteQuery(sql1);
                tbdashboard1.Text = ds1.Tables[0].Rows[0]["TotalApplications"] != DBNull.Value
                    ? ds1.Tables[0].Rows[0]["TotalApplications"].ToString() : "0";

                // 2. Total applications under review (applications with status "Submitted")
                string sql2 = $"SELECT COUNT(*) AS SubmittedApplications FROM ApplicationsTable WHERE jsid = '{_userID}' AND status = 'Submitted'";
                DataSet ds2 = da.ExecuteQuery(sql2);
                tbdashboard2.Text = ds2.Tables[0].Rows[0]["SubmittedApplications"] != DBNull.Value
                    ? ds2.Tables[0].Rows[0]["SubmittedApplications"].ToString() : "0";

                // 3. Interview scheduled for you (applications with status "Interview Scheduled")
                string sql3 = $"SELECT COUNT(*) AS InterviewScheduled FROM ApplicationsTable WHERE jsid = '{_userID}' AND status = 'Interview Scheduled'";
                DataSet ds3 = da.ExecuteQuery(sql3);
                tbdashboard3.Text = ds3.Tables[0].Rows[0]["InterviewScheduled"] != DBNull.Value
                    ? ds3.Tables[0].Rows[0]["InterviewScheduled"].ToString() : "0";

                // 4. Applications not successful (applications with status "Rejected" and "Rejected after interview")
                string sql4 = $"SELECT COUNT(*) AS RejectedApplications FROM ApplicationsTable WHERE jsid = '{_userID}' AND status IN ('Rejected', 'Rejected after interview')";
                DataSet ds4 = da.ExecuteQuery(sql4);
                tbdashboard4.Text = ds4.Tables[0].Rows[0]["RejectedApplications"] != DBNull.Value
                    ? ds4.Tables[0].Rows[0]["RejectedApplications"].ToString() : "0";

                // 5. Jobs saved for later viewing (total jobs saved by this jobseeker)
                string sql5 = $"SELECT COUNT(*) AS TotalSavedJobs FROM SavedJobsTable WHERE jsid = '{_userID}'";
                DataSet ds5 = da.ExecuteQuery(sql5);
                tbdashboard5.Text = ds5.Tables[0].Rows[0]["TotalSavedJobs"] != DBNull.Value
                    ? ds5.Tables[0].Rows[0]["TotalSavedJobs"].ToString() : "0";

                // 6. Your first application date (earliest application date by this jobseeker)
                string sql6 = $"SELECT MIN(applieddate) AS FirstApplicationDate FROM ApplicationsTable WHERE jsid = '{_userID}'";
                DataSet ds6 = da.ExecuteQuery(sql6);
                tbdashboard6.Text = ds6.Tables[0].Rows[0]["FirstApplicationDate"] != DBNull.Value
                    ? ds6.Tables[0].Rows[0]["FirstApplicationDate"].ToString() : "Not applied yet";

                // 7. Most recent application date (latest application date by this jobseeker)
                string sql7 = $"SELECT MAX(applieddate) AS LastApplicationDate FROM ApplicationsTable WHERE jsid = '{_userID}'";
                DataSet ds7 = da.ExecuteQuery(sql7);
                tbdashboard7.Text = ds7.Tables[0].Rows[0]["LastApplicationDate"] != DBNull.Value
                    ? ds7.Tables[0].Rows[0]["LastApplicationDate"].ToString() : "Not applied yet";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading jobseeker dashboard statistics: " + ex.Message);

                // Set default values in case of error
                for (int i = 1; i <= 7; i++)
                {
                    var textBox = this.Controls.Find($"tbdashboard{i}", true).FirstOrDefault() as TextBox;
                    if (textBox != null)
                        textBox.Text = i <= 5 ? "0" : "Not available";
                }
            }
        }








        // find jobs panel


        // ✅ Profile completeness check (same logic as employer dashboard)
        private bool IsProfileComplete(string userID)
        {
            try
            {
                string sql = $@"
SELECT u.gender,
       j.nationality, j.dob, j.bloodgroup, j.jsaddress, 
       j.maritalstatus, j.religion
FROM UsersTable u
LEFT JOIN JobSeekersTable j ON u.userID = j.jsid
WHERE u.userID = '{userID}'";

                DataTable dt = da.ExecuteQueryTable(sql);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Check if JobSeekersTable record exists at all
                    if (row["nationality"] == DBNull.Value)
                        return false; // No profile record exists

                    // Check only the basic required profile fields
                    return !string.IsNullOrWhiteSpace(row["gender"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["nationality"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["dob"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["bloodgroup"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["jsaddress"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["maritalstatus"].ToString()) &&
                           !string.IsNullOrWhiteSpace(row["religion"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking profile: " + ex.Message);
            }
            return false;
        }

        

        

        

        // Common method to handle job application
        private void ApplyForJob(DataGridView dataGridView)
        {
            try
            {
                // Check if a row is selected
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a job first to apply.",
                                    "No Job Selected",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Get jobId from hidden column
                object jobIdObj = dataGridView.SelectedRows[0].Cells["jobid"].Value;
                if (jobIdObj == null || string.IsNullOrWhiteSpace(jobIdObj.ToString()))
                {
                    MessageBox.Show("Unable to identify the selected job.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                string jobId = jobIdObj.ToString();

                // Open application form and pass jobId and userId
                jsApplicationForm appForm = new jsApplicationForm();
                appForm.JobId = jobId;
                appForm.UserId = _userID;
                appForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying for job: " + ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnViewOrUpdateApplication_Click(object sender, EventArgs e)
        {
            if (dgvApplications.SelectedRows.Count > 0)
            {
                string applicationId = dgvApplications.SelectedRows[0].Cells["applicationId"].Value.ToString();
                ViewApplicationByJobseeker viewForm = new ViewApplicationByJobseeker(applicationId, _userID);
                viewForm.ShowDialog();
                LoadApplicationsForJobseeker(); // Refresh the grid after closing
            }
            else
            {
                MessageBox.Show("Please select an application first.");
            }
        }

        // ✅ Handle when user closes the form with X button
        private void jsDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Ask for confirmation
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to exit? This will log you out.",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Record logout session
                    PerformLogout();
                }
                else
                {
                    e.Cancel = true; // Cancel the form closing
                }
            }
        }

        private void LoadAvailableJobsWithFilter(string searchKeyword = "", string employmentFilter = "All")
        {
            try
            {
                string sql = @"
SELECT 
    j.jobid,
    e.companyname AS companyname,
    e.industry AS industry,
    e.address AS address,
    e.website AS website,
    e.about AS About,
    j.jobtitle AS jobtitle,
    j.employmenttype AS employmenttype,
    j.salaryrange AS salaryrange,
    j.experiencerequired AS experiencerequired,
    j.deadline AS deadline
FROM JobsTable j
INNER JOIN EmployersTable e ON j.employerid = e.employerid
WHERE j.status = 'Active' AND j.deadline >= GETDATE()";

                SqlCommand cmd = new SqlCommand(sql, da.Sqlcon);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    sql += " AND (j.jobtitle LIKE @keyword OR e.companyname LIKE @keyword)";
                    cmd.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");
                }

                // Apply employment type filter
                if (employmentFilter != "All")
                {
                    sql += " AND j.employmenttype = @employmentFilter";
                    cmd.Parameters.AddWithValue("@employmentFilter", employmentFilter);
                }

                cmd.CommandText = sql;
                DataSet ds = da.ExecuteQuery(cmd);

                // Clear selection before processing
                dgvJobs.ClearSelection();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dgvJobs.AutoGenerateColumns = false;
                    dgvJobs.Columns.Clear();

                    // Add ALL columns (same as original)
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "jobtitle",
                        HeaderText = "Job title",
                        DataPropertyName = "jobtitle"
                    });

                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "companyname",
                        HeaderText = "Company",
                        DataPropertyName = "companyname"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "industry",
                        HeaderText = "Industry",
                        DataPropertyName = "industry"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "address",
                        HeaderText = "Location",
                        DataPropertyName = "address"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "website",
                        HeaderText = "Website",
                        DataPropertyName = "website"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "about",
                        HeaderText = "About",
                        DataPropertyName = "about"
                    });
                    
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "employmenttype",
                        HeaderText = "Employment Type",
                        DataPropertyName = "employmenttype"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "salaryrange",
                        HeaderText = "Salary range",
                        DataPropertyName = "salaryrange"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "experiencerequired",
                        HeaderText = "Experience required",
                        DataPropertyName = "experiencerequired"
                    });
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "deadline",
                        HeaderText = "Deadline",
                        DataPropertyName = "deadline"
                    });
                    // Hidden JobID column
                    dgvJobs.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "jobid",
                        HeaderText = "Job ID",
                        DataPropertyName = "jobid",
                        Visible = false
                    });

                    dgvJobs.DataSource = ds.Tables[0];
                    dgvJobs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dgvJobs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvJobs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                    // Clear selection after loading
                    dgvJobs.ClearSelection();
                }
                else
                {
                    dgvJobs.DataSource = null;

                    // Show message only if searching or filtering (not when loading all)
                    if (!string.IsNullOrEmpty(searchKeyword) || employmentFilter != "All")
                    {
                        MessageBox.Show("No jobs found with the current search criteria.",
                                      "No Results Found",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }

                    dgvJobs.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading jobs: " + ex.Message);
                dgvJobs.ClearSelection();
            }
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            // Clear the search textbox
            tbSearchId.Text = "";

            // Reset filter to "All"
            cbFilterEmploymentType.SelectedItem = "All";

            // Clear DataGridView selection FIRST
            dgvJobs.ClearSelection();

            // Reload all available jobs (no filters)
            LoadAvailableJobsWithFilter("", "All");

            // Remove focus from any control
            this.ActiveControl = null;
            this.Focus();
        }

        private void btnSearchJobs_Click(object sender, EventArgs e)
        {
            string searchText = tbSearchId.Text?.Trim() ?? "";
            string employmentFilter = cbFilterEmploymentType.SelectedItem?.ToString() ?? "All";

            // Validate empty search
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a job title or company name to search.",
                              "Search Required",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            LoadAvailableJobsWithFilter(searchText, employmentFilter);
        }

        private void cbFilterEmploymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string searchText = tbSearchId.Text?.Trim() ?? "";
            string employmentFilter = cbFilterEmploymentType.SelectedItem?.ToString() ?? "All";

            LoadAvailableJobsWithFilter(searchText, employmentFilter);
        }
    }
}