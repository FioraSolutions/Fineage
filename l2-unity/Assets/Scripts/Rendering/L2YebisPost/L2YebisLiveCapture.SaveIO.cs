using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public static partial class L2YebisLiveCapture
{





    struct Band
    {
        public float[] R;
        public float[] G;
        public float[] B;
        public int Count;

        public static Band Create(int capacity)
        {
            return new Band
            {
                R = new float[capacity],
                G = new float[capacity],
                B = new float[capacity],
                Count = 0
            };
        }

        public void Add(float r, float g, float b)
        {
            if (Count >= R.Length)
            {
                return;
            }
            R[Count] = r;
            G[Count] = g;
            B[Count] = b;
            Count++;
        }

        public string MedianText()
        {
            return Median(R).ToString("0.####", CultureInfo.InvariantCulture) + " " +
                   Median(G).ToString("0.####", CultureInfo.InvariantCulture) + " " +
                   Median(B).ToString("0.####", CultureInfo.InvariantCulture);
        }

        float Median(float[] values)
        {
            if (Count <= 0)
            {
                return 0f;
            }
            var copy = new float[Count];
            Array.Copy(values, copy, Count);
            Array.Sort(copy);
            return copy[Count / 2];
        }
    }
}
