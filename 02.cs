using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Comms;
using Packets;
using PMComm;
using NNPWSCOMM;

namespace NNPWSPC
{
    /// <summary>
    /// 无负压供水配参工具主控界面类
    /// </summary>
    public partial class Form_NNPWSPC : Form
    {
        private Cls_PCTClient m_Client=null;
        private Cls_SerialPort m_SerialPort = null;
        private Cls_PCTClientMsgP m_ClientMsgProcess = null;        
        private string m_Fn = Application.StartupPath + "\\PCT.conf"; //保存配置信息的文件
        //yth更改，加入短信发送
        private Cls_SMSSP cls_SMSSP = null;
        /// <summary>
        /// 管理服务器缺省值定义
        /// </summary>
        private string m_SysServerIP = "127.0.0.1";
        private int m_SysServerPort = 8891;

        /// <summary>
        /// 以下是需要保存的数据
        /// </summary>       
        //本地端口号定义
        private int m_MPort = 10001;//本地端口号       

        //串口参数
        private string m_PortName = "COM1";
        private byte m_BaudRate = 1;
        private byte m_DataBits = 3;
        private byte m_StopBits = 0;
        private byte m_Parity = 1;
        //项目ID和类别码
        private byte[] m_ProjectID = new byte[5] { 0xFF, 0xFF, 0x00, 0x00, 0x01 }; //可以通过修改程序设定
        private int m_ClassCode = 0x0001;

        //系统参数
        private Cls_CollectorSPB m_CollectorSPB = null;

        //运行参数
        private Cls_CollectorPB m_CollectorPB = null;
        //加入控制按钮的状态
        //yth更改
               //自动设备启动状态
         private bool button_status_auto=true;        
        //手动泵1启动状态
         private bool button_status_pump1=true;        
        //手动泵2启动状态
         private bool button_status_pump2=true;       
        //手动泵3启动状态
         private bool button_status_pump3=true;
        //手动泵4启动状态
         private bool button_status_pump4=true;
        //手动辅泵启动状态
         private bool button_status_pump5=true;
        //手动增压启动状态
         private bool button_status_pump6=true;
        //手动电动阀启动状态
         private bool button_status_ele = true;
        
        






