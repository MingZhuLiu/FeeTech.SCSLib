using System;
using System.Threading;
using FeeTech.SCSLib;

namespace SCSTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FeeTech.SCSLib.FeeTechClient.ShowDebugInfo = true;
            var initClientFlag = FeeTech.SCSLib.FeeTechClient.InitClient("/dev/tty.wchusbserial14140", 115200);
            if (initClientFlag == true)
            {
                Console.WriteLine("初始化成功!");
            }
            else
            {
                Console.WriteLine("初始化失败!");
                return;
            }

            //扫描设备列表
            FeeTech.SCSLib.FeeTechClient.Instance.ScanDevices();

            Thread.Sleep(1000);
            Console.WriteLine("设备列表:");
            Console.WriteLine();
            var devices = FeeTech.SCSLib.FeeTechClient.Instance.Devices;
            foreach (var item in devices)
            {
                Console.WriteLine("Id:" + item.Id);
                Console.WriteLine("错误状态:");
                Console.WriteLine("       是否过载:" + (item.IsOverLoad ? "是" : "否"));
                Console.WriteLine("       是否过热:" + (item.IsOverHot ? "是" : "否"));
                Console.WriteLine("       是否过压:" + (item.IsOverVoltage ? "是" : "否"));

            }


            // FeeTech.SCSLib.FeeTechClient.Instance.SendData(0xFE, 0x01);
            // FeeTech.SCSLib.FeeTechClient.Instance.SendData(0xFE, 0x01);
            FeeTech.SCSLib.FeeTechClient.Instance.SendData(0x01, 0x03, new byte[] { 0x28, 0x01 });

            Thread.Sleep(1000);




            while (true)
            {
                Console.ReadKey();

            }




        }
    }
}
