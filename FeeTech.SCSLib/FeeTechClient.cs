using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FeeTech.SCSLib
{



    public class FeeTechClient
    {
        #region  变量

        public static bool ShowDebugInfo { get; set; }

        private static FeeTechClient _client = null;

        /// <summary>
        /// FeeTech Servo Instance
        /// </summary>
        /// <returns></returns>
        public static FeeTechClient Instance => _client != null ? _client : throw new NullReferenceException("Please invoke Method InitService.");

        /// <summary>
        /// Check which port is being used on your controller
        /// </summary>
        /// <value>ex) Windows: "COM1"   Linux: "/dev/ttyUSB0" Mac: "/dev/tty.usbserial-*"</value>
        private string _deviceName { get; set; }

        /// <summary>
        /// SCServo default baudrate : 115200
        /// </summary>
        /// <value>SCServo default baudrate</value>
        private int _baudrate { get; set; }

        /// <summary>
        /// Read Time Out :Default 500ms
        /// </summary>
        /// <value></value>
        private int _timeOutRead { get; set; }

        /// <summary>
        /// Write Time Out :Default 500ms
        /// </summary>
        /// <value></value>
        private int _timeOutWrite { get; set; }


        /// <summary>
        /// 串行端口客户端
        /// </summary>
        /// <value></value>
        private SerialPort _serialPort { get; set; }

        /// <summary>
        /// IsOpend 是否打开
        /// </summary>
        public bool IsOpend => _serialPort == null ? false : _serialPort.IsOpen;

        /// <summary>
        /// Device List/当前在线舵机列表
        /// </summary>
        /// <value></value>
        public List<SteeringEngine> Devices { get; set; }


        #endregion

        /// <summary>
        /// Init SerialPort Controller
        /// </summary>
        /// <param name="deviceName">ex) Windows: "COM1"   Linux: "/dev/ttyUSB0" Mac: "/dev/tty.usbserial-*"</param>
        /// <param name="baudrate">SCServo default baudrate : 1000000</param>
        /// <param name="timeOutRead">Default 500ms</param>
        /// <param name="timeOutWrite">Default 500ms</param>
        /// <returns></returns>
        public static bool InitClient(string deviceName, int baudrate, int timeOutRead = 500, int timeOutWrite = 500)
        {
            if (_client != null && _client.IsOpend)
            {
                _client.Close();
            }
            _client = new FeeTechClient(deviceName, baudrate, timeOutRead, timeOutWrite);
            return _client.Open();
        }


        private FeeTechClient(String deviceName, int baudrate, int timeOutRead = 500, int timeOutWrite = 500)
        {
            this._baudrate = baudrate;
            this._deviceName = deviceName;
            this._timeOutRead = timeOutRead;
            this._timeOutWrite = timeOutWrite;

            _serialPort = new SerialPort(this._deviceName)
            {
                BaudRate = this._baudrate,
                ReadTimeout = this._timeOutRead,
                WriteTimeout = this._timeOutWrite,
                Encoding = Encoding.UTF8,
                // DataBits = 8,
                // StopBits = System.IO.Ports.StopBits.One,
                // Parity = System.IO.Ports.Parity.None,
                // Handshake = System.IO.Ports.Handshake.None,
                // RtsEnable = true
            };
            _serialPort.DataReceived += OnReceiver;
            Devices = new List<SteeringEngine>();

        }

        private void OnReceiver(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] buff = new byte[sp.ReadBufferSize];
            sp.Read(buff, 0, buff.Length);
            List<byte> receiveData = null;
            if (buff.Length >= 4 && buff[0] == 0xFF && buff[1] == 0xFF)
            {
                var index = (int)buff[3];
                // receiveData = buff.Skip(2).Take(2 + index).ToList();
                receiveData = buff.Skip(0).Take(4 + index).ToList();
            }

            if (ShowDebugInfo)
            {
                Console.Write("收到指令<<<<<<:");
                receiveData.ForEach(v => Console.Write(v.ToString("X2") + " "));
                Console.WriteLine();
            }
            HandlerData(receiveData);
        }

        private void HandlerData(List<byte> buffer)
        {
            buffer = buffer.Skip(2).Take(buffer.Count - 1).ToList();
            var deviceId = Convert.ToInt16(buffer[0]);
            var se = Devices.Where(p => p.Id == deviceId).FirstOrDefault();
            if (se == null)
            {
                se = new SteeringEngine();
                se.Id = deviceId;
                Devices.Add(se);
            }
            var dataLength = Convert.ToInt16(buffer[1]) - 2;
            se.Error = buffer.Skip(2).Take(1).First();
            var dataBuffer = buffer.Skip(3).Take(dataLength).ToList();
            se.SetReceiverData(dataBuffer);

        }

        /// <summary>
        /// Open Serial Port
        /// </summary>
        /// <returns>OpenStatus</returns>
        public bool Open()
        {
            _serialPort.Open();
            return IsOpend;
        }



        /// <summary>
        /// Close Serial Port
        /// </summary>
        public void Close()
        {
            if (_serialPort != null && _client.IsOpend)
            {
                _serialPort.Close();
            }
        }

        public void SendData(byte id, byte instruction, byte[] parameter = null)
        {
            var arrayData = new List<byte>();
            arrayData.Add(id);
            arrayData.Add(parameter == null ? (byte)2 : (byte)(parameter.Length + 2));
            arrayData.Add(instruction);
            if (parameter != null)
                arrayData.AddRange(parameter);

            arrayData.Add(Tools.CheckSum(arrayData.ToArray()));
            var sendData = new byte[] { 0xFF, 0xFF }.ToList();
            sendData.AddRange(arrayData);

            if (ShowDebugInfo)
            {
                Console.Write("发送指令>>>>>>:");
                sendData.ForEach(v => Console.Write(v.ToString("X2") + " "));
                Console.WriteLine();
            }
            _serialPort.Write(sendData.ToArray(), 0, sendData.Count);
        }

        /// <summary>
        /// Scan Devices
        /// </summary>
        public void ScanDevices()
        {
            SendData(0xFE, 0x01);
        }





    }
}
