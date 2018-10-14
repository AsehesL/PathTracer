using ASL.PathTracer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathTracerForm
{
    public partial class MainForm : Form
    {
        private Scene m_Scene;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.fileNameLabel.Parent = this.renderResultBox;
            this.infoLabel.Parent = this.renderResultBox;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(this.openFileDialog.FileName))
                {
                    return;
                }

                GenerateScene(this.openFileDialog.FileName);
            }
        }


        private void GenerateScene(string fileName)
        {
            m_Scene = Scene.Create(fileName);
            if (m_Scene == null)
            {
                this.multiThreadCheckBox.Enabled = false;
                this.bounceInputBox.Enabled = false;
                this.samplerTypeCombo.Enabled = false;
                this.numSampleInputBox.Enabled = false;
                this.widthInputBox.Enabled = false;
                this.heightInputBox.Enabled = false;
                this.fastPreviewButton.Enabled = false;
                this.renderButton.Enabled = false;

                this.fileNameLabel.Text = "当前场景（空）";
            }
            else
            {

                this.multiThreadCheckBox.Enabled = true;
                this.bounceInputBox.Enabled = true;
                this.samplerTypeCombo.Enabled = true;
                this.numSampleInputBox.Enabled = true;
                this.widthInputBox.Enabled = true;
                this.heightInputBox.Enabled = true;
                this.fastPreviewButton.Enabled = true;
                this.renderButton.Enabled = true;

                var tpnames = System.Enum.GetNames(typeof(SamplerType));
                this.samplerTypeCombo.Items.Clear();
                foreach (var it in tpnames)
                    this.samplerTypeCombo.Items.Add(it);
                //this.samplerTypeCombo.

                this.fileNameLabel.Text = "当前场景:" + fileName;
            }
        }

        private void bounceInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.LimitInputNumber(sender, e);
        }

        private void numSampleInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.LimitInputNumber(sender, e);
        }

        private void widthInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.LimitInputNumber(sender, e);
        }

        private void heightInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.LimitInputNumber(sender, e);
        }

        private void LimitInputNumber(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0; 
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;  
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0; 
                }
            }
        }
    }
}
