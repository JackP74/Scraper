
namespace Scraper
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.ListURLs = new System.Windows.Forms.ListView();
            this.ColumnURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TxtAddURL = new System.Windows.Forms.TextBox();
            this.BtnAddURL = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.ContextListURLs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextListURLsCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextListURLsOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextListURLs.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListURLs
            // 
            this.ListURLs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListURLs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnURL,
            this.ColumnActive,
            this.ColumnStatus});
            this.ListURLs.ContextMenuStrip = this.ContextListURLs;
            this.ListURLs.FullRowSelect = true;
            this.ListURLs.HideSelection = false;
            this.ListURLs.Location = new System.Drawing.Point(4, 4);
            this.ListURLs.Margin = new System.Windows.Forms.Padding(4);
            this.ListURLs.Name = "ListURLs";
            this.ListURLs.Size = new System.Drawing.Size(667, 269);
            this.ListURLs.TabIndex = 0;
            this.ListURLs.UseCompatibleStateImageBehavior = false;
            this.ListURLs.View = System.Windows.Forms.View.Details;
            // 
            // ColumnURL
            // 
            this.ColumnURL.Text = "URL";
            this.ColumnURL.Width = 455;
            // 
            // ColumnActive
            // 
            this.ColumnActive.Text = "Active";
            this.ColumnActive.Width = 82;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.Text = "Status";
            this.ColumnStatus.Width = 88;
            // 
            // TxtAddURL
            // 
            this.TxtAddURL.Location = new System.Drawing.Point(4, 276);
            this.TxtAddURL.Margin = new System.Windows.Forms.Padding(4);
            this.TxtAddURL.Name = "TxtAddURL";
            this.TxtAddURL.Size = new System.Drawing.Size(565, 22);
            this.TxtAddURL.TabIndex = 1;
            // 
            // BtnAddURL
            // 
            this.BtnAddURL.Location = new System.Drawing.Point(573, 274);
            this.BtnAddURL.Margin = new System.Windows.Forms.Padding(4);
            this.BtnAddURL.Name = "BtnAddURL";
            this.BtnAddURL.Size = new System.Drawing.Size(100, 27);
            this.BtnAddURL.TabIndex = 2;
            this.BtnAddURL.Text = "Add URL";
            this.BtnAddURL.UseVisualStyleBackColor = true;
            this.BtnAddURL.Click += new System.EventHandler(this.BtnAddURL_Click);
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(472, 305);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(4);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(201, 28);
            this.BtnStart.TabIndex = 3;
            this.BtnStart.Text = "Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // LabelStatus
            // 
            this.LabelStatus.AutoSize = true;
            this.LabelStatus.Location = new System.Drawing.Point(8, 311);
            this.LabelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(90, 17);
            this.LabelStatus.TabIndex = 4;
            this.LabelStatus.Text = "Status: Idle...";
            // 
            // ContextListURLs
            // 
            this.ContextListURLs.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextListURLs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextListURLsCopy,
            this.ContextListURLsOpen});
            this.ContextListURLs.Name = "ContextListURLs";
            this.ContextListURLs.Size = new System.Drawing.Size(115, 52);
            // 
            // ContextListURLsCopy
            // 
            this.ContextListURLsCopy.Name = "ContextListURLsCopy";
            this.ContextListURLsCopy.Size = new System.Drawing.Size(114, 24);
            this.ContextListURLsCopy.Text = "Copy";
            this.ContextListURLsCopy.Click += new System.EventHandler(this.ContextListURLsCopy_Click);
            // 
            // ContextListURLsOpen
            // 
            this.ContextListURLsOpen.Name = "ContextListURLsOpen";
            this.ContextListURLsOpen.Size = new System.Drawing.Size(114, 24);
            this.ContextListURLsOpen.Text = "Open";
            this.ContextListURLsOpen.Click += new System.EventHandler(this.ContextListURLsOpen_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 336);
            this.Controls.Add(this.LabelStatus);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.BtnAddURL);
            this.Controls.Add(this.TxtAddURL);
            this.Controls.Add(this.ListURLs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scraper";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ContextListURLs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ListURLs;
        private System.Windows.Forms.ColumnHeader ColumnURL;
        private System.Windows.Forms.ColumnHeader ColumnActive;
        private System.Windows.Forms.ColumnHeader ColumnStatus;
        private System.Windows.Forms.TextBox TxtAddURL;
        private System.Windows.Forms.Button BtnAddURL;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.ContextMenuStrip ContextListURLs;
        private System.Windows.Forms.ToolStripMenuItem ContextListURLsCopy;
        private System.Windows.Forms.ToolStripMenuItem ContextListURLsOpen;
    }
}