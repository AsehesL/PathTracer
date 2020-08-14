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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHDRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openTaskQueueMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new System.Windows.Forms.Panel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.renderResultBox = new System.Windows.Forms.PictureBox();
            this.logListView = new System.Windows.Forms.ListView();
            this.logType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logMsg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.settingPanel = new System.Windows.Forms.Panel();
            this.retonemappingButton = new System.Windows.Forms.Button();
            this.tonemappingCheckBox = new System.Windows.Forms.CheckBox();
            this.renderChannelCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.exposureInputBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.renderButton = new System.Windows.Forms.Button();
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
#if DEBUG
            this.pixelDebugCheckBox = new System.Windows.Forms.CheckBox();
#endif
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
            this.progressBar.Location = new System.Drawing.Point(12, 531);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(743, 23);
            this.progressBar.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(767, 25);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveHDRToolStripMenuItem,
            this.toolStripSeparator1,
            this.openTaskQueueMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.openToolStripMenuItem.Text = "打开...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.saveToolStripMenuItem.Text = "保存渲染结果";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveHDRToolStripMenuItem
            // 
            this.saveHDRToolStripMenuItem.Enabled = false;
            this.saveHDRToolStripMenuItem.Name = "saveHDRToolStripMenuItem";
            this.saveHDRToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.saveHDRToolStripMenuItem.Text = "保存HDR渲染结果";
            this.saveHDRToolStripMenuItem.Click += new System.EventHandler(this.SaveHDRToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // openTaskQueueMenuItem
            // 
            this.openTaskQueueMenuItem.Name = "openTaskQueueMenuItem";
            this.openTaskQueueMenuItem.Size = new System.Drawing.Size(174, 22);
            this.openTaskQueueMenuItem.Text = "运行渲染队列...";
            this.openTaskQueueMenuItem.Click += new System.EventHandler(this.OpenTaskQueueMenuItem_Click);
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
            this.panel.Size = new System.Drawing.Size(495, 334);
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
            this.renderResultBox.Size = new System.Drawing.Size(495, 334);
            this.renderResultBox.TabIndex = 0;
            this.renderResultBox.TabStop = false;
            this.renderResultBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderResultBox_MouseClick);
            // 
            // logListView
            // 
            this.logListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.logType,
            this.logMsg});
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logListView.HideSelection = false;
            this.logListView.Location = new System.Drawing.Point(12, 367);
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
            // settingPanel
            // 
            this.settingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.settingPanel.Controls.Add(this.retonemappingButton);
            this.settingPanel.Controls.Add(this.tonemappingCheckBox);
            this.settingPanel.Controls.Add(this.renderChannelCombo);
            this.settingPanel.Controls.Add(this.label2);
            this.settingPanel.Controls.Add(this.exposureInputBox);
            this.settingPanel.Controls.Add(this.label1);
            this.settingPanel.Controls.Add(this.renderButton);
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
#if DEBUG
            this.settingPanel.Controls.Add(this.pixelDebugCheckBox);
