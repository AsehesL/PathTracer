namespace PathTracerForm
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new System.Windows.Forms.Panel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.renderResultBox = new System.Windows.Forms.PictureBox();
            this.logListView = new System.Windows.Forms.ListView();
            this.logType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logMsg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.multiThreadCheckBox = new System.Windows.Forms.CheckBox();
            this.settingPanel = new System.Windows.Forms.Panel();
            this.renderButton = new System.Windows.Forms.Button();
            this.fastPreviewButton = new System.Windows.Forms.Button();
            this.heightInputBox = new System.Windows.Forms.TextBox();
            this.widthInputBox = new System.Windows.Forms.TextBox();
            this.samplerTypeCombo = new System.Windows.Forms.ComboBox();
            this.numSampleInputBox = new System.Windows.Forms.TextBox();
            this.heightLabel = new System.Windows.Forms.Label();
            this.widthLabel = new System.Windows.Forms.Label();
            this.samplerTypeLabel = new System.Windows.Forms.Label();
            this.samplerLabel = new System.Windows.Forms.Label();
            this.bounceLabel = new System.Windows.Forms.Label();
            this.bounceInputBox = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip.SuspendLayout();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renderResultBox)).BeginInit();
            this.settingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 485);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(743, 23);
            this.progressBar.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(767, 25);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.FileToolStripMenuItem.Text = "文件";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.OpenToolStripMenuItem.Text = "打开";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.SaveToolStripMenuItem.Text = "保存渲染结果";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.infoLabel);
            this.panel.Controls.Add(this.fileNameLabel);
            this.panel.Controls.Add(this.renderResultBox);
            this.panel.Location = new System.Drawing.Point(260, 27);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(495, 288);
            this.panel.TabIndex = 2;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.BackColor = System.Drawing.Color.Transparent;
            this.infoLabel.ForeColor = System.Drawing.Color.Yellow;
            this.infoLabel.Location = new System.Drawing.Point(12, 25);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 12);
            this.infoLabel.TabIndex = 7;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileNameLabel.ForeColor = System.Drawing.Color.Yellow;
            this.fileNameLabel.Location = new System.Drawing.Point(12, 7);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(89, 12);
            this.fileNameLabel.TabIndex = 6;
            this.fileNameLabel.Text = "当前场景（空）";
            // 
            // renderResultBox
            // 
            this.renderResultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderResultBox.BackColor = System.Drawing.Color.Black;
            this.renderResultBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.renderResultBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.renderResultBox.Location = new System.Drawing.Point(0, 0);
            this.renderResultBox.Name = "renderResultBox";
            this.renderResultBox.Size = new System.Drawing.Size(495, 288);
            this.renderResultBox.TabIndex = 0;
            this.renderResultBox.TabStop = false;
            // 
            // logListView
            // 
            this.logListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.logType,
            this.logMsg});
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logListView.Location = new System.Drawing.Point(12, 321);
            this.logListView.MultiSelect = false;
            this.logListView.Name = "logListView";
            this.logListView.Size = new System.Drawing.Size(743, 158);
            this.logListView.TabIndex = 3;
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            // 
            // logType
            // 
            this.logType.Text = "日志类型";
            this.logType.Width = 92;
            // 
            // logMsg
            // 
            this.logMsg.Text = "日志";
            this.logMsg.Width = 642;
            // 
            // multiThreadCheckBox
            // 
            this.multiThreadCheckBox.AutoSize = true;
            this.multiThreadCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.multiThreadCheckBox.Enabled = false;
            this.multiThreadCheckBox.Location = new System.Drawing.Point(3, 3);
            this.multiThreadCheckBox.Name = "multiThreadCheckBox";
            this.multiThreadCheckBox.Size = new System.Drawing.Size(84, 16);
            this.multiThreadCheckBox.TabIndex = 4;
            this.multiThreadCheckBox.Text = "多线程渲染";
            this.multiThreadCheckBox.UseVisualStyleBackColor = true;
            // 
            // settingPanel
            // 
            this.settingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.settingPanel.Controls.Add(this.renderButton);
            this.settingPanel.Controls.Add(this.fastPreviewButton);
            this.settingPanel.Controls.Add(this.heightInputBox);
            this.settingPanel.Controls.Add(this.widthInputBox);
            this.settingPanel.Controls.Add(this.samplerTypeCombo);
            this.settingPanel.Controls.Add(this.numSampleInputBox);
            this.settingPanel.Controls.Add(this.heightLabel);
            this.settingPanel.Controls.Add(this.widthLabel);
            this.settingPanel.Controls.Add(this.samplerTypeLabel);
            this.settingPanel.Controls.Add(this.samplerLabel);
            this.settingPanel.Controls.Add(this.bounceLabel);
            this.settingPanel.Controls.Add(this.bounceInputBox);
            this.settingPanel.Controls.Add(this.multiThreadCheckBox);
            this.settingPanel.Location = new System.Drawing.Point(12, 27);
            this.settingPanel.Name = "settingPanel";
            this.settingPanel.Size = new System.Drawing.Size(242, 288);
            this.settingPanel.TabIndex = 5;
            // 
            // renderButton
            // 
            this.renderButton.Enabled = false;
            this.renderButton.Location = new System.Drawing.Point(54, 204);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(137, 23);
            this.renderButton.TabIndex = 16;
            this.renderButton.Text = "渲染";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // fastPreviewButton
            // 
            this.fastPreviewButton.Enabled = false;
            this.fastPreviewButton.Location = new System.Drawing.Point(54, 175);
            this.fastPreviewButton.Name = "fastPreviewButton";
            this.fastPreviewButton.Size = new System.Drawing.Size(137, 23);
            this.fastPreviewButton.TabIndex = 15;
            this.fastPreviewButton.Text = "快速预览";
            this.fastPreviewButton.UseVisualStyleBackColor = true;
            this.fastPreviewButton.Click += new System.EventHandler(this.fastPreviewButton_Click);
            // 
            // heightInputBox
            // 
            this.heightInputBox.Enabled = false;
            this.heightInputBox.Location = new System.Drawing.Point(75, 132);
            this.heightInputBox.Name = "heightInputBox";
            this.heightInputBox.Size = new System.Drawing.Size(164, 21);
            this.heightInputBox.TabIndex = 14;
            this.heightInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.heightInputBox_KeyPress);
            // 
            // widthInputBox
            // 
            this.widthInputBox.Enabled = false;
            this.widthInputBox.Location = new System.Drawing.Point(75, 105);
            this.widthInputBox.Name = "widthInputBox";
            this.widthInputBox.Size = new System.Drawing.Size(164, 21);
            this.widthInputBox.TabIndex = 13;
            this.widthInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.widthInputBox_KeyPress);
            // 
            // samplerTypeCombo
            // 
            this.samplerTypeCombo.Enabled = false;
            this.samplerTypeCombo.FormattingEnabled = true;
            this.samplerTypeCombo.Location = new System.Drawing.Point(75, 79);
            this.samplerTypeCombo.Name = "samplerTypeCombo";
            this.samplerTypeCombo.Size = new System.Drawing.Size(164, 20);
            this.samplerTypeCombo.TabIndex = 12;
            // 
            // numSampleInputBox
            // 
            this.numSampleInputBox.Enabled = false;
            this.numSampleInputBox.Location = new System.Drawing.Point(75, 52);
            this.numSampleInputBox.Name = "numSampleInputBox";
            this.numSampleInputBox.Size = new System.Drawing.Size(164, 21);
            this.numSampleInputBox.TabIndex = 11;
            this.numSampleInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numSampleInputBox_KeyPress);
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(3, 135);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(53, 12);
            this.heightLabel.TabIndex = 10;
            this.heightLabel.Text = "渲染高度";
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(3, 108);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(53, 12);
            this.widthLabel.TabIndex = 9;
            this.widthLabel.Text = "渲染宽度";
            // 
            // samplerTypeLabel
            // 
            this.samplerTypeLabel.AutoSize = true;
            this.samplerTypeLabel.Location = new System.Drawing.Point(3, 82);
            this.samplerTypeLabel.Name = "samplerTypeLabel";
            this.samplerTypeLabel.Size = new System.Drawing.Size(53, 12);
            this.samplerTypeLabel.TabIndex = 8;
            this.samplerTypeLabel.Text = "采样类型";
            // 
            // samplerLabel
            // 
            this.samplerLabel.AutoSize = true;
            this.samplerLabel.Location = new System.Drawing.Point(3, 55);
            this.samplerLabel.Name = "samplerLabel";
            this.samplerLabel.Size = new System.Drawing.Size(53, 12);
            this.samplerLabel.TabIndex = 7;
            this.samplerLabel.Text = "采样次数";
            // 
            // bounceLabel
            // 
            this.bounceLabel.AutoSize = true;
            this.bounceLabel.Location = new System.Drawing.Point(3, 28);
            this.bounceLabel.Name = "bounceLabel";
            this.bounceLabel.Size = new System.Drawing.Size(53, 12);
            this.bounceLabel.TabIndex = 6;
            this.bounceLabel.Text = "反弹次数";
            // 
            // bounceInputBox
            // 
            this.bounceInputBox.Enabled = false;
            this.bounceInputBox.Location = new System.Drawing.Point(75, 25);
            this.bounceInputBox.Name = "bounceInputBox";
            this.bounceInputBox.Size = new System.Drawing.Size(164, 21);
            this.bounceInputBox.TabIndex = 5;
            this.bounceInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.bounceInputBox_KeyPress);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "场景文件|*.scene";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "BMP文件|*.bmp";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(767, 520);
            this.Controls.Add(this.settingPanel);
            this.Controls.Add(this.logListView);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "A$heslL光线追踪器v1.0";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renderResultBox)).EndInit();
            this.settingPanel.ResumeLayout(false);
            this.settingPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ListView logListView;
        private System.Windows.Forms.ColumnHeader logType;
        private System.Windows.Forms.ColumnHeader logMsg;
        private System.Windows.Forms.PictureBox renderResultBox;
        private System.Windows.Forms.CheckBox multiThreadCheckBox;
        private System.Windows.Forms.Panel settingPanel;
        private System.Windows.Forms.Button renderButton;
        private System.Windows.Forms.Button fastPreviewButton;
        private System.Windows.Forms.TextBox heightInputBox;
        private System.Windows.Forms.TextBox widthInputBox;
        private System.Windows.Forms.ComboBox samplerTypeCombo;
        private System.Windows.Forms.TextBox numSampleInputBox;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label samplerTypeLabel;
        private System.Windows.Forms.Label samplerLabel;
        private System.Windows.Forms.Label bounceLabel;
        private System.Windows.Forms.TextBox bounceInputBox;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

