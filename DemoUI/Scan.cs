using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace JasperUnloadUI.Model
{
    public class Scan
    {
        public event EventHandler StateChanged;
        public bool mState;
        public bool State
        {
            get { return mState; }
            set
            {
                if (mState != value)
                {
                    mState = value;
                    if (StateChanged != null)
                        StateChanged(null, null);
                }
            }
        }
        public void ini(string Com)
        {
            mSerialPort = new SerialPort(Com, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            mSerialPort.ReadTimeout = 10000;
            mSerialPort.WriteTimeout = 10000;
        }
        public void Connect()
        {
            try
            {
                mSerialPort.Open();
                State = true;
            }
            catch (Exception ex) { Trace.WriteLine(ex.Message, "扫码连接"); }
        }

        public bool DoBarcode = true;

        public SerialPort mSerialPort;
        //public static bool DoBarcode=true;
        public string BarCode;
        //static byte[] START_DECODE = new byte[] { 0x4C, 0x4F, 0x4E, 0x0D,0x0A };//{0x03,0x53,0x80,0xFF,0x2A };
        //static byte[] STOP_DECODE = new byte[] { 0x4C, 0x4F, 0x46, 0x46, 0x0D, 0x0A };
        static byte[] START_DECODE = new byte[] { 0x16, 0x54, 0x0D };
        static byte[] STOP_DECODE = new byte[] { 0x16, 0x55, 0x0D };
        static byte[] MODE_DECODE = new byte[] { 0x16, 0x4D, 0x0D, 0x30, 0x34, 0x30, 0x31, 0x44, 0x30, 0x35, 0x2E };
        public delegate void ProcessDelegate(string barcode);
        public async void GetBarCode(ProcessDelegate CallBack)
        {
            BarCode = "Error";
            Func<System.Threading.Tasks.Task> taskFunc = () =>
            {
                return System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        if (DoBarcode)
                        {
                            if (!mSerialPort.IsOpen)
                                Connect();
                            mSerialPort.ReadExisting();
                            mSerialPort.Write(START_DECODE, 0, START_DECODE.Length);
                            BarCode = mSerialPort.ReadLine();
                            string[] ss = BarCode.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                            BarCode = ss[0];
                        }
                        State = true;
                    }
                    catch (Exception ex)
                    {
                        State = false;
                        Trace.WriteLine(ex.Message, "GetBarCode");
                    }
                    try
                    {
                        if (DoBarcode)
                        {
                            if (!mSerialPort.IsOpen)
                                Connect();
                            mSerialPort.ReadExisting();
                            mSerialPort.Write(STOP_DECODE, 0, STOP_DECODE.Length);
                        }
                        State = true;
                    }
                    catch (Exception ex)
                    {
                        State = false;
                        Trace.WriteLine(ex.Message, "StopBarCode");
                    }
                });
            };
            await taskFunc();
            CallBack(BarCode);
        }
        public void SetMode()
        {
            try
            {
                if (DoBarcode)
                {
                    if (!mSerialPort.IsOpen)
                        Connect();
                    mSerialPort.ReadExisting();
                    mSerialPort.Write(MODE_DECODE, 0, MODE_DECODE.Length);
                }
                State = true;
            }
            catch (Exception ex)
            {
                State = false;
                Trace.WriteLine(ex.Message, "SetMode");
            }
        }
    }
}
