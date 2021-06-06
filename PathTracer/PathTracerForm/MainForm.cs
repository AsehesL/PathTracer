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
        private Bitmap m_Bitmap;
        private IRenderResult m_Result;

        public MainForm()
        {
            InitializeComponent();

            Log.Init();

            Log.onAddLogEvent += OnAddLog;
            Log.onClearLogEvent += OnClearLog;
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
            this.openFileDialog.Filter = "场景文件|*.scene";
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
            int defaultWidth, defaultHeight;
            Log.Clear();
            Log.Info("载入场景:"+fileName);
            m_Scene = Scene.Create(fileName, out defaultWidth, out defaultHeight);
            if (m_Scene == null)
            {
                Log.Err("载入场景异常");
                this.bounceInputBox.Enabled = false;
                this.samplerTypeCombo.Enabled = false;
                this.numSampleInputBox.Enabled = false;
                this.widthInputBox.Enabled = false;
                this.heightInputBox.Enabled = false;
                //this.fastPreviewButton.Enabled = false;
                this.renderChannelCombo.Enabled = false;
                this.renderButton.Enabled = false;
                this.saveToolStripMenuItem.Enabled = false;
                this.tonemappingCheckBox.Enabled = false;
#if DEBUG
                this.pixelDebugCheckBox.Enabled = false;
#endif

                this.fileNameLabel.Text = "当前场景（空）";
            }
            else
            {
                Log.Info("场景载入成功");
                if (defaultWidth > 0)
                    this.widthInputBox.Text = defaultWidth.ToString();
                if (defaultHeight > 0)
                    this.heightInputBox.Text = defaultHeight.ToString();
                this.bounceInputBox.Enabled = true;
                this.samplerTypeCombo.Enabled = true;
                this.numSampleInputBox.Enabled = true;
                this.widthInputBox.Enabled = true;
                this.heightInputBox.Enabled = true;
                //this.fastPreviewButton.Enabled = true;
                this.renderChannelCombo.Enabled = true;
                this.renderButton.Enabled = true;
                this.saveToolStripMenuItem.Enabled = true;
                this.tonemappingCheckBox.Enabled = true;
#if DEBUG
                this.pixelDebugCheckBox.Enabled = true;
#endif

                var tpnames = System.Enum.GetNames(typeof(SamplerType));
                this.samplerTypeCombo.Items.Clear();
                foreach (var it in tpnames)
                    this.samplerTypeCombo.Items.Add(it);
                this.samplerTypeCombo.SelectedIndex = 0;
                //this.samplerTypeCombo.

                var chnames = System.Enum.GetNames(typeof(RenderChannel));
                this.renderChannelCombo.Items.Clear();
                foreach (var it in chnames)
                    this.renderChannelCombo.Items.Add(it);
                this.renderChannelCombo.SelectedIndex = 0;

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

        private void exposureInputBox_KeyPress(object sender, KeyPressEventArgs e)
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

        //private void fastPreviewButton_Click(object sender, EventArgs e)
        //{
        //    if (m_Scene == null)
        //        return;
        //    uint width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : uint.Parse(this.widthInputBox.Text);
        //    uint height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : uint.Parse(this.heightInputBox.Text);
        //    if (width <= 0 || height <= 0)
        //    {
        //        MessageBox.Show("请输入宽高合法的宽高!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //    Log.Info("开始快速预览");

        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    this.progressBar.Value = 0;
        //    this.progressBar.Maximum = 100;
        //    m_Result = m_Scene.FastRender(width, height, this.ProgressCallBack);
        //    stopWatch.Stop();

        //    if (m_Result != null)
        //    {
        //        Log.CompleteInfo($"渲染完成，总计用时:{stopWatch.ElapsedMilliseconds}");

        //        if (m_Bitmap != null)
        //        {
        //            m_Bitmap.Dispose();
        //            m_Bitmap = null;
        //        }

        //        m_Bitmap = m_Result.TransferToBMP(m_Bitmap, 0.45f);
        //        this.renderResultBox.BackgroundImage = m_Bitmap;

        //        this.progressBar.Value = 0;
        //    }
        //    else
        //        Log.Err("渲染预览!");
        //}

        private void renderButton_Click(object sender, EventArgs e)
        {
            if (m_Scene == null)
                return;
            int traceTimes = string.IsNullOrEmpty(this.bounceInputBox.Text)?0: int.Parse(this.bounceInputBox.Text);
            int numSamples = string.IsNullOrEmpty(this.numSampleInputBox.Text) ? 0 : int.Parse(this.numSampleInputBox.Text);
            var sampleType = (SamplerType) this.samplerTypeCombo.SelectedIndex;
            var renderChannel = (RenderChannel)this.renderChannelCombo.SelectedIndex;
            uint width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : uint.Parse(this.widthInputBox.Text);
            uint height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : uint.Parse(this.heightInputBox.Text);

            float exposure = -1.0f;
            if(this.tonemappingCheckBox.Checked && !string.IsNullOrEmpty(this.exposureInputBox.Text))
            {
                exposure = float.Parse(this.exposureInputBox.Text);
            }

            //if (renderChannel == RenderChannel.Full && traceTimes <= 0)
            //{
            //    MessageBox.Show("不允许反弹次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            if (traceTimes < 0)
                traceTimes = 0;

            //if (renderChannel == RenderChannel.Full && numSamples <= 0)
            //{
            //    MessageBox.Show("不允许采样次数小于等于0!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            if (numSamples <= 0)
                numSamples = 1;

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
            var pt = new ASL.PathTracer.PathTracer(m_Scene);
            //var pt = new ASL.PathTracer.VolumeTextureRenderer(m_Scene);
            RenderConfig config = new RenderConfig()
            {
                traceTimes = traceTimes,
                samplerType = sampleType,
                numSamples = numSamples,
                numSets = 83,
                width = width,
                height = height,
            };

            m_Result = pt.Render(config, this.ProgressCallBack);
            stopWatch.Stop();

            if (m_Result != null)
            {
                Log.CompleteInfo($"渲染完成，总计用时:{stopWatch.ElapsedMilliseconds}");

                if (m_Result is Texture)
                {
                    if (m_Bitmap != null)
                    {
                        m_Bitmap.Dispose();
                        m_Bitmap = null;
                    }

                    var tex = m_Result as Texture;
                    m_Bitmap = tex.TransferToBMP(m_Bitmap, 0.45f, exposure);
                    this.renderResultBox.BackgroundImage = m_Bitmap;
                    if (this.tonemappingCheckBox.Checked)
                        this.retonemappingButton.Enabled = true;
                }
                else
                {
                    this.renderResultBox.BackgroundImage = null;
                    this.retonemappingButton.Enabled = false;
                }

                this.progressBar.Value = 0;
            }
            else
                Log.Err("渲染失败!");
        }

        private void renderResultBox_MouseClick(object sender, MouseEventArgs e)
        {
            #if DEBUG
            if (this.pixelDebugCheckBox.Checked)
            {
                if (MessageBox.Show("开启单像素调试后点击画布将对单个像素渲染，这会清空之前的渲染结果，是否继续？", "警告", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    int traceTimes = string.IsNullOrEmpty(this.bounceInputBox.Text) ? 0 : int.Parse(this.bounceInputBox.Text);
                    int numSamples = string.IsNullOrEmpty(this.numSampleInputBox.Text) ? 0 : int.Parse(this.numSampleInputBox.Text);
                    var sampleType = (SamplerType)this.samplerTypeCombo.SelectedIndex;
                    uint width = string.IsNullOrEmpty(this.widthInputBox.Text) ? 0 : uint.Parse(this.widthInputBox.Text);
                    uint height = string.IsNullOrEmpty(this.heightInputBox.Text) ? 0 : uint.Parse(this.heightInputBox.Text);
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

                    var pt = new ASL.PathTracer.PathTracer(m_Scene);
                    RenderConfig config = new RenderConfig()
                    {
                        traceTimes = traceTimes,
                        samplerType = sampleType,
                        numSamples = numSamples,
                        numSets = 83,
                        width = width,
                        height = height,
                    };
                    m_Result = pt.RenderDebugSinglePixel(x, y, config);

                    if (m_Result != null)
                    {
                        Log.CompleteInfo("渲染完成");

                        //this.renderResultBox.BackgroundImage = result.SaveToImage();
                    }
                    else
                        Log.Err("渲染失败!");
                }
            }
            #endif
        }

        private void ProgressCallBack(int progress, int total)
        {
            float p = ((float)progress) / total;
            int percent = (int)(p * 100.0f);
            this.progressBar.Value = percent;

            ////if (renderJobResult != null && renderJobResult is PathTracerRenderJobResult)
            ////{
            ////    var r = (PathTracerRenderJobResult)renderJobResult;
            ////    for(int i=0;i<r.tileWidth;i++)
            ////    {
            ////        for(int j=0;j<r.tileHeight;j++)
            ////        {
            ////            ASL.PathTracer.Color col = r.GetPixel(i, j);
            ////            col.Gamma(0.45f);
            ////            col.ClampColor();
            ////            System.Drawing.Color c = System.Drawing.Color.FromArgb((int)(col.r * 255.0f),
            ////                (int)(col.g * 255.0f), (int)(col.b * 255.0f));
            ////            m_Bitmap.SetPixel((int)m_Bitmap.Width - 1 - (i + r.x), (int)m_Bitmap.Height - 1 - (j + r.y), c);
            ////        }
            ////    }
            ////}
            //this.renderResultBox.BackgroundImage = m_Bitmap;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_Result == null)
                return;
            this.saveFileDialog.Filter = m_Result.GetExtensions();
            DialogResult result = this.saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(this.saveFileDialog.FileName))
                {
                    return;
                }

                m_Result.Save(this.saveFileDialog.FileName);
            }
        }

        private void OpenTaskQueueMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog.Filter = "渲染队列文件|*.queue";
            DialogResult result = this.openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(this.openFileDialog.FileName))
                {
                    return;
                }

                var queue = ASL.PathTracer.TaskQueue.Load(this.openFileDialog.FileName);
                
                bool success = queue != null ? queue.Execute(this.ProgressCallBack) : false;

                if (success)
                {
                    Log.CompleteInfo($"渲染完成!");

                    this.progressBar.Value = 0;
                }
                else
                    Log.Err("渲染失败!");
            }
        }

        private void TonemappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.exposureInputBox.Enabled = this.tonemappingCheckBox.Checked;
            if (this.renderResultBox.BackgroundImage == null)
                return;
            this.retonemappingButton.Enabled = this.tonemappingCheckBox.Checked;
        }

        private void retonemappingButton_Click(object sender, EventArgs e)
        {
            if (m_Result != null)
            {
                if (m_Bitmap != null)
                {
                    m_Bitmap.Dispose();
                    m_Bitmap = null;
                }

                float exposure = -1.0f;
                if (this.tonemappingCheckBox.Checked && !string.IsNullOrEmpty(this.exposureInputBox.Text))
                {
                    exposure = float.Parse(this.exposureInputBox.Text);
                }

                if (m_Result is Texture)
                {
                    var tex = m_Result as Texture;
                    m_Bitmap = tex.TransferToBMP(m_Bitmap, 0.45f, exposure);
                    this.renderResultBox.BackgroundImage = m_Bitmap;
                }
                else
                {
                    this.renderResultBox.BackgroundImage = null;
                }
            }
        }

        private void OnAddLog(Log.LogItem logItem)
        {
            ListViewItem item = new ListViewItem();
            item.Text = logItem.logType.ToString();
            item.SubItems.Add(logItem.message);
            ASL.PathTracer.Color foreCol = logItem.GetFontColor();
            ASL.PathTracer.Color backCol = logItem.GetBackColor();
            int foreColARGB = foreCol.ToARGB();
            int backColARGB = backCol.ToARGB();
            item.ForeColor = System.Drawing.Color.FromArgb(foreColARGB);
            item.BackColor = System.Drawing.Color.FromArgb(backColARGB);

            this.logListView.Items.Add(item);
        }

        private void OnClearLog()
        {
            this.logListView.Items.Clear();
        }
    }
}
