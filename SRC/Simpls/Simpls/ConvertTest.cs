using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Simpls
{
    public class ConvertTest
    {

        static byte[] ConvertToByteArray(ushort value)
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                bytes[i] = (byte)((value >> i) & 1);
            }
            return bytes;
        }

        static bool[] ConvertToBoolArray(byte[] byteArray)
        {
            bool[] boolArray = new bool[byteArray.Length];
            for (int i = 0; i < byteArray.Length; i++)
            {
                boolArray[i] = byteArray[i] == 1;
            }
            return boolArray;
        }


        [Test]
        public void UShortToBoolArray()
        {
            var array = ConvertToByteArray(15);
        }

    }
}