#endif
            this.settingPanel.Location = new System.Drawing.Point(12, 27);
            this.settingPanel.Name = "settingPanel";
            this.settingPanel.Size = new System.Drawing.Size(242, 334);
            this.settingPanel.TabIndex = 5;
            // 
            // retonemappingButton
            // 
            this.retonemappingButton.Enabled = false;
            this.retonemappingButton.Location = new System.Drawing.Point(75, 270);
            this.retonemappingButton.Name = "retonemappingButton";
            this.retonemappingButton.Size = new System.Drawing.Size(137, 23);
            this.retonemappingButton.TabIndex = 23;
            this.retonemappingButton.Text = "重新Tonemapping";
            this.retonemappingButton.UseVisualStyleBackColor = true;
            this.retonemappingButton.Click += new System.EventHandler(this.retonemappingButton_Click);
            // 
            // tonemappingCheckBox
            // 
            this.tonemappingCheckBox.AutoSize = true;
            this.tonemappingCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tonemappingCheckBox.Enabled = false;
            this.tonemappingCheckBox.ForeColor = System.Drawing.Color.Black;
            this.tonemappingCheckBox.Location = new System.Drawing.Point(3, 190);
            this.tonemappingCheckBox.Name = "tonemappingCheckBox";
            this.tonemappingCheckBox.Size = new System.Drawing.Size(90, 16);
            this.tonemappingCheckBox.TabIndex = 22;
            this.tonemappingCheckBox.Text = "Tonemapping";
            this.tonemappingCheckBox.UseVisualStyleBackColor = true;
            this.tonemappingCheckBox.CheckedChanged += new System.EventHandler(this.TonemappingCheckBox_CheckedChanged);
            // 
            // renderChannelCombo
            // 
            this.renderChannelCombo.Enabled = false;
            this.renderChannelCombo.FormattingEnabled = true;
            this.renderChannelCombo.Location = new System.Drawing.Point(75, 160);
            this.renderChannelCombo.Name = "renderChannelCombo";
            this.renderChannelCombo.Size = new System.Drawing.Size(164, 20);
            this.renderChannelCombo.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "渲染通道";
            // 
            // exposureInputBox
            // 
            this.exposureInputBox.Enabled = false;
            this.exposureInputBox.Location = new System.Drawing.Point(75, 214);
            this.exposureInputBox.Name = "exposureInputBox";
            this.exposureInputBox.Size = new System.Drawing.Size(164, 21);
            this.exposureInputBox.TabIndex = 18;
            this.exposureInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.exposureInputBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "曝光强度";
            // 
            // renderButton
            // 
            this.renderButton.Enabled = false;
            this.renderButton.Location = new System.Drawing.Point(75, 241);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(137, 23);
            this.renderButton.TabIndex = 16;
            this.renderButton.Text = "渲染";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // heightInputBox
            // 
            this.heightInputBox.Enabled = false;
            this.heightInputBox.Location = new System.Drawing.Point(75, 133);
            this.heightInputBox.Name = "heightInputBox";
            this.heightInputBox.Size = new System.Drawing.Size(164, 21);
            this.heightInputBox.TabIndex = 14;
            this.heightInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.heightInputBox_KeyPress);
            // 
            // widthInputBox
            // 
            this.widthInputBox.Enabled = false;
            this.widthInputBox.Location = new System.Drawing.Point(75, 106);
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
            this.heightLabel.Location = new System.Drawing.Point(3, 136);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(53, 12);
            this.heightLabel.TabIndex = 10;
            this.heightLabel.Text = "渲染高度";
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(3, 109);
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
#if DEBUG
            // 
            // pixelDebugCheckBox
            // 
            this.pixelDebugCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pixelDebugCheckBox.AutoSize = true;
            this.pixelDebugCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pixelDebugCheckBox.Enabled = false;
            this.pixelDebugCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.pixelDebugCheckBox.Location = new System.Drawing.Point(3, 311);
            this.pixelDebugCheckBox.Name = "pixelDebugCheckBox";
            this.pixelDebugCheckBox.Size = new System.Drawing.Size(84, 16);
            this.pixelDebugCheckBox.TabIndex = 6;
            this.pixelDebugCheckBox.Text = "单像素调试";
            this.pixelDebugCheckBox.UseVisualStyleBackColor = true;
#endif
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
            this.ClientSize = new System.Drawing.Size(767, 566);
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
        private System.Windows.Forms.Panel settingPanel;
        private System.Windows.Forms.Button renderButton;
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
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
#if DEBUG
        private System.Windows.Forms.CheckBox pixelDebugCheckBox;
#endif
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openTaskQueueMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHDRToolStripMenuItem;
        private System.Windows.Forms.TextBox exposureInputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox renderChannelCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox tonemappingCheckBox;
        private System.Windows.Forms.Button retonemappingButton;
    }
}

