//using BingLibrary.hjb.tools;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;

namespace BingLibrary.hjb.PLC
{
    //Update by Bing.2017/10/12
    public class ThingetPLC
    {
        #region 信捷PLC操作

        //读取X线圈
        public bool ReadX(int coilAddress)
        {
            string r;
            try
            {
                r = PLCRead("X", coilAddress);
                r = r == "" ? "0" : r;
            }
            catch { return false; }
            return Convert.ToInt32(r, 16) != 0 ? true : false;
        }

        //读取Y线圈
        public bool ReadY(int coilAddress)
        {
            string r;
            try
            {
                r = PLCRead("Y", coilAddress);
                r = r == "" ? "0" : r;
            }
            catch { return false; }
            return Convert.ToInt32(r, 16) != 0 ? true : false;
        }

        //置位Y线圈,false or true
        public bool SetY(int coilAddress, bool coilValue)
        {
            try
            {
                return coilValue ? PLCWrite("Y", coilAddress, "FF00") : PLCWrite("Y", coilAddress, "0000");
            }
            catch { return false; }
        }

        //读取M线圈
        public bool ReadM(int coilAddress)
        {
            string r;
            try
            {
                r = PLCRead("M", coilAddress);
                r = r == "" ? "0" : r;
            }
            catch { return false; }
            return Convert.ToInt32(r, 16) != 0 ? true : false;
        }
        public bool ReadSM(int coilAddress)
        {
            string r;
            try
            {
                r = PLCRead("SM", coilAddress);
                r = r == "" ? "0" : r;
            }
            catch { return false; }
            return Convert.ToInt32(r, 16) != 0 ? true : false;
        }

        //置位M线圈,false or true
        public bool SetM(int coilAddress, bool coilValue)
        {
            try
            {
                return coilValue ? PLCWrite("M", coilAddress, "FF00") : PLCWrite("M", coilAddress, "0000");
            }
            catch { return false; }
        }

        //读取D寄存器
        public double ReadD(int coilAddressx)
        {
            try
            {
                string r = PLCRead("D", coilAddressx);
                //string t1 = r.Substring(0, 4);
                //string t2 = r.Remove(0, 4);
                //string t3 = t2 + t1;
                double d = Convert.ToInt32(r, 16);
                return d != 0 ? d : 0;
            }
            catch { return 0; }
        }

        //写入D寄存器
        public bool WriteD(int coilAddress, string coilValue)
        {
            try
            {
                return PLCWrite("D", coilAddress, coilValue);
            }
            catch { return false; }
        }

        //读取W寄存器
        public double ReadW(int coilAddressx)
        {
            try
            {
                string r = PLCRead("W", coilAddressx);
                double d = Convert.ToInt16(r, 16);
                return d != 0 ? d : 0;
            }
            catch { return 0; }
        }

