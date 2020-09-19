using System.Collections.Generic;

namespace FeeTech.SCSLib
{
    public class SteeringEngine
    {
        public SteeringEngine()
        {
            MemoryData = new Dictionary<int, byte[]>();
        }
        /// <summary>
        /// Id 舵机Id
        /// </summary>
        /// <value></value>
        public int Id { get; set; }

        /// <summary>
        /// Error  接收到的错误数据
        /// </summary>
        /// <value></value>
        public byte Error { get; set; }

        /// <summary>
        /// IsOverLoad 是否过载
        /// </summary>
        public bool IsOverLoad => Error >> 5 == 0x00 ? false : true;

        /// <summary>
        /// IsOverHot 是否过热
        /// </summary>
        public bool IsOverHot => Error >> 2 == 0x00 ? false : true;

        /// <summary>
        /// IsOverVoltage  是否过压欠压
        /// </summary>
        public bool IsOverVoltage => Error >> 0 == 0x00 ? false : true;



        public Dictionary<int, byte[]> MemoryData { get; set; }

        public void SetReceiverData(List<byte> buffer)
        {
            
        }


    }

}