/* Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;

namespace ColorPicker
{
    internal static class NumberHelpers
    {
        private static TValueType ClampInternal<TValueType>(TValueType value, TValueType min, TValueType max)
            where TValueType : IComparable
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException("min must be lower than max", "min");
            }

            if (value.CompareTo(min) < 0)
            {
                return min;
            }

            if (value.CompareTo(max) > 0)
            {
                return max;
            }

            return value;
        }

        public static byte Clamp(this byte value, byte min, byte max)
        {
            return ClampInternal(value, min, max);
        }

        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            return ClampInternal(value, min, max);
        }

        public static UInt16 Clamp(this UInt16 value, UInt16 min, UInt16 max)
        {
            return ClampInternal(value, min, max);
        }

        public static Int16 Clamp(this Int16 value, Int16 min, Int16 max)
        {
            return ClampInternal(value, min, max);
        }

        public static UInt32 Clamp(this UInt32 value, UInt32 min, UInt32 max)
        {
            return ClampInternal(value, min, max);
        }

        public static Int32 Clamp(this Int32 value, Int32 min, Int32 max)
        {
            return ClampInternal(value, min, max);
        }

        public static byte ClampToByte(this Int32 value)
        {
            return (byte)ClampInternal(value, 0, 255);
        }

        public static UInt64 Clamp(this UInt64 value, UInt64 min, UInt64 max)
        {
            return ClampInternal(value, min, max);
        }

        public static Int64 Clamp(this Int64 value, Int64 min, Int64 max)
        {
            return ClampInternal(value, min, max);
        }

        public static Single Clamp(this Single value, Single min, Single max)
        {
            return ClampInternal(value, min, max);
        }

        public static byte ClampToByte(this Single value)
        {
            return (byte)ClampInternal(value, 0, 255);
        }

        public static Double Clamp(this Double value, Double min, Double max)
        {
            return ClampInternal(value, min, max);
        }

        public static byte ClampToByte(this Double value)
        {
            return (byte)ClampInternal(value, 0, 255);
        }

        public static float Lerp(this float from, float to, float frac)
        {
            return (from + frac * (to - from));
        }

        public static double Lerp(this double from, double to, double frac)
        {
            return (from + frac * (to - from));
        }
    }
}
