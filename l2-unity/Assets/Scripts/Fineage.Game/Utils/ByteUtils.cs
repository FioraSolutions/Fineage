using System;
using System.Collections.Generic;

public partial class ByteUtils
{








 

        public static int CombineFlags(params int[] flags)
        {
            if (flags == null) throw new ArgumentNullException(nameof(flags));
            int mask = 0;
            foreach (var f in flags)
            {
                if (f <= 0 || (f & (f - 1)) != 0)
                    throw new ArgumentException($"Значение {f} не является степенью двойки.", nameof(flags));
                mask |= f;
            }
            return mask;
        }

    
        public static long CombineFlags(params long[] flags)
        {
            if (flags == null) throw new ArgumentNullException(nameof(flags));
            long mask = 0;
            foreach (var f in flags)
            {
                if (f <= 0 || (f & (f - 1)) != 0)
                    throw new ArgumentException($"Значение {f} не является степенью двойки.", nameof(flags));
                mask |= f;
            }
            return mask;
        }






}
