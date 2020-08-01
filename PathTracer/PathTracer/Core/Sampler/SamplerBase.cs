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
        Hammersley,
        /// <summary>
        /// 规则采样
        /// </summary>
        Regular,
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
                default:
                    return new RegularSampler(numSamples, numSets);
            }
        }
    }

    public abstract class SamplerBase
    {
        public int numSamples
        {
            get { return m_NumSamples; }
        }

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

        public double GetRandom()
        {
	        return sRandom.NextDouble();
        }

        public Vector3 SampleHemiSphere(float e)
        {
            Vector2 sample = Sample();

            float cos_phi = (float)Math.Cos(2.0f * Math.PI * sample.x);
            float sin_phi = (float)Math.Sin(2.0f * Math.PI * sample.x);
            float cos_theta = (float)Math.Pow(1.0f - sample.y, 1.0f / (e + 1.0f));
            float sin_theta = (float)Math.Sqrt(1.0f - cos_theta * cos_theta);
            float pu = sin_theta * cos_phi;
            float pv = sin_theta * sin_phi;
            float pw = cos_theta;

            return new Vector3(pu, pv, pw);
        }

        public Vector2 SampleUnitDisk()
        {
            Vector2 sample = Sample();
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

        public Vector2 Sample()
        {
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
                    m_Samples[i * numSamples + j] =
                        new Vector2(((float)j) / numSamples, Phi(j));
                }
            }
        }

        private float Phi(int j)
        {
            float x = 0.0f;
            float f = 0.5f;
            while (((int)j) > 0)
            {
                x += f * (j % 2);
                j = j / 2;
                f *= 0.5f;
            }

            return x;
        }
    }

    class RegularSampler : SamplerBase
    {
        public RegularSampler(int numSamples, int numSets = 83) : base(numSamples, numSets)
        {
            int n = (int)Math.Sqrt(numSamples);
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
}
