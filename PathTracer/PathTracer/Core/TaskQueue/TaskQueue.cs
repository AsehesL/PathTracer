using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ASL.PathTracer
{
    /// <summary>
    /// TaskQueue以xml的形式存放在硬盘上，主要作用是方便渲染多个文件，并自动在渲染结束后存储文件，执行指定cmd文件等
    /// 例如可以渲染多个文件，并在渲染结束后执行关机命令
    /// </summary>
    public class TaskQueue
    {
        [XmlArray(ElementName = "Tasks")]
        public RenderTask[] renderTasks;
        [XmlAttribute(AttributeName = "FinishCmd")]
        public string finishCommand;

        public static TaskQueue Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            if (!File.Exists(path))
                return null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TaskQueue));
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return serializer.Deserialize(stream) as TaskQueue;
                }
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public bool Execute(RenderJobCallBackDelegate progressCallBackAction = null)
        {
            bool result = false;
            if (renderTasks != null)
            {
                for (int i = 0; i < renderTasks.Length; i++)
                {
                    if (renderTasks[i] != null)
                    {
                        if (renderTasks[i].Execute(progressCallBackAction))
                        {
                            result = true;
                            ExecuteCommand(renderTasks[i].finishCommand);
                        }
                    }
                }

                if (result)
                {
                    ExecuteCommand(finishCommand);
                }
            }
            return result;
        }

        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = false;
            p.Start();
            p.StandardInput.WriteLine(command + "&exit");
            p.StandardInput.AutoFlush = true;
            p.WaitForExit();
            p.Close();
        }
    }

    public class RenderTask
    {
        [XmlAttribute(AttributeName = "Source")]
        public string scenePath;
        [XmlAttribute(AttributeName = "Bounce")]
        public int bounceTimes;
        [XmlAttribute(AttributeName = "Sampler")]
        public SamplerType samplerType;
        [XmlAttribute(AttributeName = "NumSamples")]
        public int numSamples;
        [XmlAttribute(AttributeName = "HDR")]
        public bool saveHDR;
        [XmlAttribute(AttributeName = "Tonemapping")]
        public bool tonemapping;
        [XmlAttribute(AttributeName = "Exposure")]
        public float exposure;
        [XmlAttribute(AttributeName = "Width")]
        public int width;
        [XmlAttribute(AttributeName = "Height")]
        public int height;
        [XmlAttribute(AttributeName = "Output")]
        public string outputPath;
        [XmlAttribute(AttributeName = "FinishCmd")]
        public string finishCommand;

        public bool Execute(RenderJobCallBackDelegate progressCallBackAction = null)
        {
            if (string.IsNullOrEmpty(scenePath))
                return false;
            if(File.Exists(scenePath) == false)
                return false;
            if (string.IsNullOrEmpty(outputPath))
                return false;
            FileInfo outputFileInfo = new FileInfo(outputPath);
            if (outputFileInfo.Directory == null)
                return false;
            if (bounceTimes <= 0)
                return false;
            if (numSamples <= 0)
                return false;
            if (width <= 0 || height <= 0)
                return false;
            var scene = Scene.Create(scenePath);
            var pt = new PathTracer(scene);
            RenderConfig config = new RenderConfig()
            {
                traceTimes = bounceTimes,
                samplerType = samplerType,
                numSamples = numSamples,
                numSets = 83,
                width = (uint)width,
                height = (uint)height,
            };
            var tex = pt.Render(config, progressCallBackAction);
            
            if (outputFileInfo.Directory.Exists)
                outputFileInfo.Directory.Create();
            if (saveHDR)
            {
                tex.Save(Path.Combine(outputFileInfo.FullName, ".hdr"));
                //tex.SaveToHDR(Path.Combine(outputFileInfo.FullName, ".hdr"));
            }
            else
            {
                //var bitmap = tex.TransferToBMP(null, 0.45f, tonemapping ? exposure : -1.0f);
                //FileStream stream = new FileStream(outputFileInfo.FullName, FileMode.Create, FileAccess.Write);
                //bitmap.Save(stream, ImageFormat.Bmp);
                //stream.Close();
                //bitmap.Dispose();
                tex.Save(outputFileInfo.FullName);
            }

            return true;
        }
    }
}