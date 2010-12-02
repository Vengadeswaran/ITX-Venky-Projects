namespace Installer
{
    partial class LicensingITXDM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicensingITXDM));
            this.office2007BlackTheme1 = new Telerik.WinControls.Themes.Office2007BlackTheme();
            this.PnlActivated = new Telerik.WinControls.UI.RadPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.PnlToActivate = new Telerik.WinControls.UI.RadPanel();
            this.TxtUrl = new System.Windows.Forms.TextBox();
            this.LblStatus = new System.Windows.Forms.Label();
            this.BtnRefresh = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.PnlActivated)).BeginInit();
            this.PnlActivated.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PnlToActivate)).BeginInit();
            this.PnlToActivate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // PnlActivated
            // 
            this.PnlActivated.Controls.Add(this.label1);
            this.PnlActivated.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlActivated.Location = new System.Drawing.Point(0, 77);
            this.PnlActivated.Name = "PnlActivated";
            this.PnlActivated.Size = new System.Drawing.Size(501, 76);
            this.PnlActivated.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(103, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 15);
            this.label1.TabIndex = 44;
            this.label1.Text = "The Licensing is Activated Already. Thank you.";
            // 
            // PnlToActivate
            // 
            this.PnlToActivate.Controls.Add(this.TxtUrl);
            this.PnlToActivate.Controls.Add(this.LblStatus);
            this.PnlToActivate.Controls.Add(this.BtnRefresh);
            this.PnlToActivate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlToActivate.Location = new System.Drawing.Point(0, 1);
            this.PnlToActivate.Name = "PnlToActivate";
            this.PnlToActivate.Size = new System.Drawing.Size(501, 76);
            this.PnlToActivate.TabIndex = 46;
            // 
            // TxtUrl
            // 
            this.TxtUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtUrl.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtUrl.Location = new System.Drawing.Point(12, 38);
            this.TxtUrl.Name = "TxtUrl";
            this.TxtUrl.Size = new System.Drawing.Size(387, 21);
            this.TxtUrl.TabIndex = 42;
            // 
            // LblStatus
            // 
            this.LblStatus.AutoSize = true;
            this.LblStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblStatus.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LblStatus.Location = new System.Drawing.Point(12, 17);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(0, 15);
            this.LblStatus.TabIndex = 43;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.AllowShowFocusCues = true;
            this.BtnRefresh.ForeColor = System.Drawing.Color.White;
            this.BtnRefresh.Location = new System.Drawing.Point(405, 38);
            this.BtnRefresh.Name = "BtnRefresh";
            // 
            // 
            // 
            this.BtnRefresh.RootElement.ForeColor = System.Drawing.Color.White;
            this.BtnRefresh.Size = new System.Drawing.Size(79, 22);
            this.BtnRefresh.TabIndex = 44;
            this.BtnRefresh.Text = "Activate";
            this.BtnRefresh.ThemeName = "Office2007Black";
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // LicensingITXDM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 153);
            this.Controls.Add(this.PnlToActivate);
            this.Controls.Add(this.PnlActivated);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LicensingITXDM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ThemeName = "Office2007Black";
            this.Load += new System.EventHandler(this.LicensingITXAP_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PnlActivated)).EndInit();
            this.PnlActivated.ResumeLayout(false);
            this.PnlActivated.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PnlToActivate)).EndInit();
            this.PnlToActivate.ResumeLayout(false);
            this.PnlToActivate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.Themes.Office2007BlackTheme office2007BlackTheme1;
        private Telerik.WinControls.UI.RadPanel PnlActivated;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadPanel PnlToActivate;
        private System.Windows.Forms.TextBox TxtUrl;
        private System.Windows.Forms.Label LblStatus;
        private Telerik.WinControls.UI.RadButton BtnRefresh;
    }
}