using System;
using System.Text;

namespace FeeTech.SCSLib
{
    public static class Tools
    {


        /// <summary>
        /// 方法：获取校验
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="len">数据包长度</param>
        /// <returns></returns>
        public static byte CheckSum(byte[] data)
        {
            char sum = (char)0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += (char)data[i];
            }
            var x = (ushort)(~sum);
            var buffer = BitConverter.GetBytes(x);
            return buffer[0];
        }
    }
}