        //写入W寄存器
        public bool WriteW(int coilAddress, string coilValue)
        {
            try
            {
                string s1 = Convert.ToString(Convert.ToInt32(coilValue, 10), 16);
                return PLCWrite("W", coilAddress, s1);
            }
            catch { return false; }
        }
        /// <summary>
        /// 信捷批量写入线圈
        /// </summary>
        /// <param name="coilAddress"></param>
        /// <param name="ba"></param>
        /// <returns></returns>
        public bool WritMultiMCoil(int coilAddress, bool[] ba)
        {
            string s1 = "";
            for (int i = 0; i < ba.Length; i++)
            {
                s1 += ba[ba.Length - 1 - i] ? "1" : "0";
            }
            return PLCWrite("MutiM", coilAddress, s1);
        }
        //一次读160个M
        public bool[] ReadMultiMCoil(int coilStartAddress)
        {
            string temp0 = PLCRead("M", coilStartAddress, "01", "00a0");
            string temp1 = "";
            try
            {
                for (int i = 0; i < 40; i = i + 2)
                {
                    temp1 += temp0[i + 1].ToString() + temp0[i].ToString();
                }
            }
            catch
            {

            }

            string[] temp2 = new string[40];

            bool[] result = new bool[160];

            string temp160 = "";
            try
            {
                for (int i = 0; i < 40; i++)
                {
                    temp2[i] = temp1[i].ToString();
                    int t0 = Convert.ToInt32(temp2[i], 16);
                    string t1 = Convert.ToString(t0, 2);
                    int t2 = Convert.ToInt32(t1);
                    string t3 = string.Format("{0:D4}", t2);
                    temp160 += BinaryReverse(t3);
                }
            }
            catch { }
            try
            {
                for (int i = 0; i < 160; i++)
                {
                    result[i] = temp160[i] == '1';
                }
            }
            catch { }
            return result;
        }
        public int[] readMultiW(int coilStartAdress,int count)
        {
            string temp0;
            temp0 = PLCRead("W", coilStartAdress, "01", count.ToString("X4"));
            int[] rst = new int[count];
            for (int i = 0; i < count; i++)
            {
                rst[i] = Convert.ToInt32(temp0.Substring(4 * i, 4), 16);
            }
            return rst;

        }
        //一次读30个D
        public ObservableCollection<double> readMultiD(int coilStartAdress)
        {
            string temp0;
            temp0 = PLCRead("D", coilStartAdress, "01", "001e");
            ObservableCollection<double> result = new ObservableCollection<double>();
            try
            {
                for (int i = 0; i < 30; i++)
                {
                    result.Add(Convert.ToInt32(temp0.Substring(8 * i, 8), 16));
                }
            }
            catch { }

            return result;
        }
        public ObservableCollection<double> readMultiHD(int coilStartAdress)
        {
            string temp0;
            temp0 = PLCRead("HD", coilStartAdress, "01", "001e");
            ObservableCollection<double> result = new ObservableCollection<double>();
            try
            {
                for (int i = 0; i < 30; i++)
                {
                    result.Add(Convert.ToInt32(temp0.Substring(8 * i, 8), 16));
                }
            }
            catch { }

            return result;
        }

        #endregion 信捷PLC操作

        #region Modbus Lib

        private string mRecStr;
        private ManualResetEvent Pause_Event = new ManualResetEvent(false);
        private object SerialLock = new object();

        private bool PLCWrite(string coilType, int coilAddress, string coilData, string deviceID = "01")
        {
            string modbusStr;
            string functionCode, coilStartlAddress, coliValue;
            switch (coilType)
            {
                case "M":
                    functionCode = "05";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;

                case "X":
                    functionCode = "05";
                    coilStartlAddress = (16384 + coilAddress).ToString("X4");
                    break;

                case "Y":
                    functionCode = "05";
                    coilStartlAddress = (18432 + coilAddress).ToString("X4");
                    break;

                case "S":
                    functionCode = "05";
                    coilStartlAddress = (20480 + coilAddress).ToString("X4");
                    break;

                case "D"://32位
                    functionCode = "10";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;

                case "W"://16位
                    functionCode = "06";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;
                case "MutiM":
                    functionCode = "0F";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;
                default:
                    return false;
            }

            int tempV1;
            if (coilType == "D")
            {
                tempV1 = Convert.ToInt32(coilData);
                coliValue = tempV1.ToString("X8");
                string SubStr;
                SubStr = coliValue.Substring(0, 4);
                coliValue = coliValue.Insert(8, SubStr);
                coliValue = coliValue.Remove(0, 4);
                modbusStr = deviceID + functionCode + coilStartlAddress + "000204" + coliValue;
            }
            else
            {
                if (coilType == "MutiM")
                {
                    string NewData = "";
                    for (int i = 0; i < 8 - coilData.Length % 8 && coilData.Length % 8 != 0; i++)
                    {
                        NewData += "0";
                    }
                    NewData += coilData;
                    string Quantity = coilData.Length.ToString("X4");
                    string byteCount = (NewData.Length / 8).ToString("X2");
                    modbusStr = deviceID + functionCode + coilStartlAddress + Quantity + byteCount;
                    for (int i = 0; i < NewData.Length / 8; i++)
                    {
                        string Coils8 = NewData.Substring(NewData.Length - 8 * (i + 1), 8);
                        tempV1 = Convert.ToInt32(Coils8, 2);
                        coliValue = tempV1.ToString("X2");
                        modbusStr += coliValue;
                    }
                }
                else
                {
                    tempV1 = Convert.ToInt32(coilData, 16);
                    coliValue = tempV1.ToString("X4");
                    if (coliValue.Length > 4)
                    {
                        coliValue = coliValue.Substring(4);
                    }
                    modbusStr = deviceID + functionCode + coilStartlAddress + coliValue;
                }

            }
            byte[] mByteToWrite = StrToByte(modbusStr);

            lock (SerialLock)
            {
                mRecStr = "";
                curSerialPort.ReadExisting();
                curSerialPort.Write(mByteToWrite, 0, mByteToWrite.Length);

                //Thread.Sleep(20);
                int len = 8;
                int mCount = curSerialPort.BytesToRead;
                int mtiemout = 0;
                while (mCount < len)
                {
                    mCount = curSerialPort.BytesToRead;
                    Thread.Sleep(10);
                    mtiemout++;
                    if (mtiemout > 100)
                    {
                        curSerialPort.ReadExisting();
                        return false;
                    }
                }

                byte[] mRecByte = new byte[len];
                curSerialPort.Read(mRecByte, 0, len);
                mRecStr = "";
                for (int ctick = 0; ctick < len; ctick++)
                {
                    mRecStr = mRecStr + mRecByte[ctick].ToString("X2");
                }

                if (modbusStr.Contains(mRecStr.Remove(12)))
                    return true;
                else
                {
                    //Tool.DebugInfo("写入PLC失败!");
                    return false;
                }
            }
        }

