using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public enum SamplerType
    {
        /// <summary>
        /// 随机采样
        /// </summary>
        Random,
        /// <summary>
        /// 抖动采样
        /// </summary>
        Jittered,
        /// <summary>
        /// Hammersley采样
        /// </summary>
        Hammersley,
        /// <summary>
        /// 规则采样
        /// </summary>
        Regular,

        NRooks,
    }

    static class SamplerFactory
    {
        public static SamplerBase Create(SamplerType samplerType, int numSamples, int numSets = 83)
        {
            switch (samplerType)
            {
                case SamplerType.Hammersley:
                    return new HammersleySampler(numSamples, numSets);
                case SamplerType.Random:
                    return new RandomSampler(numSamples, numSets);
                case SamplerType.Regular:
                    return new RegularSampler(numSamples, numSets);
                case SamplerType.Jittered:
                    return new JitteredSampler(numSamples, numSets);
                case SamplerType.NRooks:
                    return new NRooksSampler(numSamples, numSets);
                default:
                    return new RegularSampler(numSamples, numSets);
            }
        }
    }

    public abstract class SamplerBase
    {
        /// <summary>
        /// 采样点总数
        /// </summary>
        public int numSamples
        {
            get { return m_NumSamples; }
        }

        /// <summary>
        /// 当前采样
        /// </summary>
        public int currentSample { get; private set; }

        protected int m_NumSamples;
        protected int m_NumSets;
        private int m_Index = 0;
        private int m_Jump = 0;

        private int[] m_ShuffledIndices;

        protected Vector2[] m_Samples;

        protected System.Random sRandom = new System.Random();


        public SamplerBase(int numSamples, int numSets = 83)
        {
            InitSampler(numSamples, numSets);

            m_ShuffledIndices = new int[m_NumSets * m_NumSamples];

            SetupShuffledIndices();
        }

        public bool NextSample()
        {
            if(currentSample < numSamples)
            {
                currentSample++;
                return true;
            }
            return false;
        }

        public void ResetSampler()
        {
            currentSample = 0;
        }

        public double GetRandom()
        {
	        return sRandom.NextDouble();
        }

        /// <summary>
        /// 半球映射
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        //public Vector3 SampleHemiSphere()
        //{
        //    Vector2 sample = SampleUnitSquare();

        //    double cos_phi = Math.Cos(2.0f * Math.PI * sample.x);
        //    double sin_phi = Math.Sin(2.0f * Math.PI * sample.x);
        //    double cos_theta = 1.0f - sample.y;
        //    double sin_theta = Math.Sqrt(1.0f - cos_theta * cos_theta);
        //    double pu = sin_theta * cos_phi;
        //    double pv = sin_theta * sin_phi;
        //    double pw = cos_theta;

        //    return new Vector3(pu, pv, pw);
        //}

        public Vector3 SampleHemiSphere(float roughness)
        {
            Vector2 sample = SampleUnitSquare();

            float a = roughness * roughness;

            double phi = 2.0f * Math.PI * sample.x;
            double cos_theta = Math.Sqrt((1.0 - sample.y) / (1.0 + (a * a - 1.0) * sample.y));
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
        }

        public Vector3 SampleHemiSphere()
        {
            Vector2 sample = SampleUnitSquare();

            double phi = 2.0f * Math.PI * sample.x;
            double cos_theta = Math.Sqrt(1.0 - sample.y);
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
        }

        /// <summary>
        /// 球形映射
        /// </summary>
        /// <returns></returns>
        public Vector3 SampleSphere()
        {
            Vector2 sample = SampleUnitSquare();

            double x = Math.Cos(2.0f * Math.PI * sample.x) * 2.0f * Math.Sqrt(sample.y * (1 - sample.y));
            double y = Math.Sin(2.0f * Math.PI * sample.x) * 2.0f * Math.Sqrt(sample.y * (1 - sample.y));
            double z = 1.0f - 2.0f * sample.y;

            return new Vector3(x, y, z).normalized;
        }

        /// <summary>
        /// 单位圆形映射
        /// </summary>
        /// <returns></returns>
        public Vector2 SampleUnitDisk()
        {
            Vector2 sample = SampleUnitSquare();
            sample.x = 2.0 * sample.x - 1.0;
            sample.y = 2.0 * sample.y - 1.0;
            double r = 0.0;
            double phi = 0.0;
            if(sample.x > -sample.y)
            {
                if(sample.x > sample.y)
                {
                    r = sample.x;
                    phi = sample.y / sample.x;
                }
                else
                {
                    r = sample.y;
                    phi = 2.0 - sample.x / sample.y;
                }
            }
            else
            {
                if(sample.x < sample.y)
                {
                    r = -sample.x;
                    phi = 4.0 + sample.y / sample.x;
                }
                else
                {
                    r = -sample.y;
                    if(sample.y < -double.Epsilon || sample.y > double.Epsilon)
                    {
                        phi = 6.0 - sample.x / sample.y;
                    }
                    else
                    {
                        phi = 0.0;
                    }
                }
            }
            phi *= Math.PI * 0.25;
            return new Vector2(r * Math.Cos(phi), r * Math.Sin(phi));
        }

        protected abstract void InitSampler(int numSamples, int numSets);

        public Vector2 SampleUnitSquare()
        {
            if (m_NumSamples == 1)
                return new Vector2(0.5, 0.5);
            if ((int) (m_Index % m_NumSamples) == 0)
            {
                m_Jump = sRandom.Next(0, m_NumSets) * m_NumSamples;
            }

            Vector2 sp = m_Samples[m_Jump + m_ShuffledIndices[m_Jump + m_Index % m_NumSamples]];
            m_Index += 1;
            return sp;
        }

        private void SetupShuffledIndices()
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < m_NumSamples; i++)
                indices.Add(i);

            m_ShuffledIndices = new int[m_NumSamples * m_NumSets];
            for (int i = 0; i < m_NumSets; i++)
            {
                Shuffle(indices);
                for (int j = 0; j < m_NumSamples; j++)
                {
                    m_ShuffledIndices[i * m_NumSamples + j] = indices[j];
                }
            }
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = sRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    class RandomSampler : SamplerBase
    {
        public RandomSampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
        }

        protected override void InitSampler(int numSamples, int numSets)
        {
            m_NumSamples = numSamples;
            m_NumSets = numSets;
            m_Samples = new Vector2[m_NumSets * m_NumSamples];

            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < numSamples; j++)
                {
                    m_Samples[i * numSamples + j] =
                        new Vector2((float)sRandom.NextDouble(), (float)sRandom.NextDouble());
                }
            }
        }
    }

    class JitteredSampler : SamplerBase
    {
        public JitteredSampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
        }

        protected override void InitSampler(int numSamples, int numSets)
        {
            int n = (int)Math.Sqrt(numSamples);
            m_NumSamples = n * n;
            m_NumSets = numSets;
            m_Samples = new Vector2[m_NumSets * m_NumSamples];
            int index = 0;
            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        Vector2 sp = new Vector2((k + (float)sRandom.NextDouble()) / n, (j + (float)sRandom.NextDouble()) / n);
                        m_Samples[index] = sp;
                        index += 1;
                    }
                }
            }
        }
    }

    class HammersleySampler : SamplerBase
    {
        public HammersleySampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
        }

        protected override void InitSampler(int numSamples, int numSets)
        {
            m_NumSamples = numSamples;
            m_NumSets = numSets;
            m_Samples = new Vector2[m_NumSets * m_NumSamples];
            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < numSamples; j++)
                {
                    m_Samples[i * numSamples + j] = Hammersley(j, i * numSamples + j, numSamples);
                }
            }
        }

        float RadicalInverse_VdC(ulong bits)
        {
            bits = (bits << 16) | (bits >> 16);
            bits = ((bits & 0x55555555u) << 1) | ((bits & 0xAAAAAAAAu) >> 1);
            bits = ((bits & 0x33333333u) << 2) | ((bits & 0xCCCCCCCCu) >> 2);
            bits = ((bits & 0x0F0F0F0Fu) << 4) | ((bits & 0xF0F0F0F0u) >> 4);
            bits = ((bits & 0x00FF00FFu) << 8) | ((bits & 0xFF00FF00u) >> 8);
            return (float)bits * 2.3283064365386963e-10f;
        }

        Vector2 Hammersley(int i, int j, int N)
        {
            return new Vector2((float)i / (float)N, RadicalInverse_VdC((ulong)j));
        }

        //private float Phi(int j)
        //{
        //    float x = 0.0f;
        //    float f = 0.5f;
        //    while (((int)j) > 0)
        //    {
        //        x += f * (double)((j<=0) & 1);
        //        j = j / 2;
        //        f *= 0.5f;
        //    }

        //    return x;
        //}
    }

    class RegularSampler : SamplerBase
    {
        public RegularSampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
        }

        protected override void InitSampler(int numSamples, int numSets)
        {
            int n = (int)Math.Sqrt(numSamples);
            m_NumSamples = n * n;
            m_NumSets = numSets;
            m_Samples = new Vector2[m_NumSets * m_NumSamples];
            int index = 0;
            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        m_Samples[index] =
                            new Vector2((0.5f + k) / n, (0.5f + j) / n);
                        index += 1;
                    }
                }
            }
        }
    }

    class NRooksSampler : SamplerBase
    {
        public NRooksSampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
        }

        protected override void InitSampler(int numSamples, int numSets)
        {
            m_NumSamples = numSamples;
            m_NumSets = numSets;
            m_Samples = new Vector2[m_NumSets * m_NumSamples];
            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < numSamples; j++)
                {
                    Vector2 sp = new Vector2((j + (float)sRandom.NextDouble()) / numSamples, (j + (float)sRandom.NextDouble()) / numSamples);
                    m_Samples[i * numSamples + j] = sp;
                }
            }

            ShuffleXCoordinates();
            ShuffleYCoordinates();
        }

        private void ShuffleXCoordinates()
        {
            for (int i = 0; i < m_NumSets; i++)
            {
                for (int j = 0; j < m_NumSamples-1; j++)
                {
                    int target = sRandom.Next() % m_NumSamples + i * m_NumSamples;
                    double temp = m_Samples[j + i * m_NumSamples + 1].x;
                    m_Samples[j + i * m_NumSamples + 1].x = m_Samples[target].x;
                    m_Samples[target].x = temp;
                }
            }
        }

        private void ShuffleYCoordinates()
        {
            for (int i = 0; i < m_NumSets; i++)
            {
                for (int j = 0; j < m_NumSamples - 1; j++)
                {
                    int target = sRandom.Next() % m_NumSamples + i * m_NumSamples;
                    double temp = m_Samples[j + i * m_NumSamples + 1].y;
                    m_Samples[j + i * m_NumSamples + 1].y = m_Samples[target].y;
                    m_Samples[target].y = temp;
                }
            }
        }
    }
}
