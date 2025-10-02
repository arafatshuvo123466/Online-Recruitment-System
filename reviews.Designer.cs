namespace Employee_Management_System
{
    partial class reviews
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(reviews));
            this.lblExpMatters = new System.Windows.Forms.Label();
            this.lblThoughts = new System.Windows.Forms.Label();
            this.btnSubmitReview = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.dgvReviews = new System.Windows.Forms.DataGridView();
            this.username = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.review = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.publisheddate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.publishedtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbFilterReviews = new System.Windows.Forms.ComboBox();
            this.lblFilterReviews = new System.Windows.Forms.Label();
            this.btnUpdateReview = new System.Windows.Forms.Button();
            this.btnDeleteReview = new System.Windows.Forms.Button();
            this.labelCharCount = new System.Windows.Forms.Label();
            this.tbReview = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReviews)).BeginInit();
            this.SuspendLayout();
            // 
            // lblExpMatters
            // 
            this.lblExpMatters.AutoSize = true;
            this.lblExpMatters.Font = new System.Drawing.Font("Teko", 38.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpMatters.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblExpMatters.Location = new System.Drawing.Point(8, 5);
            this.lblExpMatters.Name = "lblExpMatters";
            this.lblExpMatters.Size = new System.Drawing.Size(394, 73);
            this.lblExpMatters.TabIndex = 18;
            this.lblExpMatters.Text = "Your experience matters";
            // 
            // lblThoughts
            // 
            this.lblThoughts.AutoSize = true;
            this.lblThoughts.Font = new System.Drawing.Font("Teko", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThoughts.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblThoughts.Location = new System.Drawing.Point(12, 62);
            this.lblThoughts.Name = "lblThoughts";
            this.lblThoughts.Size = new System.Drawing.Size(491, 51);
            this.lblThoughts.TabIndex = 19;
            this.lblThoughts.Text = "Your thoughts help us to improve the platform";
            // 
            // btnSubmitReview
            // 
            this.btnSubmitReview.Font = new System.Drawing.Font("Teko", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmitReview.Location = new System.Drawing.Point(18, 447);
            this.btnSubmitReview.Name = "btnSubmitReview";
            this.btnSubmitReview.Size = new System.Drawing.Size(138, 26);
            this.btnSubmitReview.TabIndex = 111;
            this.btnSubmitReview.Text = "Submit review";
            this.btnSubmitReview.UseVisualStyleBackColor = true;
            this.btnSubmitReview.Click += new System.EventHandler(this.btnSubmitReview_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Font = new System.Drawing.Font("Teko", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.Location = new System.Drawing.Point(320, 447);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(73, 26);
            this.btnLogout.TabIndex = 112;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // dgvReviews
            // 
            this.dgvReviews.AllowUserToAddRows = false;
            this.dgvReviews.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReviews.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReviews.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReviews.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.username,
            this.review,
            this.publisheddate,
            this.publishedtime});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReviews.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReviews.Location = new System.Drawing.Point(19, 162);
            this.dgvReviews.Name = "dgvReviews";
            this.dgvReviews.ReadOnly = true;
            this.dgvReviews.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReviews.Size = new System.Drawing.Size(502, 175);
            this.dgvReviews.TabIndex = 113;
            this.dgvReviews.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvReviews_CellClick);
            this.dgvReviews.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvReviews_CellDoubleClick);
            this.dgvReviews.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvReviews_DataBindingComplete);
            // 
            // username
            // 
            this.username.DataPropertyName = "UserName";
            this.username.HeaderText = "User";
            this.username.Name = "username";
            this.username.ReadOnly = true;
            // 
            // review
            // 
            this.review.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.review.DataPropertyName = "Review";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.review.DefaultCellStyle = dataGridViewCellStyle2;
            this.review.HeaderText = "Review";
            this.review.MinimumWidth = 290;
            this.review.Name = "review";
            this.review.ReadOnly = true;
            // 
            // publisheddate
            // 
            this.publisheddate.DataPropertyName = "PublishedDate";
            this.publisheddate.HeaderText = "Published date";
            this.publisheddate.Name = "publisheddate";
            this.publisheddate.ReadOnly = true;
            // 
            // publishedtime
            // 
            this.publishedtime.DataPropertyName = "PublishedTime";
            this.publishedtime.HeaderText = "Published time";
            this.publishedtime.Name = "publishedtime";
            this.publishedtime.ReadOnly = true;
            // 
            // cbFilterReviews
            // 
            this.cbFilterReviews.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cbFilterReviews.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterReviews.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.cbFilterReviews.FormattingEnabled = true;
            this.cbFilterReviews.Items.AddRange(new object[] {
            "All",
            "Your reviews"});
            this.cbFilterReviews.Location = new System.Drawing.Point(122, 124);
            this.cbFilterReviews.Name = "cbFilterReviews";
            this.cbFilterReviews.Size = new System.Drawing.Size(113, 21);
            this.cbFilterReviews.TabIndex = 114;
            this.cbFilterReviews.SelectedIndexChanged += new System.EventHandler(this.cbFilterReviews_SelectedIndexChanged);
            // 
            // lblFilterReviews
            // 
            this.lblFilterReviews.AutoSize = true;
            this.lblFilterReviews.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterReviews.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblFilterReviews.Location = new System.Drawing.Point(18, 125);
            this.lblFilterReviews.Name = "lblFilterReviews";
            this.lblFilterReviews.Size = new System.Drawing.Size(102, 17);
            this.lblFilterReviews.TabIndex = 115;
            this.lblFilterReviews.Text = "Filter reviews :";
            // 
            // btnUpdateReview
            // 
            this.btnUpdateReview.Font = new System.Drawing.Font("Teko", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateReview.Location = new System.Drawing.Point(162, 447);
            this.btnUpdateReview.Name = "btnUpdateReview";
            this.btnUpdateReview.Size = new System.Drawing.Size(73, 26);
            this.btnUpdateReview.TabIndex = 116;
            this.btnUpdateReview.Text = "Update";
            this.btnUpdateReview.UseVisualStyleBackColor = true;
            this.btnUpdateReview.Click += new System.EventHandler(this.btnUpdateReview_Click);
            // 
            // btnDeleteReview
            // 
            this.btnDeleteReview.Font = new System.Drawing.Font("Teko", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteReview.Location = new System.Drawing.Point(241, 447);
            this.btnDeleteReview.Name = "btnDeleteReview";
            this.btnDeleteReview.Size = new System.Drawing.Size(73, 26);
            this.btnDeleteReview.TabIndex = 117;
            this.btnDeleteReview.Text = "Delete";
            this.btnDeleteReview.UseVisualStyleBackColor = true;
            this.btnDeleteReview.Click += new System.EventHandler(this.btnDeleteReview_Click);
            // 
            // labelCharCount
            // 
            this.labelCharCount.AutoSize = true;
            this.labelCharCount.BackColor = System.Drawing.SystemColors.ControlLight;
            this.labelCharCount.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCharCount.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelCharCount.Location = new System.Drawing.Point(453, 416);
            this.labelCharCount.Name = "labelCharCount";
            this.labelCharCount.Size = new System.Drawing.Size(37, 13);
            this.labelCharCount.TabIndex = 118;
            this.labelCharCount.Text = "0/999";
            // 
            // tbReview
            // 
            this.tbReview.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbReview.Location = new System.Drawing.Point(19, 354);
            this.tbReview.Multiline = true;
            this.tbReview.Name = "tbReview";
            this.tbReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbReview.Size = new System.Drawing.Size(502, 76);
            this.tbReview.TabIndex = 0;
            this.tbReview.TextChanged += new System.EventHandler(this.tbReview_TextChanged);
            // 
            // reviews
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Indigo;
            this.ClientSize = new System.Drawing.Size(540, 488);
            this.Controls.Add(this.labelCharCount);
            this.Controls.Add(this.btnDeleteReview);
            this.Controls.Add(this.btnUpdateReview);
            this.Controls.Add(this.lblFilterReviews);
            this.Controls.Add(this.cbFilterReviews);
            this.Controls.Add(this.dgvReviews);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnSubmitReview);
            this.Controls.Add(this.lblThoughts);
            this.Controls.Add(this.lblExpMatters);
            this.Controls.Add(this.tbReview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "reviews";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Submit a review - JobConnect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.reviews_FormClosing);
            this.Load += new System.EventHandler(this.reviews_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReviews)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblExpMatters;
        private System.Windows.Forms.Label lblThoughts;
        private System.Windows.Forms.Button btnSubmitReview;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.DataGridView dgvReviews;
        private System.Windows.Forms.ComboBox cbFilterReviews;
        private System.Windows.Forms.Label lblFilterReviews;
        private System.Windows.Forms.Button btnUpdateReview;
        private System.Windows.Forms.Button btnDeleteReview;
        private System.Windows.Forms.Label labelCharCount;
        private System.Windows.Forms.TextBox tbReview;
        private System.Windows.Forms.DataGridViewTextBoxColumn username;
        private System.Windows.Forms.DataGridViewTextBoxColumn review;
        private System.Windows.Forms.DataGridViewTextBoxColumn publisheddate;
        private System.Windows.Forms.DataGridViewTextBoxColumn publishedtime;
    }
}