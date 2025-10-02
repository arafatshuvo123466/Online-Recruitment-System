namespace Employee_Management_System
{
    partial class recoverPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(recoverPassword));
            this.tbUserID = new System.Windows.Forms.TextBox();
            this.lblEnterUserID = new System.Windows.Forms.Label();
            this.btnLoginNow = new System.Windows.Forms.Button();
            this.tbRetype = new System.Windows.Forms.TextBox();
            this.lblRetypePass = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.lblVerify = new System.Windows.Forms.Label();
            this.lblRecover = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbUserID
            // 
            this.tbUserID.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbUserID.Location = new System.Drawing.Point(247, 143);
            this.tbUserID.Multiline = true;
            this.tbUserID.Name = "tbUserID";
            this.tbUserID.Size = new System.Drawing.Size(193, 28);
            this.tbUserID.TabIndex = 54;
            // 
            // lblEnterUserID
            // 
            this.lblEnterUserID.AutoSize = true;
            this.lblEnterUserID.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnterUserID.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblEnterUserID.Location = new System.Drawing.Point(244, 121);
            this.lblEnterUserID.Name = "lblEnterUserID";
            this.lblEnterUserID.Size = new System.Drawing.Size(98, 17);
            this.lblEnterUserID.TabIndex = 53;
            this.lblEnterUserID.Text = "Enter user ID :";
            // 
            // btnLoginNow
            // 
            this.btnLoginNow.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnLoginNow.Font = new System.Drawing.Font("Teko", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoginNow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLoginNow.Location = new System.Drawing.Point(21, 339);
            this.btnLoginNow.Name = "btnLoginNow";
            this.btnLoginNow.Size = new System.Drawing.Size(151, 32);
            this.btnLoginNow.TabIndex = 52;
            this.btnLoginNow.Text = "Reset Password";
            this.btnLoginNow.UseVisualStyleBackColor = false;
            this.btnLoginNow.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // tbRetype
            // 
            this.tbRetype.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbRetype.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRetype.Location = new System.Drawing.Point(21, 277);
            this.tbRetype.Multiline = true;
            this.tbRetype.Name = "tbRetype";
            this.tbRetype.Size = new System.Drawing.Size(419, 28);
            this.tbRetype.TabIndex = 48;
            // 
            // lblRetypePass
            // 
            this.lblRetypePass.AutoSize = true;
            this.lblRetypePass.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRetypePass.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblRetypePass.Location = new System.Drawing.Point(18, 255);
            this.lblRetypePass.Name = "lblRetypePass";
            this.lblRetypePass.Size = new System.Drawing.Size(158, 17);
            this.lblRetypePass.TabIndex = 47;
            this.lblRetypePass.Text = "Re-type new password :";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPass.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPass.Location = new System.Drawing.Point(18, 187);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(144, 17);
            this.lblPass.TabIndex = 46;
            this.lblPass.Text = "Enter new password :";
            // 
            // tbPassword
            // 
            this.tbPassword.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPassword.Location = new System.Drawing.Point(21, 209);
            this.tbPassword.Multiline = true;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(419, 28);
            this.tbPassword.TabIndex = 45;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblEmail.Location = new System.Drawing.Point(18, 121);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(102, 17);
            this.lblEmail.TabIndex = 44;
            this.lblEmail.Text = "Email address :";
            // 
            // tbEmail
            // 
            this.tbEmail.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbEmail.Location = new System.Drawing.Point(21, 143);
            this.tbEmail.Multiline = true;
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(193, 28);
            this.tbEmail.TabIndex = 43;
            // 
            // lblVerify
            // 
            this.lblVerify.AutoSize = true;
            this.lblVerify.Font = new System.Drawing.Font("Teko", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVerify.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblVerify.Location = new System.Drawing.Point(13, 59);
            this.lblVerify.Name = "lblVerify";
            this.lblVerify.Size = new System.Drawing.Size(280, 46);
            this.lblVerify.TabIndex = 38;
            this.lblVerify.Text = "Verify your identity to reset";
            // 
            // lblRecover
            // 
            this.lblRecover.AutoSize = true;
            this.lblRecover.Font = new System.Drawing.Font("Teko", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecover.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblRecover.Location = new System.Drawing.Point(8, 1);
            this.lblRecover.Name = "lblRecover";
            this.lblRecover.Size = new System.Drawing.Size(375, 76);
            this.lblRecover.TabIndex = 37;
            this.lblRecover.Text = "Recover Your Account";
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnBack.Font = new System.Drawing.Font("Teko", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnBack.Location = new System.Drawing.Point(178, 339);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(102, 32);
            this.btnBack.TabIndex = 55;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // recoverPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Indigo;
            this.ClientSize = new System.Drawing.Size(461, 395);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.tbUserID);
            this.Controls.Add(this.lblEnterUserID);
            this.Controls.Add(this.btnLoginNow);
            this.Controls.Add(this.tbRetype);
            this.Controls.Add(this.lblRetypePass);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.lblVerify);
            this.Controls.Add(this.lblRecover);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "recoverPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reset password - JobConnect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUserID;
        private System.Windows.Forms.Label lblEnterUserID;
        private System.Windows.Forms.Button btnLoginNow;
        private System.Windows.Forms.TextBox tbRetype;
        private System.Windows.Forms.Label lblRetypePass;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label lblVerify;
        private System.Windows.Forms.Label lblRecover;
        private System.Windows.Forms.Button btnBack;
    }
}