        private string PLCRead(string coilType, int coilAddress, string deviceID = "01", string coilCount = "0001", bool mHex = false)
        {
            string functionCode, coilStartlAddress, coliValue;
            string modbusStr;
            switch (coilType)
            {
                case "M":
                    functionCode = "01";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;

                case "X":
                    functionCode = "01";
                    coilStartlAddress = (16384 + coilAddress).ToString("X4");
                    break;

                case "Y":
                    functionCode = "01";
                    coilStartlAddress = (18432 + coilAddress).ToString("X4");
                    break;

                case "S":
                    functionCode = "01";
                    coilStartlAddress = (20480 + coilAddress).ToString("X4");
                    break;

                case "SM":
                    functionCode = "01";
                    coilStartlAddress = (36864 + coilAddress).ToString("X4");
                    break;

                case "D"://32位
                    functionCode = "03";
                    coilStartlAddress = coilAddress.ToString("X4");
                    coilCount = (Convert.ToInt32(coilCount, 16) * 2).ToString("X4");
                    break;
                case "HD"://32位
                    functionCode = "03";
                    coilStartlAddress = (41087 + coilAddress).ToString("X4");
                    coilCount = (Convert.ToInt32(coilCount, 16) * 2).ToString("X4");
                    break;
                case "W"://16位
                    functionCode = "03";
                    coilStartlAddress = coilAddress.ToString("X4");
                    break;

                default:
                    return "";
            }

            modbusStr = deviceID + functionCode + coilStartlAddress + coilCount;

            byte[] mByteToWrite = StrToByte(modbusStr);

            int len = Convert.ToInt32(coilCount, 16);
            if (functionCode == "01")
            {
                if (len % 8 != 0)

                    len = len / 8 + 1;
                else
                    len = len / 8;

                len += 5;
            }
            else
            {
                len = len * 2 + 5;
            }
            lock (SerialLock)
            {
                mRecStr = "";
                curSerialPort.ReadExisting();
                curSerialPort.Write(mByteToWrite, 0, mByteToWrite.Length);

                //Thread.Sleep(20);
                int mCount = curSerialPort.BytesToRead;
                int mtiemout = 0;
                while (mCount < len)
                {
                    mCount = curSerialPort.BytesToRead;
                    Thread.Sleep(1);
                    mtiemout++;
                    if (mtiemout > 100)
                    {
                        curSerialPort.ReadExisting();
                        return "";
                    }
                }

                byte[] mRecByte = new byte[len];
                curSerialPort.Read(mRecByte, 0, len);
                mRecStr = "";
                for (int ctick = 0; ctick < len; ctick++)
                {
                    mRecStr = mRecStr + mRecByte[ctick].ToString("X2");
                }

                int m;
                if (mRecStr.Contains(deviceID + functionCode))
                {
                    m = 2 * Convert.ToInt32(mRecStr.Substring(4, 2), 16);
                    coliValue = mRecStr.Substring(6, m);
                    if (coilType == "D")
                    {
                        for (int n = 1; n < m / 8 + 1; n++)
                        {
                            string SubStr;
                            SubStr = coliValue.Substring((n - 1) * 8, 4);
                            coliValue = coliValue.Insert(8 * n, SubStr);
                            coliValue = coliValue.Remove((n - 1) * 8, 4);
                        }
                    }
                    if (mHex)
                    {
                        coliValue = "0x" + coliValue;
                    }

                    return coliValue;
                }
                //Tool.DebugInfo("读取PLC失败!");
                return "";
            }
        }

