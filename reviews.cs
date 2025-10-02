using DocumentFormat.OpenXml.Spreadsheet;
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
    public partial class reviews : Form
    {
        private const int MAX_CHARACTERS = 999;
        private bool maxLengthReached = false;
        private string _userId;
        private string _userType;
        private string _selectedReviewId = string.Empty;
        private string _originalReviewText = string.Empty;
        private readonly dataAccess da;

        public reviews(string userId, string userType)
        {
            InitializeComponent();
            _userId = userId;
            _userType = userType;

            //labelCharCount.Font = new Font("Consolas", labelCharCount.Font.Size);
            da = new dataAccess();

            cbFilterReviews.SelectedIndexChanged += cbFilterReviews_SelectedIndexChanged;
            dgvReviews.CellDoubleClick += dgvReviews_CellDoubleClick;
        }

        private void reviews_Load(object sender, EventArgs e)
        {
            UpdateCharacterCount();

            // Initialize combobox selection
            cbFilterReviews.SelectedIndex = 0; // Select "All" by default

            LoadReviews("All"); // Load all reviews initially
            RemoveFocusFromDataGridView();
        }

         

        private void ResetFilter()
        {
            // Temporarily remove event handler to prevent triggering
            cbFilterReviews.SelectedIndexChanged -= cbFilterReviews_SelectedIndexChanged;

            // Set filter back to "All"
            cbFilterReviews.SelectedIndex = 0;

            // Add event handler back
            cbFilterReviews.SelectedIndexChanged += cbFilterReviews_SelectedIndexChanged;
        }

        private void LoadReviews(string filterType = "All")
        {
            try
            {
                string query = @"
SELECT 
    u.firstName + ' ' + u.lastName AS UserName,
    r.reviewtext AS Review,
    r.posteddate AS PublishedDate,
    r.postedtime AS PublishedTime,
    r.rid,
    r.userID
FROM ReviewsTable r
INNER JOIN UsersTable u ON r.userID = u.userID";

                // Add filter condition if "Your reviews" is selected
                if (filterType == "Your reviews")
                {
                    query += $" WHERE r.userID = '{_userId}'";
                }

                query += " ORDER BY r.posteddate DESC, r.postedtime DESC";

                DataTable dt = da.ExecuteQueryTable(query);
                dgvReviews.DataSource = dt;
                ConfigureGridView();

                // Show message if no reviews found for "Your reviews" filter
                if (filterType == "Your reviews" && (dt == null || dt.Rows.Count == 0))
                {
                    MessageBox.Show("You haven't posted any reviews yet.", "No Reviews",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reviews: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridView()
        {
            if (dgvReviews.Columns.Count > 0)
            {
                // Hide unnecessary columns
                dgvReviews.Columns["rid"].Visible = false;
                dgvReviews.Columns["userID"].Visible = false;

                // Set column headers
                dgvReviews.Columns["UserName"].HeaderText = "User";
                dgvReviews.Columns["Review"].HeaderText = "Review";
                dgvReviews.Columns["PublishedDate"].HeaderText = "Published date";
                dgvReviews.Columns["PublishedTime"].HeaderText = "Published time";

                // Format the columns
                dgvReviews.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvReviews.Columns["Review"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvReviews.Columns["PublishedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvReviews.Columns["PublishedTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Make review text wrap
                dgvReviews.Columns["Review"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvReviews.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }

        private string GenerateReviewId()
        {
            try
            {
                // Get the maximum numeric part of existing review IDs
                string query = @"
SELECT ISNULL(MAX(CAST(SUBSTRING(rid, 3, LEN(rid)-2) AS INT)), 95000) 
FROM ReviewsTable 
WHERE rid LIKE 'R-%' AND ISNUMERIC(SUBSTRING(rid, 3, LEN(rid)-2)) = 1";

                int maxId = Convert.ToInt32(da.ExecuteScalarQuery(query));

                // Return the next ID in sequence
                return $"R-{maxId + 1}";
            }
            catch (Exception ex)
            {
                // Fallback: use count + 95000 if the above fails
                try
                {
                    string query = "SELECT COUNT(*) FROM ReviewsTable";
                    int count = Convert.ToInt32(da.ExecuteScalarQuery(query));
                    return $"R-{95000 + count + 1}";
                }
                catch
                {
                    // Final fallback: random number
                    return $"R-{95000 + new Random().Next(1000, 9999)}";
                }
            }
        }

        private void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbReview.Text))
            {
                MessageBox.Show("Please write a review before submitting.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Generate a unique review ID
                string reviewId = GenerateReviewId();

                // Get current date and time in the required format
                string currentDate = DateTime.Now.ToString("dd MMM, yyyy");
                string currentTime = DateTime.Now.ToString("hh:mm tt").ToLower();

                string query = $@"
INSERT INTO ReviewsTable (rid, userID, reviewtext, posteddate, postedtime)
VALUES ('{reviewId}', '{_userId}', '{tbReview.Text.Trim().Replace("'", "''")}', 
        '{currentDate}', '{currentTime}')";

                int rowsAffected = da.ExecuteUpdateQuery(query);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Review submitted successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear selection and textbox
                    ClearReviewSelection();

                    // Reset filter to "All" and reload
                    ResetFilter();
                    LoadReviews("All");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting review: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveFocusFromDataGridView()
        {
            if (dgvReviews != null)
            {
                dgvReviews.ClearSelection();
                dgvReviews.CurrentCell = null;
                dgvReviews.TabStop = false;
            }
        }

        private void ClearReviewSelection()
        {
            if (dgvReviews != null)
            {
                dgvReviews.ClearSelection();
                dgvReviews.CurrentCell = null;
                _selectedReviewId = string.Empty;
                _originalReviewText = string.Empty;
                tbReview.Clear();
            }
        }





        private void tbReview_TextChanged(object sender, EventArgs e)
        {
            UpdateCharacterCount();

            
            if (tbReview.Text.Length > MAX_CHARACTERS)
            {
                tbReview.Text = tbReview.Text.Substring(0, MAX_CHARACTERS);
                tbReview.SelectionStart = MAX_CHARACTERS;

                if (!maxLengthReached)
                {
                    MessageBox.Show($"Maximum character limit reached! Only {MAX_CHARACTERS} characters are allowed.",
                                  "Character limit exceeded",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    maxLengthReached = true;
                }
            }
            else
            {
                maxLengthReached = false;
            }
        }

        private void UpdateCharacterCount()
        {
            //int currentCount = tbReview.Text.Length;
            //labelCharCount.Text = $"{currentCount}/{MAX_CHARACTERS}";

            int currentCount = tbReview.Text.Length;
            labelCharCount.Text = $"{currentCount,3}/999"; // Right-align within 3 spaces
        }

        // Optional: Prevent paste operations that would exceed the limit
        private void tbReview_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle Ctrl+V (paste)
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    int totalLength = tbReview.Text.Length + clipboardText.Length;

                    if (totalLength > MAX_CHARACTERS)
                    {
                        e.SuppressKeyPress = true;
                        MessageBox.Show($"Pasting this text would exceed the {MAX_CHARACTERS} character limit.",
                                      "Paste Warning",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void dgvReviews_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvReviews.Rows.Count)
            {
                // Get the userID from the selected row
                string reviewUserId = dgvReviews.Rows[e.RowIndex].Cells["userID"]?.Value?.ToString();
                string reviewText = dgvReviews.Rows[e.RowIndex].Cells["Review"]?.Value?.ToString();
                string reviewId = dgvReviews.Rows[e.RowIndex].Cells["rid"]?.Value?.ToString();

                // Store the selected review information
                _selectedReviewId = reviewId;
                _originalReviewText = reviewText;

                // For double-click: only populate textbox if it's the user's own review
                if (reviewUserId == _userId)
                {
                    // This will be used for update functionality
                }
            }
        }

        private void dgvReviews_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ClearReviewSelection();
        }

        private void cbFilterReviews_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterReviews.SelectedItem != null)
            {
                string selectedFilter = cbFilterReviews.SelectedItem.ToString();

                // Clear any existing selection when filter changes
                ClearReviewSelection();

                LoadReviews(selectedFilter);
            }
        }

        private void btnUpdateReview_Click(object sender, EventArgs e)
        {
            // Check if a review is selected for update
            if (string.IsNullOrEmpty(_selectedReviewId))
            {
                MessageBox.Show("Please select your review to update (double-click on your review).",
                                "No Review Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the review text is empty
            if (string.IsNullOrWhiteSpace(tbReview.Text))
            {
                MessageBox.Show("Review text cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the review text has actually changed
            if (tbReview.Text.Trim() == _originalReviewText)
            {
                MessageBox.Show("No changes detected. Please modify the review text before updating.",
                                "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string updatedReviewText = tbReview.Text.Trim().Replace("'", "''");

                string query = $@"
UPDATE ReviewsTable 
SET reviewtext = '{updatedReviewText}'
WHERE rid = '{_selectedReviewId}' AND userID = '{_userId}'";

                int rowsAffected = da.ExecuteUpdateQuery(query);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Review updated successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear selection and reset variables
                    ClearReviewSelection();

                    // Reload reviews based on current filter
                    string currentFilter = cbFilterReviews.SelectedItem?.ToString() ?? "All";
                    LoadReviews(currentFilter);
                }
                else
                {
                    MessageBox.Show("Failed to update review. The review may not exist or you don't have permission to update it.",
                                    "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating review: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteReview_Click(object sender, EventArgs e)
        {
            // Check if a review is selected for deletion
            if (string.IsNullOrEmpty(_selectedReviewId))
            {
                MessageBox.Show("Please select a review to delete (click on a row first).",
                                "No Review Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected row to check ownership
            if (dgvReviews.CurrentRow == null)
            {
                MessageBox.Show("Please select a valid review.",
                                "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reviewUserId = dgvReviews.CurrentRow.Cells["userID"]?.Value?.ToString();

            // Check if the review belongs to the logged-in user
            if (reviewUserId != _userId)
            {
                MessageBox.Show("You can only delete your own reviews.",
                                "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm deletion with the user
            DialogResult result = MessageBox.Show("Are you sure you want to delete this review? This action cannot be undone.",
                                                 "Confirm Deletion",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning,
                                                 MessageBoxDefaultButton.Button2);

            if (result == DialogResult.No)
            {
                return; // User cancelled the deletion
            }

            try
            {
                string query = $@"
DELETE FROM ReviewsTable 
WHERE rid = '{_selectedReviewId}' AND userID = '{_userId}'";

                int rowsAffected = da.ExecuteUpdateQuery(query);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Review deleted successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear selection and reset variables
                    ClearReviewSelection();

                    // Reload reviews based on current filter
                    string currentFilter = cbFilterReviews.SelectedItem?.ToString() ?? "All";
                    LoadReviews(currentFilter);
                }
                else
                {
                    MessageBox.Show("Failed to delete review. The review may not exist or you don't have permission to delete it.",
                                    "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting review: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvReviews_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvReviews.Rows.Count)
            {
                // Get the userID from the selected row
                string reviewUserId = dgvReviews.Rows[e.RowIndex].Cells["userID"]?.Value?.ToString();
                string reviewText = dgvReviews.Rows[e.RowIndex].Cells["Review"]?.Value?.ToString();
                string reviewId = dgvReviews.Rows[e.RowIndex].Cells["rid"]?.Value?.ToString();

                // Check if the review belongs to the logged-in user
                if (reviewUserId == _userId)
                {
                    // Populate the textbox with the review text for update
                    tbReview.Text = reviewText;
                    _selectedReviewId = reviewId;
                    _originalReviewText = reviewText;

                    // Optional: Focus on the textbox for editing
                    tbReview.Focus();
                }
                // If it's not the user's review, do nothing (no message box)
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Logout confirmation
            DialogResult logoutResult = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Only proceed if user clicks Yes
            if (logoutResult == DialogResult.Yes)
            {
                PerformLogout();
            }
            // If No is clicked, do nothing and stay on the form
        }

        // ✅ NEW: Method to handle actual logout with session recording
        private void PerformLogout()
        {
            try
            {
                // Extract user ID and session ID from Tag (passed from dashboard)
                string tagValue = this.Tag?.ToString();
                string[] tagParts = tagValue?.Split('|');

                string userId = _userId;
                string sessionId = tagParts?.Length > 1 ? tagParts[1] : string.Empty;

                // Record logout time
                bool logoutRecorded = SessionTracker.RecordLogout(sessionId, userId);

                if (!logoutRecorded)
                {
                    MessageBox.Show("Warning: Could not record logout time.",
                                  "Session Warning",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }

                // Go back to login page
                login loginForm = new login();
                loginForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during logout: " + ex.Message,
                              "Logout Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                // Still proceed to login page even if session recording fails
                login loginForm = new login();
                loginForm.Show();
                this.Hide();
            }
        }

        // ✅ Handle when user closes the review form with X button
        private void reviews_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}