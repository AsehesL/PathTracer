using ASL.PathTracer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

            Log.Init(this.logListView);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.fileNameLabel.Parent = this.renderResultBox;
            this.infoLabel.Parent = this.renderResultBox;

            var args = System.Environment.GetCommandLineArgs();
            if (args != null && args.Length >= 2)
            {
                GenerateScene(args[1]);
            }
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
            Log.Clear();
            Log.Info("载入场景:"+fileName);
            m_Scene = Scene.Create(fileName);
            if (m_Scene == null)
            {
                Log.Err("载入场景异常");
                this.bounceInputBox.Enabled = false;
                this.samplerTypeCombo.Enabled = false;
                this.numSampleInputBox.Enabled = false;
                this.widthInputBox.Enabled = false;
                this.heightInputBox.Enabled = false;
                this.fastPreviewButton.Enabled = false;
                this.renderButton.Enabled = false;
                this.SaveToolStripMenuItem.Enabled = false;
                this.pixelDebugCheckBox.Enabled = false;

                this.fileNameLabel.Text = "当前场景（空）";
            }
            else
            {
                Log.Info("场景载入成功");
                this.bounceInputBox.Enabled = true;
                this.samplerTypeCombo.Enabled = true;
                this.numSampleInputBox.Enabled = true;
                this.widthInputBox.Enabled = true;
                this.heightInputBox.Enabled = true;
                this.fastPreviewButton.Enabled = true;
                this.renderButton.Enabled = true;
                this.SaveToolStripMenuItem.Enabled = true;
                this.pixelDebugCheckBox.Enabled = true;

                var tpnames = System.Enum.GetNames(typeof(SamplerType));
                this.samplerTypeCombo.Items.Clear();
                foreach (var it in tpnames)
                    this.samplerTypeCombo.Items.Add(it);
                this.samplerTypeCombo.SelectedIndex = 0;
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

        private void fastPreviewButton_Click(object sender, EventArgs e)
        {
            if (m_Scene == null)
                return;
            int width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : int.Parse(this.widthInputBox.Text);
            int height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : int.Parse(this.heightInputBox.Text);
            if (width <= 0 || height <= 0)
            {
                MessageBox.Show("请输入宽高合法的宽高!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Log.Info("开始快速预览");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.progressBar.Value = 0;
            this.progressBar.Maximum = 100;
            var result = m_Scene.FastRender(width, height, this.ProgressCallBack);
            stopWatch.Stop();

            if (result != null)
            {
                Log.CompleteInfo($"渲染完成，总计用时:{stopWatch.ElapsedMilliseconds}");

                this.renderResultBox.BackgroundImage = result.SaveToImage();

                this.progressBar.Value = 0;
            }
            else
                Log.Err("渲染预览!");
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            if (m_Scene == null)
                return;
            int traceTimes = string.IsNullOrEmpty(this.bounceInputBox.Text)?0: int.Parse(this.bounceInputBox.Text);
            int numSamples = string.IsNullOrEmpty(this.numSampleInputBox.Text) ? 0 : int.Parse(this.numSampleInputBox.Text);
            var sampleType = (SamplerType) this.samplerTypeCombo.SelectedIndex;
            int width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : int.Parse(this.widthInputBox.Text);
            int height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : int.Parse(this.heightInputBox.Text);

            if (traceTimes <= 0)
            {
                MessageBox.Show("不允许反弹次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numSamples <= 0)
            {
                MessageBox.Show("不允许采样次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (width <= 0 || height <= 0)
            {
                MessageBox.Show("请输入宽高合法的宽高!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Log.Info("开始渲染");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.progressBar.Value = 0;
            this.progressBar.Maximum = 100;
            var result = m_Scene.Render(traceTimes, sampleType, numSamples, width, height, 83, this.ProgressCallBack);
            stopWatch.Stop();

            if (result != null)
            {
                Log.CompleteInfo($"渲染完成，总计用时:{stopWatch.ElapsedMilliseconds}");

                this.renderResultBox.BackgroundImage = result.SaveToImage();

                this.progressBar.Value = 0;
            }
            else
                Log.Err("渲染失败!");
        }

        private void renderResultBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.pixelDebugCheckBox.Checked)
            {
                if (MessageBox.Show("开启单像素调试后点击画布将对单个像素渲染，这会清空之前的渲染结果，是否继续？", "警告", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    int traceTimes = string.IsNullOrEmpty(this.bounceInputBox.Text) ? 0 : int.Parse(this.bounceInputBox.Text);
                    int numSamples = string.IsNullOrEmpty(this.numSampleInputBox.Text) ? 0 : int.Parse(this.numSampleInputBox.Text);
                    var sampleType = (SamplerType)this.samplerTypeCombo.SelectedIndex;
                    int width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : int.Parse(this.widthInputBox.Text);
                    int height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : int.Parse(this.heightInputBox.Text);
                    if (traceTimes <= 0)
                    {
                        MessageBox.Show("不允许反弹次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (numSamples <= 0)
                    {
                        MessageBox.Show("不允许采样次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (width <= 0 || height <= 0)
                    {
                        MessageBox.Show("请输入宽高合法的宽高!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int w = ((PictureBox) sender).Width;
                    int h = ((PictureBox) sender).Height;

                    Log.Info($"点击了坐标:({w - 1 - e.Location.X},{h - 1 - e.Location.Y})，开始单像素渲染");

                    int x = (int) (((float) (w - 1 -e.Location.X)) / w * width);
                    int y = (int)(((float) (h - 1 - e.Location.Y)) / h * height);

                    Log.Info($"渲染目标像素:({x},{y})");

                    var result = m_Scene.RenderSinglePixel(x, y, traceTimes, sampleType, numSamples, width, height);

                    if (result != null)
                    {
                        Log.CompleteInfo("渲染完成");

                        //this.renderResultBox.BackgroundImage = result.SaveToImage();
                    }
                    else
                        Log.Err("渲染失败!");
                }
            }

        }

        private void ProgressCallBack(int progress, int total)
        {
            float p = ((float)progress) / total;
            int percent = (int)(p * 100.0f);
            this.progressBar.Value = percent;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.renderResultBox.BackgroundImage == null)
                return;
            DialogResult result = this.saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(this.saveFileDialog.FileName))
                {
                    return;
                }

                FileStream stream = new FileStream(this.saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                this.renderResultBox.BackgroundImage.Save(stream, ImageFormat.Bmp);

                stream.Close();
            }
        }
    }
}