        /// <summary>
        /// 保存相关配置信息到文件中
        /// </summary>
        /// <returns></returns>
        private bool SaveToFile()
        {
            bool m_rt = false;

            try
            {
                using (FileStream fs = new FileStream(m_Fn, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    
                    formatter.Serialize(fs, m_MPort);

                    formatter.Serialize(fs, m_PortName);
                    formatter.Serialize(fs, m_BaudRate);
                    formatter.Serialize(fs, m_DataBits);
                    formatter.Serialize(fs, m_StopBits);
                    formatter.Serialize(fs, m_Parity);
                    formatter.Serialize(fs, m_ProjectID);
                    formatter.Serialize(fs, m_ClassCode);

                    formatter.Serialize(fs, m_CollectorSPB.ToBytes());
                    formatter.Serialize(fs, m_CollectorPB.ToBytes());
                    
                    fs.Close();

                    m_rt = true;
                }
            }
            catch
            {
                m_rt = false;
            }

            return m_rt;
        }

        /// <summary>
        /// 从文件中读取相关配置信息
        /// </summary>
        /// <returns></returns>
        private bool ReadFromFile()
        {
            bool m_rt = false;

            if (File.Exists(m_Fn))
            {
                try
                {
                    using (FileStream fs = new FileStream(m_Fn, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        m_MPort = (int)formatter.Deserialize(fs);

                        m_PortName = (string)formatter.Deserialize(fs);
                        m_BaudRate = (byte)formatter.Deserialize(fs);
                        m_DataBits = (byte)formatter.Deserialize(fs);
                        m_StopBits = (byte)formatter.Deserialize(fs);
                        m_Parity = (byte)formatter.Deserialize(fs);

                        m_ProjectID = (byte[])formatter.Deserialize(fs);
                        m_ClassCode = (int)formatter.Deserialize(fs);

                        m_CollectorSPB = new Cls_CollectorSPB((byte[])formatter.Deserialize(fs));
                        m_CollectorPB = new Cls_CollectorPB((byte[])formatter.Deserialize(fs));

                        fs.Close();

                        m_rt = true;
                    }
                }
                catch
                {
                    m_rt = false;
                }
            }

            return m_rt;
        }

        /// <summary>
        /// 初始化串口和配参对象
        /// </summary>
        /// <returns></returns>
        private bool InitialPCT()
        {
            bool m_rt = false;

            //初始化并打开串口            
            try
            {
                if (m_SerialPort == null) m_SerialPort = new Cls_SerialPort(m_PortName, m_BaudRate, m_DataBits, m_StopBits, m_Parity);

                if ((m_SerialPort != null) && (!m_SerialPort.IsOpen))
                {
                    m_SerialPort.Open();
                    m_rt = true;
                }
            }
            catch
            {
                m_SerialPort = null;
                MessageBox.Show("串口：" + m_PortName + "无法打开，请重新设置通信参数");
            }
            finally
            {
                toolStripStatusLabel_SerialMsg.Text = m_SerialPort.GetComPortConfig;
            }

            //配参数据对象初始化
            if (m_rt)
            {
                if (m_Client == null)
                {
                    try
                    {
                        m_rt = false;

                        m_Client = new Cls_PCTClient(m_SerialPort, m_CollectorSPB, m_CollectorPB);

                        m_Client.MPort = m_MPort;

                        m_Client.ProjectID = m_ProjectID;
                        m_Client.ClassCode = m_ClassCode;

                        m_Client.m_DispMessageEvent += new DispMessageEvent(OnDispMessage);
                        m_Client.m_DisplayClientSysAlarm += new DisplayClientSysAlarm(OnDisplayClientSysAlarm);
                        m_Client.m_DisplaySPB += new DisplaySPB(OnDisplaySPB);
                        m_Client.m_DisplayPB += new DisplayPB(OnDisplayPB);

                        m_rt = true;
                    }
                    catch (Exception ex)
                    {
                        m_Client = null;
                        MessageBox.Show("创建配参对象错误:[" + ex.Message + "]");
                    }
                }
            }

            if (m_rt)
            {
                try
                {
                    m_rt = false;

                    if (m_ClientMsgProcess == null)
                    {
                        m_ClientMsgProcess = new Cls_PCTClientMsgP(m_Client, 1000);
                        m_ClientMsgProcess.m_DispMessageEvent += new DispMessageEvent(OnDispMessage);
                        m_ClientMsgProcess.m_DisplayClientSysAlarm += new DisplayClientSysAlarm(OnDisplayClientSysAlarm);
                    }

                    if (m_ClientMsgProcess != null) m_ClientMsgProcess.Start();

                    m_rt = true;
                }
                catch (Exception ex)
                {
                    m_ClientMsgProcess = null;
                    MessageBox.Show("创建配参消息处理对象失败：[" + ex.Message + "]");
                }
            }

            return m_rt;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Form_NNPWSPC()
        {
            bool m_rt = false;

            InitializeComponent();
            
            //读取配置文件并初始化相关参数对象                        
            m_rt = ReadFromFile();

            if (m_rt == false) //按缺省设置
            {
                MessageBox.Show("配置文件读取失败!");

                //回复缺省设置
                m_SysServerIP = "127.0.0.1";
                m_SysServerPort = 8891;

                m_MPort = 10001;//本地端口号       

                m_PortName = "COM1";
                m_BaudRate = 1;
                m_DataBits = 3;
                m_StopBits = 0;
                m_Parity = 1;

                m_ProjectID = new byte[5] { 0xFF, 0xFF, 0x00, 0x00, 0x01 };
                m_ClassCode = 0x0001;

                m_CollectorSPB = new Cls_CollectorSPB();

                m_CollectorSPB.SysServerIP = m_SysServerIP;
                m_CollectorSPB.SysPort = m_SysServerPort;

                m_CollectorPB = new Cls_CollectorPB();
            }

            //显示相关参数
            OnDisplaySPB(m_CollectorSPB);
            OnDisplayPB(m_CollectorPB);

            //初始化配参对象并打开串口            
            m_rt = InitialPCT();            
        }

        /// <summary>
        /// 显示系统警告
        /// </summary>
        /// <param name="st"></param>
        private void OnDisplayClientSysAlarm(string st)
        {

            if (statusStrip_main.InvokeRequired)
            {
                DisplayClientSysAlarm m_DisplayClientSysAlarm = new DisplayClientSysAlarm(Fn_DisplayClientSysAlarm);
                this.Invoke(m_DisplayClientSysAlarm, new object[] { st });
            }
            else
            {
                Fn_DisplayClientSysAlarm(st);
            }

        }

        /// <summary>
        /// 显示系统警告
        /// </summary>
        /// <param name="st"></param>
        void Fn_DisplayClientSysAlarm(string st)
        {
            toolStripStatusLabel_SysAlarm.Text = st;

            MessageBox.Show(st);          
        }

        /// <summary>
        /// 显示数据包
        /// </summary>
        /// <param name="packetData">数据包</param>
        /// <param name="st">源类型</param>
        private void OnDispMessage(cls_Packet packetData, string st)
        {

            if (listView_DispPackets.InvokeRequired)
            {
                DispMessageEvent m_DisplayPacket = new DispMessageEvent(DisplayPacketMsg);
                this.Invoke(m_DisplayPacket, new object[] { packetData, st });
            }
            else
            {
                DisplayPacketMsg(packetData, st);
            }

        }

        /// <summary>
        /// 显示数据包
        /// </summary>
        /// <param name="packetData">要显示的数据包</param>
        /// <param name="st">要显示的源类型</param>
        private void DisplayPacketMsg(cls_Packet packetData, string st)
        {
            try
            {
                while (listView_DispPackets.Items.Count > 1000) listView_DispPackets.Items.RemoveAt(0);

                ListViewItem m_lvt = listView_DispPackets.Items.Add(st);
                m_lvt.SubItems.Add(packetData.Ver.ToString());
                m_lvt.SubItems.Add(packetData.Len.ToString());
                m_lvt.SubItems.Add(packetData.Type == 0 ? "命令包" : "透传包");
                m_lvt.SubItems.Add(packetData.FrameNo.ToString());

                if (packetData.Type == 0) //命令包
                {
                    cls_CMDPacket m_CMDPacket = new cls_CMDPacket(packetData.ToByte());

                    m_lvt.SubItems.Add(Cls_Funs.Fn_BCDsToString(m_CMDPacket.TerminalID, 0, 5, true)); //采集器ID
                    m_lvt.SubItems.Add(m_CMDPacket.CMD.ToString()); //命令码

                    if (m_CMDPacket.CMD == 0x13) //数据上传数据包(UploadStateData)     0x13
                    {
                        //分析上传数据包数据内容
                        byte[] m_tmp = m_CMDPacket.ToByte();
                        //yth
                        cls_CMDPacket m_cmpacket = new cls_CMDPacket(m_tmp);
                        Cls_UploadStateData m_UploadStateData = new Cls_UploadStateData(m_tmp);

                        int recn = m_UploadStateData.RecordNum;

                        string m_st = "";

                        if (recn >= 1)
                        {
                            m_st = m_st + "记录数:" + recn.ToString();
                            m_st = m_st + " 采集时间：" + m_UploadStateData.CollectorSDatas[0].AcquisitionTime.ToShortTimeString();                            
                        }

                        m_lvt.SubItems.Add(m_st + "【" + Cls_Funs.Fn_BytesToHexString((new cls_CMDPacket(packetData.ToByte())).Data) + "】");
                        //yth加上状态数据刷新

                    }
                    else
                        m_lvt.SubItems.Add(Cls_Funs.Fn_BytesToHexString((new cls_CMDPacket(packetData.ToByte())).Data));
                }
                else //透传包
                {
                    m_lvt.SubItems.Add("");
                    m_lvt.SubItems.Add("");
                    m_lvt.SubItems.Add(Cls_Funs.Fn_BytesToHexString(packetData.Data));
                }

                m_lvt.SubItems.Add(Cls_Funs.Fn_BytesToHexString(packetData.CheckCode));
            }
            catch (Exception ex)
            {
                Fn_DisplayClientSysAlarm("帧信息显示错误:[" + ex.Message + "]");// MessageBox.Show("帧信息显示错误:[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// 显示系统参数
        /// </summary>
        /// <param name="floatCollectorSPB"></param>
        private void OnDisplaySPB(Cls_CollectorSPB collectorSPB)
        {
            if (tabPage_CPB.InvokeRequired)
            {
                DisplaySPB m_DisplaySPB = new DisplaySPB(Fn_DisplaySPB);
                this.Invoke(m_DisplaySPB, new object[] { collectorSPB });
            }
            else
            {
                Fn_DisplaySPB(collectorSPB);
            }
        }

        /// <summary>
        /// 显示系统参数
        /// </summary>
        /// <param name="floatCollectorSPB"></param>
        private void Fn_DisplaySPB(Cls_CollectorSPB collectorSPB)
        {
            try
            {
                //公共参数
                textBox_SoftVer.Text = collectorSPB.SoftVer;
                string m_ts = Cls_Funs.Fn_BytesToHexString(collectorSPB.TerminalIDBytes);
                //string m_ts = Cls_Funs.Fn_BytesToHexString(m_ProjectID);
                if ((m_ts != null) && (m_ts.Length == 10))
                {
                    textBox_TerminalID0.Text = m_ts.Substring(0, 7);
                    textBox_TerminalID.Text = m_ts.Substring(7, 3);
                }
                else
                {
                    byte buf = (byte)((DateTime.Now.Year - 2010) * 4 + (Cls_Funs.WeekOfYear(DateTime.Now) / 16));

                    textBox_TerminalID0.Text = "FFFF" + buf.ToString("X2") + (Cls_Funs.WeekOfYear(DateTime.Now) % 16).ToString("X1");
                    textBox_TerminalID.Text = "001";
                }

                textBox_TerminalName.Text = collectorSPB.TerminalName;

                textBox_Longitude.Text = collectorSPB.Longitude.ToString();
                textBox_Latitude.Text = collectorSPB.Latitude.ToString();

                comboBox_APN.SelectedIndex = collectorSPB.APN;
                comboBox_CommunicationMode.SelectedIndex = collectorSPB.CommunicationMode;
                comboBox_ProtocolType.SelectedIndex = collectorSPB.ProtocolType;

                numericUpDown_UploadInterval.Value = collectorSPB.UploadInterval;
                numericUpDown_UploadIimerlen.Value = collectorSPB.UploadIimerlen;
                numericUpDown_ReUploadNumber.Value = collectorSPB.ReUploadNumber;
                numericUpDown_ReUploadDelay.Value = collectorSPB.ReUploadDelay;

                //终端参数不显示
               textBox_SysServerIP.Text = collectorSPB.SysServerIP;
                numericUpDown_SysPort.Value = collectorSPB.SysPort;

                textBox_MyIP.Text = collectorSPB.MyIP;
                textBox_MyMask.Text = collectorSPB.MyMask;
                textBox_MyGate.Text = collectorSPB.MyGate;
                textBox_MyDNS.Text = collectorSPB.MyDNS;

                //应用参数
                comboBox_485AMode.SelectedIndex = collectorSPB.R485AMode;
                numericUpDown_485ANo.Value = collectorSPB.R485ANo;
                comboBox_485ABaudRate.SelectedIndex = collectorSPB.R485ABaudRate;
                comboBox_485ADataBit.SelectedIndex = collectorSPB.R485ADataBit;
                comboBox_485AStopBit.SelectedIndex = collectorSPB.R485AStopBit;
                comboBox_485AParityBit.SelectedIndex = collectorSPB.R485AParityBit;

                comboBox_485BMode.SelectedIndex = collectorSPB.R485BMode;
                numericUpDown_485BNo.Value = collectorSPB.R485BNo;
                comboBox_485BBaudRate.SelectedIndex = collectorSPB.R485BBaudRate;
                comboBox_485BDataBit.SelectedIndex = collectorSPB.R485BDataBit;
                comboBox_485BStopBit.SelectedIndex = collectorSPB.R485BStopBit;
                comboBox_485BParityBit.SelectedIndex = collectorSPB.R485BParityBit;

                numericUpDown_TransactionQueryInterval.Value = collectorSPB.TransactionQueryInterval;
                numericUpDown_CollectInterval.Value = collectorSPB.CollectInterval;
                numericUpDown_CollectTimerLen.Value = collectorSPB.CollectTimerLen;
                //yth更改加入 显示设备选型和配件选型
                //设备选型
                comboBox_EquipmentType.SelectedIndex = collectorSPB.EquipmentType;
                checkBox_TemperatureHumidityInstrumentCFlag.Checked = m_CollectorSPB.TemperatureHumidityInstrumentCFlag;
                //流量计
                checkBox_FlowmeterCFlag.Checked = m_CollectorSPB.FlowmeterCFlag;
                //开箱报警
                checkBox_OpenBoxInstrumentCFlag.Checked = m_CollectorSPB.OpenBoxInstrumentCFlag;
                //指纹阅读器
                checkBox_OpenBoxInstrumentCFlag.Checked = m_CollectorSPB.FingerprintInstrumentCFlag;

                textBox_BackupPassword.Text = m_CollectorSPB.BackupPassword;
                textBox_TerminalPassWord.Text = m_CollectorSPB.TerminalPassWord;

                textBox_TelePhones.Text = "";
                if (m_CollectorSPB.TelePhoneNumber >= 1)
                    textBox_TelePhones.Text = m_CollectorSPB.TelePhones[0].Trim();
                if (m_CollectorSPB.TelePhoneNumber >= 2)
                    textBox_TelePhones.Text = textBox_TelePhones.Text + System.Environment.NewLine + m_CollectorSPB.TelePhones[1].Trim();

            }
            catch (Exception ex)
            {
                Fn_DisplayClientSysAlarm("系统参数信息显示错误:[" + ex.Message + "]"); //MessageBox.Show("系统参数信息显示错误:[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// 显示运行参数
        /// </summary>
        /// <param name="floatCollectorSPB"></param>
        private void OnDisplayPB(Cls_CollectorPB collectorPB)
        {
            if (tabPage_CPB.InvokeRequired)
            {
                DisplayPB m_DisplayPB = new DisplayPB(Fn_DisplayPB);
                this.Invoke(m_DisplayPB, new object[] { collectorPB });
            }
            else
            {
                Fn_DisplayPB(collectorPB);
            }
        }

        /// <summary>
        /// 显示运行参数
        /// </summary>
        /// <param name="floatCollectorPB"></param>
        private void Fn_DisplayPB(Cls_CollectorPB collectorPB)
        {
            try
            {
                if (m_CollectorPB.SettingTime != null)
                //textBox_SettingTime.Text = m_CollectorPB.SettingTime.ToString("yyMMddhhmmss");

                //textBox_PSouce.Text = m_CollectorPB.PSouce.ToString();

                //textBox_PumpPowerMax1.Text = m_CollectorPB.PumpPowerMax1.ToString();
                //textBox_PumpPowerMax2.Text = m_CollectorPB.PumpPowerMax2.ToString();
                //textBox_PumpPowerMax3.Text = m_CollectorPB.PumpPowerMax3.ToString();
                //textBox_PumpPowerMax4.Text = m_CollectorPB.PumpPowerMax4.ToString();
                //textBox_PumpPowerMax5.Text = m_CollectorPB.PumpPowerMax5.ToString();

                //textBox_SettingPressure.Text = m_CollectorPB.SettingPressure.ToString("0.00");
                //textBox_InletPressureLowest.Text = m_CollectorPB.InletPressureLowest.ToString("0.00");
                //textBox_OutletPressureMaximum.Text = m_CollectorPB.OutletPressureMaximum.ToString("0.00");
                ////textBox_InverterFrequencyMaximum.Text = m_CollectorPB.InverterFrequencyMaximum.ToString();
                ////extBox_InverterPowerLowest.Text = m_CollectorPB.InverterPowerLowest.ToString();
                //textBox_TimingRotationTime.Text = m_CollectorPB.TimingRotationTime.ToString();
                //textBox_PressureFullscale.Text = m_CollectorPB.PressureFullscale.ToString("0.0");

                //textBox_InletPressureThreshold.Text = m_CollectorPB.InletPressureThreshold.ToString("0.00");
                //textBox_OutletPressureThreshold.Text = m_CollectorPB.OutletPressureThreshold.ToString("0.00");
                ////textBox_FrequencyThreshold.Text = m_CollectorPB.FrequencyThreshold.ToString();
                ////textBox_PowerVThreshold.Text = m_CollectorPB.PowerVThreshold.ToString();
                //textBox_FlowVThreshold.Text = m_CollectorPB.FlowVThreshold.ToString();
                //textBox_LiquidlevelThreshold.Text = m_CollectorPB.LiquidlevelThreshold.ToString();            


                //yth更改2016/05/25
                    
                 //设置时间
                textBox_SettingTime.Text = m_CollectorPB.SettingTime.ToString("yyMMddhhmmss");
                //参数来源
                textBox_PSouce.Text = m_CollectorPB.PSouce.ToString();
                //泵1额定功率
                textBox_PumpPowerMax1.Text = m_CollectorPB.PumpPowerMax1.ToString();
                //泵2额定功率
                textBox_PumpPowerMax2.Text = m_CollectorPB.PumpPowerMax2.ToString();
                //泵3额定功率
                textBox_PumpPowerMax3.Text = m_CollectorPB.PumpPowerMax3.ToString();
                //泵4额定功率
                textBox_PumpPowerMax4.Text = m_CollectorPB.PumpPowerMax4.ToString();
                //泵5额定功率(辅泵)
                textBox_PumpPowerMax5.Text = m_CollectorPB.PumpPowerMax5.ToString();
                //泵6额定功率
                textBox_PumpPowerMax6.Text = m_CollectorPB.PumpPowerMax6.ToString();
                //设定压力
                textBox_SettingPressure.Text = m_CollectorPB.SettingPressure.ToString();
                //设定增压压力(有增压泵时使用)
                textBox_SettingBoostPressure.Text = m_CollectorPB.SettingBoostPressure.ToString();
                ////进水压力低限
                textBox_InletPressureLowest.Text = m_CollectorPB.InletPressureLowest.ToString();
                //进水压力(自来水)低限(箱式时使用)
                textBox_PipePressureLowest.Text = m_CollectorPB.PipePressureLowest.ToString();
                //出水压力高限
                textBox_OutletPressureMaximum.Text = m_CollectorPB.OutletPressureMaximum.ToString();
                //出水压力(增压)高限（有增压泵时使用）
                textBox_BoostPressureMaximum.Text = m_CollectorPB.BoostPressureMaximum.ToString();
                //主变频器频率高限
                textBox_MasterInverterFrequencyMaximum.Text = m_CollectorPB.MasterInverterFrequencyMaximum.ToString();
                //主变频器功率低限
                textBox_MasterInverterPowerLowest.Text = m_CollectorPB.MasterInverterPowerLowest.ToString();
                //主电流高限
                textBox_MasterECMaximum.Text = m_CollectorPB.MasterECMaximum.ToString();
                //辅变频器频率高限
                textBox_SubInverterFrequencyMaximum.Text = m_CollectorPB.SubInverterFrequencyMaximum.ToString();
                //辅变频器功率低限
                textBox_SubInverterPowerLowest.Text = m_CollectorPB.SubInverterPowerLowest.ToString();
                //辅电流高限
                textBox_SubECMaximum.Text = m_CollectorPB.SubECMaximum.ToString();
                //定时轮换时间
                textBox_TimingRotationTime.Text = m_CollectorPB.TimingRotationTime.ToString();
                //出水压力表量程
                textBox_PressureFullscale.Text = m_CollectorPB.PressureFullscale.ToString();
                //液位低限
                textBox_LiquidlevelLowest.Text = m_CollectorPB.LiquidlevelLowest.ToString();
                //环境温度高限
                textBox_AmbientTempMaximum.Text = m_CollectorPB.AmbientTempMaximum.ToString();
                //环境湿度高限
                textBox_AmbientHumidityMaximum.Text = m_CollectorPB.AmbientHumidityMaximum.ToString();
                //进口压力阈值
                textBox_InletPressureThreshold.Text = m_CollectorPB.InletPressureThreshold.ToString();
                //进口(自来水)压力阈值,使用于箱式
                textBox_PipePressureThreshold.Text = m_CollectorPB.PipePressureThreshold.ToString();
                //出口压力阈值
                textBox_OutletPressureThreshold.Text = m_CollectorPB.OutletPressureThreshold.ToString();
                //主变频器频率阈值
                textBox_MasterFrequencyThreshold.Text = m_CollectorPB.MasterFrequencyThreshold.ToString();
                //主变频器功率阈值
                textBox_MasterPowerVThreshold.Text = m_CollectorPB.MasterPowerVThreshold.ToString();
                //主电流阈值
                textBox_MasterECThreshold.Text = m_CollectorPB.MasterECThreshold.ToString();
                //辅变频器频率阈值
                textBox_SubFrequencyThreshold.Text = m_CollectorPB.SubFrequencyThreshold.ToString();
                //辅变频器功率阈值
                textBox_SubPowerVThreshold.Text = m_CollectorPB.SubPowerVThreshold.ToString();
                //辅电流阈值
                textBox_SubECThreshold.Text = m_CollectorPB.SubECThreshold.ToString();
                //流量阈值
                textBox_FlowVThreshold.Text = m_CollectorPB.FlowVThreshold.ToString();
                //液位阈值
                textBox_LiquidlevelThreshold.Text = m_CollectorPB.LiquidlevelThreshold.ToString();
                //环境温度阈值
                textBox_AmbientTempThreshold.Text = m_CollectorPB.AmbientTempThreshold.ToString();
                //环境湿度阈值
                textBox_AmbientHumidityThreshold.Text = m_CollectorPB.AmbientHumidityThreshold.ToString();
                

            }
            catch (Exception ex)
            {
                Fn_DisplayClientSysAlarm("运行参数信息显示错误:[" + ex.Message + "]"); //MessageBox.Show("运行参数信息显示错误:[" + ex.Message + "]");
            }
        }


        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Exit_Click(object sender, EventArgs e)
        {
            //关闭串口
            if (m_SerialPort != null) m_SerialPort.Close();

            //保存参数
            SaveToFile();

            this.Close();
        }              

        /// <summary>
        /// 从界面中读取系统参数对象数据
        /// </summary>
        /// <returns></returns>
        private bool ReadSPBFromInterface()
        {
            bool m_rt = false;

            //获取对象数据
            try
            {
                //公共参数
                m_CollectorSPB.SoftVer = textBox_SoftVer.Text;

                m_CollectorSPB.TerminalID = textBox_TerminalID0.Text.Trim() + textBox_TerminalID.Text.Trim();

                m_CollectorSPB.TerminalName = textBox_TerminalName.Text;

                m_CollectorSPB.Longitude = double.Parse(textBox_Longitude.Text);
                m_CollectorSPB.Latitude = double.Parse(textBox_Latitude.Text);

                m_CollectorSPB.APN = (byte)comboBox_APN.SelectedIndex;
                m_CollectorSPB.CommunicationMode = (byte)comboBox_CommunicationMode.SelectedIndex;
                m_CollectorSPB.ProtocolType = (byte)comboBox_ProtocolType.SelectedIndex;

                m_CollectorSPB.UploadInterval = (byte)numericUpDown_UploadInterval.Value;
                m_CollectorSPB.UploadIimerlen = (byte)numericUpDown_UploadIimerlen.Value;
                m_CollectorSPB.ReUploadNumber = (byte)numericUpDown_ReUploadNumber.Value;
                m_CollectorSPB.ReUploadDelay = (byte)numericUpDown_ReUploadDelay.Value;

                //终端参数
                m_CollectorSPB.SysServerIP = textBox_SysServerIP.Text;
                m_CollectorSPB.SysPort = (int)numericUpDown_SysPort.Value;

                m_CollectorSPB.MyIP = textBox_MyIP.Text;
                m_CollectorSPB.MyMask = textBox_MyMask.Text;
                m_CollectorSPB.MyGate = textBox_MyGate.Text;
                m_CollectorSPB.MyDNS = textBox_MyDNS.Text;

                //应用参数
                m_CollectorSPB.R485AMode = (byte)comboBox_485AMode.SelectedIndex;
                m_CollectorSPB.R485ANo = (byte)numericUpDown_485ANo.Value;
                m_CollectorSPB.R485ABaudRate = (byte)comboBox_485ABaudRate.SelectedIndex;
                m_CollectorSPB.R485ADataBit = (byte)comboBox_485ADataBit.SelectedIndex;
                m_CollectorSPB.R485AStopBit = (byte)comboBox_485AStopBit.SelectedIndex;
                m_CollectorSPB.R485AParityBit = (byte)comboBox_485AParityBit.SelectedIndex;

                m_CollectorSPB.R485BMode = (byte)comboBox_485BMode.SelectedIndex;
                m_CollectorSPB.R485BNo = (byte)numericUpDown_485BNo.Value;
                m_CollectorSPB.R485BBaudRate = (byte)comboBox_485BBaudRate.SelectedIndex;
                m_CollectorSPB.R485BDataBit = (byte)comboBox_485BDataBit.SelectedIndex;
                m_CollectorSPB.R485BStopBit = (byte)comboBox_485BStopBit.SelectedIndex;
                m_CollectorSPB.R485BParityBit = (byte)comboBox_485BParityBit.SelectedIndex;

                m_CollectorSPB.TransactionQueryInterval = (ushort)numericUpDown_TransactionQueryInterval.Value;
                m_CollectorSPB.CollectInterval = (ushort)numericUpDown_CollectInterval.Value;
                m_CollectorSPB.CollectTimerLen = (byte)numericUpDown_CollectTimerLen.Value;

                //设备密码
                m_CollectorSPB.TerminalPassWord = textBox_TerminalPassWord.Text.Trim();
                //设备备用密码
                m_CollectorSPB.BackupPassword = textBox_BackupPassword.Text.Trim();

                m_CollectorSPB.TelePhones = textBox_TelePhones.Lines;

                //yth更改添加设备类型和选配
                //温湿度仪
                m_CollectorSPB.TemperatureHumidityInstrumentCFlag = checkBox_TemperatureHumidityInstrumentCFlag.Checked;
                //流量计
                m_CollectorSPB.FlowmeterCFlag = checkBox_FlowmeterCFlag.Checked;
                //开箱报警
                m_CollectorSPB.OpenBoxInstrumentCFlag = checkBox_OpenBoxInstrumentCFlag.Checked;
                //指纹阅读器
                m_CollectorSPB.FingerprintInstrumentCFlag = checkBox_FingerprintInstrumentCFlag.Checked;

                m_CollectorSPB.EquipmentType = (byte)comboBox_EquipmentType.SelectedIndex;
                m_rt = true;
            }
            catch
            {
                m_rt = false;
            }

            return m_rt;
        }

        /// <summary>
        /// 从界面读取运行参数对象
        /// </summary>
        /// <returns></returns>
        private bool ReadPBFromInterface()
        {
            bool m_rt = false;           
            
            //获取对象数据
            try
            {


                ////设置
                //m_CollectorPB.PumpPowerMax1 = double.Parse(textBox_PumpPowerMax1.Text.Trim());
                //m_CollectorPB.PumpPowerMax2 = double.Parse(textBox_PumpPowerMax2.Text.Trim());
                //m_CollectorPB.PumpPowerMax3 = double.Parse(textBox_PumpPowerMax3.Text.Trim());
                //m_CollectorPB.PumpPowerMax4 = double.Parse(textBox_PumpPowerMax4.Text.Trim());
                //m_CollectorPB.PumpPowerMax5 = double.Parse(textBox_PumpPowerMax5.Text.Trim());

                //m_CollectorPB.SettingPressure = double.Parse(textBox_SettingPressure.Text.Trim());
                //m_CollectorPB.InletPressureLowest = double.Parse(textBox_InletPressureLowest.Text.Trim());
                //m_CollectorPB.OutletPressureMaximum = double.Parse(textBox_OutletPressureMaximum.Text.Trim());
                ////m_CollectorPB.InverterFrequencyMaximum = double.Parse(textBox_InverterFrequencyMaximum.Text.Trim());
                ////m_CollectorPB.InverterPowerLowest = double.Parse(textBox_InverterPowerLowest.Text.Trim());
                //m_CollectorPB.TimingRotationTime = double.Parse(textBox_TimingRotationTime.Text.Trim());
                //m_CollectorPB.PressureFullscale = double.Parse(textBox_PressureFullscale.Text.Trim());

                //m_CollectorPB.InletPressureThreshold = double.Parse(textBox_InletPressureThreshold.Text.Trim());
                //m_CollectorPB.OutletPressureThreshold = double.Parse(textBox_OutletPressureThreshold.Text.Trim());
                ////m_CollectorPB.FrequencyThreshold = double.Parse(textBox_FrequencyThreshold.Text.Trim());
                ////m_CollectorPB.PowerVThreshold = double.Parse(textBox_PowerVThreshold.Text.Trim());
                //m_CollectorPB.FlowVThreshold = double.Parse(textBox_FlowVThreshold.Text.Trim());
                //m_CollectorPB.LiquidlevelThreshold = double.Parse(textBox_LiquidlevelThreshold.Text.Trim());
                 //yth更改2016/5/25
                m_CollectorPB.PumpPowerMax1 = double.Parse(textBox_PumpPowerMax1.Text.Trim());


                m_CollectorPB.PumpPowerMax2 = double.Parse(textBox_PumpPowerMax2.Text.Trim());


                m_CollectorPB.PumpPowerMax3 = double.Parse(textBox_PumpPowerMax3.Text.Trim());

                m_CollectorPB.PumpPowerMax4 = double.Parse(textBox_PumpPowerMax4.Text.Trim());


                m_CollectorPB.PumpPowerMax5 = double.Parse(textBox_PumpPowerMax5.Text.Trim());


                m_CollectorPB.PumpPowerMax6 = double.Parse(textBox_PumpPowerMax6.Text.Trim());


                m_CollectorPB.SettingPressure = double.Parse(textBox_SettingPressure.Text.Trim());


                m_CollectorPB.SettingBoostPressure = double.Parse(textBox_SettingBoostPressure.Text.Trim());


                m_CollectorPB.InletPressureLowest = double.Parse(textBox_InletPressureLowest.Text.Trim());


                m_CollectorPB.PipePressureLowest = double.Parse(textBox_PipePressureLowest.Text.Trim());


                m_CollectorPB.OutletPressureMaximum = double.Parse(textBox_OutletPressureMaximum.Text.Trim());


                m_CollectorPB.BoostPressureMaximum = double.Parse(textBox_BoostPressureMaximum.Text.Trim());


                m_CollectorPB.MasterInverterFrequencyMaximum = double.Parse(textBox_MasterInverterFrequencyMaximum.Text.Trim());


                m_CollectorPB.MasterInverterPowerLowest = double.Parse(textBox_MasterInverterPowerLowest.Text.Trim());


                m_CollectorPB.MasterECMaximum = double.Parse(textBox_MasterECMaximum.Text.Trim());


                m_CollectorPB.SubInverterFrequencyMaximum = double.Parse(textBox_SubInverterFrequencyMaximum.Text.Trim());


                m_CollectorPB.SubInverterPowerLowest = double.Parse(textBox_SubInverterPowerLowest.Text.Trim());


                m_CollectorPB.SubECMaximum = double.Parse(textBox_SubECMaximum.Text.Trim());


                m_CollectorPB.TimingRotationTime = double.Parse(textBox_TimingRotationTime.Text.Trim());


                m_CollectorPB.PressureFullscale = double.Parse(textBox_PressureFullscale.Text.Trim());


                m_CollectorPB.LiquidlevelLowest = double.Parse(textBox_LiquidlevelLowest.Text.Trim());


                m_CollectorPB.AmbientTempMaximum = double.Parse(textBox_AmbientTempMaximum.Text.Trim());


                m_CollectorPB.AmbientHumidityMaximum = double.Parse(textBox_AmbientHumidityMaximum.Text.Trim());


                m_CollectorPB.InletPressureThreshold = double.Parse(textBox_InletPressureThreshold.Text.Trim());


                m_CollectorPB.PipePressureThreshold = double.Parse(textBox_PipePressureThreshold.Text.Trim());


                m_CollectorPB.OutletPressureThreshold = double.Parse(textBox_OutletPressureThreshold.Text.Trim());


                m_CollectorPB.MasterFrequencyThreshold = double.Parse(textBox_MasterFrequencyThreshold.Text.Trim());


                m_CollectorPB.MasterPowerVThreshold = double.Parse(textBox_MasterPowerVThreshold.Text.Trim());


                m_CollectorPB.MasterECThreshold = double.Parse(textBox_MasterECThreshold.Text.Trim());


                m_CollectorPB.SubFrequencyThreshold = double.Parse(textBox_SubFrequencyThreshold.Text.Trim());


                m_CollectorPB.SubPowerVThreshold = double.Parse(textBox_SubPowerVThreshold.Text.Trim());


                m_CollectorPB.SubECThreshold = double.Parse(textBox_SubECThreshold.Text.Trim());


                m_CollectorPB.FlowVThreshold = double.Parse(textBox_FlowVThreshold.Text.Trim());


                m_CollectorPB.LiquidlevelThreshold = double.Parse(textBox_LiquidlevelThreshold.Text.Trim());


                m_CollectorPB.AmbientTempThreshold = double.Parse(textBox_AmbientTempThreshold.Text.Trim());


                m_CollectorPB.AmbientHumidityThreshold = double.Parse(textBox_AmbientHumidityThreshold.Text.Trim());
 
            
                m_rt = true;
            }
            catch
            {
                m_rt = false;
            }

            return m_rt;
        }

        /// <summary>
        /// 初始化系统参数设置界面
        /// </summary>
        private void InitialConfigSPBInterface()
        {
            try
            {
                //界面组件设置
                toolStripStatusLabel_State.Text = "系统参数设置";
                toolStripStatusLabel_State.Tag = 1;
                toolStripButton_Write.Enabled = true;
                toolStripButton_Read.Enabled = true;
                toolStripButton_Save.Enabled = true;
                toolStripButton_SetTerminalTime.Enabled = true;
                toolStripButton_GetTerminalTime.Enabled = true;

                panel_PB.Top = 1000;
                panel_StateData.Top = 1000;
                //yth更改添加
                panel_other2.Top = 1000;

                panel_SPB.Top = 66;

            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化系统参数错误:[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// 系统参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ConfigSPB_Click(object sender, EventArgs e)
        {
            InitialConfigSPBInterface();            

        }

        /// <summary>
        /// 运行参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ConfigPB_Click(object sender, EventArgs e)
        {
            try
            {
                //界面组件设置
                toolStripStatusLabel_State.Text = "运行参数设置";
                toolStripStatusLabel_State.Tag = 2;
                toolStripButton_Write.Enabled = true;
                toolStripButton_Read.Enabled = true;
                toolStripButton_Save.Enabled = true;
                toolStripButton_SetTerminalTime.Enabled = true;
                toolStripButton_GetTerminalTime.Enabled = true;

                panel_SPB.Top = 1000;
                panel_StateData.Top = 1000;
                //yth更改
                panel_other2.Top = 1000;
                panel_PB.Top = 66;                
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化运行参数错误:[" + ex.Message + "]");
            }
        }        

        /// <summary>
        /// 状态数据浏览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_BrowseStateData_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel_State.Text = "状态数据获取";
            toolStripStatusLabel_State.Tag = 3;
            toolStripButton_Write.Enabled = false;
            toolStripButton_Read.Enabled = false;
            toolStripButton_Save.Enabled = false;
            toolStripButton_SetTerminalTime.Enabled = false;
            toolStripButton_GetTerminalTime.Enabled = false;

            panel_SPB.Top = 1000;
            panel_PB.Top = 1000;
            //yth更改
            panel_other2.Top = 1000;
            panel_StateData.Top = 66;            
        }
         

        /// <summary>
        /// 系统初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Main_Load(object sender, EventArgs e)
        {
            InitialConfigSPBInterface();      
            
        }

        /// <summary>
        /// 向终端写入相应参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Write_Click(object sender, EventArgs e)
        {
            switch ((int)toolStripStatusLabel_State.Tag)
            { 
                case 1:
                    if (ReadSPBFromInterface())
                    {
                       
                        if (m_Client != null) m_Client.SendSystemPSP(null); //发送参数设置包
                    }
                    else
                        MessageBox.Show("读取系统参数失败！");

                    break;

                case 2:
                    if (ReadPBFromInterface())
                    {
                       // m_Client.CollectorPB = m_CollectorPB;
                        if (m_Client != null) m_Client.SendOperatingPSP(null);//发送运行参数包
                    }
                    else MessageBox.Show("读取运行参数失败！");

                    break;                
            }
        }

        /// <summary>
        /// 读取终端相应参数，向终端发送相应读取信息数据包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Read_Click(object sender, EventArgs e)
        {
            switch ((int)toolStripStatusLabel_State.Tag)
            {
                case 1:
                    if (m_Client != null) m_Client.SendSystemPAP(null);//发送读取系统参数包

                    break;
                case 2:
                    if (m_Client != null) m_Client.SendOperatingPAP(null);//发送读取运行参数包

                    break;                
            }

        }

        /// <summary>
        /// 保存当前数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            //读取参数信息
            ReadSPBFromInterface();
            ReadPBFromInterface();

            if (!SaveToFile())
            {
                MessageBox.Show("保存参数失败！");
            }
        }

        /// <summary>
        /// 设置串口通信参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Setup_Click(object sender, EventArgs e)
        {
            Form_Setup w_Setup = new Form_Setup();

            w_Setup.SysServerIP = m_SysServerIP;
            w_Setup.SysServerPort = m_SysServerPort;
            w_Setup.MPort = m_MPort;
            w_Setup.ProjectID = m_ProjectID;
            
            w_Setup.PortName = m_PortName;
            w_Setup.BaudRate = m_BaudRate;
            w_Setup.DataBits = m_DataBits;
            w_Setup.StopBits = m_StopBits;
            w_Setup.Parity = m_Parity;

            if (w_Setup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_SysServerIP = w_Setup.SysServerIP;
                m_SysServerPort = w_Setup.SysServerPort;
                m_MPort = w_Setup.MPort;
                m_ProjectID = w_Setup.ProjectID;

                m_CollectorSPB.SysServerIP = m_SysServerIP;
                m_CollectorSPB.SysPort = m_SysServerPort;

                m_PortName = w_Setup.PortName;
                m_BaudRate = w_Setup.BaudRate;
                m_DataBits = w_Setup.DataBits;
                m_StopBits = w_Setup.StopBits;
                m_Parity = w_Setup.Parity;

                //重新配置串口参数
                m_SerialPort.ConfigSerialPort(m_PortName, m_BaudRate, m_DataBits, m_StopBits, m_Parity);
                
                //显示串口状态
                toolStripStatusLabel_SerialMsg.Text = m_SerialPort.GetComPortConfig;

                //打开串口
                try
                {
                    m_SerialPort.Open();                    
                }
                catch
                {
                    MessageBox.Show("串口" + m_PortName + "打不开，请重新设置通信参数!");
                }


                OnDisplaySPB(m_CollectorSPB);
            }
        }

        /// <summary>
        /// 设置（同步）终端时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_SetTerminalTime_Click(object sender, EventArgs e)
        {
            if (m_Client != null)
            {
                if (!m_Client.SendTimeSP(null)) MessageBox.Show("设置终端时间失败！");
            }
        }

        /// <summary>
        /// 获取终端时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_GetTerminalTime_Click(object sender, EventArgs e)
        {
            if (m_Client != null)
            {
                if (!m_Client.SendDateTimeAP(null)) MessageBox.Show("获取终端时间失败！");
            }
        }

        

        /// <summary>
        /// 退出系统，关闭串口和消息处理循环
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!SaveToFile()) MessageBox.Show("保存参数失败！");
            if ((m_SerialPort != null) && (m_SerialPort.IsOpen)) m_SerialPort.Close();
            if (m_ClientMsgProcess != null) m_ClientMsgProcess.Stop();
        }  

        /// <summary>
        /// 自动更新采集器ID号类别码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TerminalID0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                byte buf;

                buf = (byte)((DateTime.Now.Year - 2010) * 4 + (Cls_Funs.WeekOfYear(DateTime.Now) / 16));

                textBox_TerminalID0.Text = m_ClassCode.ToString("X4") + buf.ToString("X2") + (Cls_Funs.WeekOfYear(DateTime.Now) % 16).ToString("X1");
            }
        }

        /// <summary>
        /// 配参注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Register_Click(object sender, EventArgs e)
        {
            if (m_Client != null)
            {
                if (!m_Client.SendPARegisterP(null)) MessageBox.Show("配参注册失败！");
            }
        }

        private void label31_Click(object sender, EventArgs e)
        {

        }
        //yth更改,其他选项的页面显示
        private void button1_Click(object sender, EventArgs e)
        {
            //界面组件设置
            toolStripStatusLabel_State.Text = "其他";
            toolStripStatusLabel_State.Tag = 4;
            toolStripButton_Write.Enabled = true;
            toolStripButton_Read.Enabled = true;
            toolStripButton_Save.Enabled = true;
            toolStripButton_SetTerminalTime.Enabled = true;
            toolStripButton_GetTerminalTime.Enabled = true;
            panel_SPB.Top = 1000;
            panel_StateData.Top = 1000;
            panel_other2.Top = 66;
            panel_PB.Top = 1000;       
        }

        //发送短信按钮
        private void button_send_message_Click(object sender, EventArgs e)
        {
            string telePhone = textBox_SMSnum.Text.Trim();
            string SMSContent = textBox_SMScontent.Text.Trim();
            byte sendType = 1;
            if (!m_Client.SendSMS(sendType, telePhone, SMSContent,null)) MessageBox.Show("短信发送失败！");
        }
        //自动设备启动
        private void button_control_auto_Click(object sender, EventArgs e)
        {   
            if(button_status_auto)
            {
            byte controlcmd=11;
            if (m_Client != null)
            {
                button_status_auto = false;
                button_control_auto.Text = "自动设备停止";
                if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("自动设备启动失败！");
            }

            }
            else
            {
                byte controlcmd = 12;
                if (m_Client != null)
                {
                    button_status_auto = true;
                    button_control_auto.Text = "自动设备启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("自动设备停止失败！");
                }
            }
        }
        //手动泵1启动
        private void button_control_pump1_Click(object sender, EventArgs e)
        {
            if (button_status_pump1)
            {
                byte controlcmd = 1;
                if (m_Client != null)
                {
                    button_status_pump1 = false;
                    button_control_pump1.Text = "手动泵1停止";

                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵1手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 2;
                if (m_Client != null)
                {
                    button_status_pump1 = true;
                    button_control_pump1.Text = "手动泵1启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵1手动停止失败！");
                }
            }
        }
        //手动泵2启动
        private void button_control_pump2_Click(object sender, EventArgs e)
        {
            if (button_status_pump2)
            {
                byte controlcmd = 3;
                if (m_Client != null)
                {
                    button_status_pump2 = false;
                    button_control_pump2.Text = "手动泵2停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵2手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 4;
                if (m_Client != null)
                {
                    button_status_pump2 = true;
                    button_control_pump2.Text = "手动泵2启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵2手动停止失败！");
                }
            }
        }
        //手动泵3启动
        private void button_control_pump3_Click(object sender, EventArgs e)
        {
            if (button_status_pump3)
            {
                byte controlcmd = 5;
                if (m_Client != null)
                {
                    button_status_pump3 = false;
                    button_control_pump3.Text = "手动泵3停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵3手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 6;
                if (m_Client != null)
                {
                    button_status_pump3 = true;
                    button_control_pump3.Text = "手动泵3启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵3手动停止失败！");
                }
            }
        }
        //手动泵4启动
        private void button_control_pump4_Click(object sender, EventArgs e)
        {
            if (button_status_pump4)
            {
                byte controlcmd = 7;
                if (m_Client != null)
                {
                    button_status_pump4 = false;
                    button_control_pump4.Text = "手动泵3停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵4手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 8;
                if (m_Client != null)
                {
                    button_status_pump4 = true;
                    button_control_pump4.Text = "手动泵3启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("泵4手动停止失败！");
                }
            }
        }
        //手动辅泵启动
        private void button_control_pump5_Click(object sender, EventArgs e)
        {
            if (button_status_pump5)
            {
                byte controlcmd = 9;
                if (m_Client != null)
                {
                    button_status_pump5 = false;
                    button_control_pump5.Text = "手动辅泵停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("辅泵手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 10;
                if (m_Client != null)
                {
                    button_status_pump5 = true;
                    button_control_pump5.Text = "手动辅泵启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("辅泵手动停止失败！");
                }
            }
        }
        //手动增压启动
        private void button_control_pump6_Click(object sender, EventArgs e)
        {
            if (button_status_pump6)
            {
                byte controlcmd = 13;
                if (m_Client != null)
                {
                    button_status_pump6 = false;
                    button_control_pump6.Text = "手动增压泵停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("增压泵手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 14;
                if (m_Client != null)
                {
                    button_status_pump6 = true;
                    button_control_pump6.Text = "手动增压泵启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("增压泵手动停止失败！");
                }
            }
        }
        //手动电动阀启动
        private void button_control_ele_Click(object sender, EventArgs e)
        {
            if (button_status_ele)
            {
                byte controlcmd = 15;
                if (m_Client != null)
                {
                    button_status_ele = false;
                    button_control_ele.Text = "手动电动阀停止";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("电动阀手动启动失败！");
                }

            }
            else
            {
                byte controlcmd = 16;
                if (m_Client != null)
                {
                    button_status_ele = true;
                    button_control_ele.Text = "手动电动阀启动";
                    if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("电动阀手动停止失败！");
                }
            }
        }
        //维持原状
        private void button_control_normal_Click(object sender, EventArgs e)
        {
            byte controlcmd = 0;
            if (m_Client != null)
            {
                if (!m_Client.SendRemoteControl(null, controlcmd)) MessageBox.Show("电动阀手动停止失败！");
            }
        }



        }
        
    }