        public SerialPort curSerialPort;

        public void ModbusInit(String PortName, int BaudRate, Parity Parity, int DataBits, StopBits StopBits)
        {
            curSerialPort = new SerialPort();
            curSerialPort.PortName = PortName;
            curSerialPort.BaudRate = BaudRate;
            curSerialPort.Parity = Parity;
            curSerialPort.DataBits = DataBits;
            curSerialPort.StopBits = StopBits;
        }

        public bool ModbusConnect()
        {
            bool isConnected = false;
            int connectCounts = 0;
            while (curSerialPort?.PortName != "" && curSerialPort?.IsOpen == false && isConnected == false && connectCounts < 10)
            {
                try
                {
                    curSerialPort.Open();
                    isConnected = true;
                }
                catch
                {
                    Thread.Sleep(500);
                    connectCounts++;
                    isConnected = false;
                }
            }
            return isConnected;
        }

        public void ModbusDisConnect()
        {
            try
            {
                curSerialPort?.Close();
            }
            catch { }
        }

        private string BinaryReverse(string text)
        {
            char[] charArray = text.ToCharArray();
            int len = text.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                char tmp = charArray[i];
                charArray[i] = charArray[len];
                charArray[len] = tmp;
            }

            return new string(charArray);
        }

        //计算PLC的校验位、、高低位没处理
        private int CRC_Verify(byte[] cBuffer, int iBufLen)
        {
            int i, j;                 //#define wPolynom 0xA001
            int wCrc = 0xffff;
            int wPolynom = 0xA001;  /*---------------------------------------------------------------------------------*/
            for (i = 0; i < iBufLen; i++)
            {
                wCrc ^= cBuffer[i];
                for (j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) != 0)
                    { wCrc = (wCrc >> 1) ^ wPolynom; }
                    else { wCrc = wCrc >> 1; }
                }
            }
            return wCrc;
        }

        private byte[] StrToByte(string mStr)
        {
            int wCrc = 0xFFFF;
            int wPolynom = 0xA001;
            mStr = mStr.Trim();
            int count = mStr.Length / 2;
            byte[] b = new byte[count + 2];
            for (int ctick = 0; ctick < count; ctick++)
            {
                string temp = mStr.Substring(ctick * 2, 2);
                b[ctick] = Convert.ToByte(temp, 16);
                wCrc ^= b[ctick];
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) != 0)
                    { wCrc = (wCrc >> 1) ^ wPolynom; }
                    else { wCrc = wCrc >> 1; }
                }
            }
            string strCrc = wCrc.ToString("X4");
            b[count] = Convert.ToByte(strCrc.Substring(2, 2), 16);
            b[count + 1] = Convert.ToByte(strCrc.Substring(0, 2), 16);

            return b;
        }

        #endregion Modbus Lib
    }
}