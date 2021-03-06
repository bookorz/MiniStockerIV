﻿using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SanwaMarco.Comm;
using MiniStockerIV.UI_Update;
using System.Configuration;
using System.Net;
using SanwaMarco;
using Microsoft.VisualBasic.PowerPacks;

namespace MiniStockerIV
{
    public partial class FormMain : Form, IConnectionReport
    {

        byte ODATA_0;
        //I7565DNM dnm = new I7565DNM("7");//測試用
        string version = "1.0.3";
        
        Boolean isCmdFin = true;
        Boolean isPause = false;
        public static Boolean isScriptRunning = false;
        Boolean autoMode = false;
        //private int dirIdx = 0;
        private int posIdx = 0;
        int intCmdTimeOut = 300000;//default 5 mins
        int ackTimeOut = 5000; // default 5 seconds
        int ackSleepTime = 200; // default 0.2 seconds
        string currentCmd = "";
        //for marco
        string macroName = "";
        string index = "";
        Boolean isAdmin = false;

        DateTime robotGetPutSTime;
        DateTime scriptSTime;

        public static int maxFloorNo = 5;
        Dictionary<string, string> error_codes = new Dictionary<string, string>();
        
        private void setIsRunning(Boolean isRun)
        {
            //isScriptRunning = isRun;
            FormMainUpdate.SetRunBtnEnable(isRun);
        }
        
        public FormMain()
        {
            InitializeComponent();

            this.tabSetting.Parent = null;//隱藏

            XmlConfigurator.Configure();//Log4N 需要
            InitCtrlGUI();
            //init marco
            Marco.ReceivedEventMessage += ReceviedEventMessage;
            Marco.ConnDevice();//連接設備
        }

        public void ReceviedEventMessage(object sender, string Message)
        {
            int cmdheadLength = Message.IndexOf(":");
            string cmdhead = Message.Substring(0, cmdheadLength);
            cmdheadLength = cmdhead.Length;
            cmdhead = cmdhead.Substring(2, cmdheadLength - 2);

            int cmdLength = Message.LastIndexOf(":");
            string cmd = Message.Substring(0, cmdLength);
            cmdLength = cmd.Length;
            cmd = cmd.Substring(Message.IndexOf(":") + 1, cmdLength - Message.IndexOf(":") - 1);

            int contentLength = Message.LastIndexOf(":");
            string content = Message.Substring(contentLength + 1);

            switch (cmdhead)
            {
                case "EVT":
                    switch (cmd)
                    {
                        case "PGSTS":

                            //$1EVT:PGSTS:15,0
                            string[] PGSTS;
                            PGSTS = content.Split(',');

                            FormMainUpdate.N2PurgeEnabledStatusUpdate(PGSTS[0], PGSTS[1]);
                            break;
                        case "O2STS":
                            string[] O2STS;
                            O2STS = content.Split(',');

                            FormMainUpdate.O2MonitorUpdate(O2STS[0]);
                            break;
                        case "N2LOW":
                            //"EVT:N2LOW:N2 Pressure is too low"
                            FormMainUpdate.RecevicedEvtUpdate(content);
                            break;

                        default:
                            FormMainUpdate.RecevicedEvtUpdate(content);
                            break;
                    }
                    break;
                case "FIN":
                    switch (cmd)
                    {
                        case "O2STS":
                            //$1FIN: O2STS: 1,00000000,0
                            string[] O2STS;
                            O2STS = content.Split(',');

                            FormMainUpdate.O2ValueUpdate(O2STS[2]);
                            break;
                        case "PGSTS":
                            //$1FIN: PGSTS: 1,00000000,36,0,0,0
                            //xxxxx: xxxxx: x,xxxxxxxx,stage,流量,開啟時間,啟用
                            string[] PGSTS;
                            PGSTS = content.Split(',');
                            //FormMainUpdate.N2PurgeStatusUpdate(plate, stage, PGSTS[3], PGSTS[5], PGSTS[4], false);
                            FormMainUpdate.N2PurgeGetStatusUpdate(PGSTS[2], PGSTS[3], PGSTS[4], PGSTS[5]);

                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }                                                                                                                                  
        }

        private void InitCtrlGUI()
        {
            //Name
            lblCtrlName1.Text = ConfigurationManager.AppSettings["Controller_1_Name"];
            lblCtrlName2.Text = ConfigurationManager.AppSettings["Controller_2_Name"];
            lblCtrlName3.Text = ConfigurationManager.AppSettings["Controller_3_Name"];
            lblCtrlName4.Text = ConfigurationManager.AppSettings["Controller_4_Name"];
            lblCtrlName5.Text = ConfigurationManager.AppSettings["Controller_5_Name"];
            //IP
            tbCtrl1_IP.Text = ConfigurationManager.AppSettings["Controller_1_IP"];
            tbCtrl2_IP.Text = ConfigurationManager.AppSettings["Controller_2_IP"];
            tbCtrl3_IP.Text = ConfigurationManager.AppSettings["Controller_3_IP"];
            tbCtrl4_IP.Text = ConfigurationManager.AppSettings["Controller_4_IP"];
            tbCtrl5_IP.Text = ConfigurationManager.AppSettings["Controller_5_IP"];
            //Port
            tbCtrl1_Port.Text = ConfigurationManager.AppSettings["Controller_1_Port"];
            tbCtrl2_Port.Text = ConfigurationManager.AppSettings["Controller_2_Port"];
            tbCtrl3_Port.Text = ConfigurationManager.AppSettings["Controller_3_Port"];
            tbCtrl4_Port.Text = ConfigurationManager.AppSettings["Controller_4_Port"];
            tbCtrl5_Port.Text = ConfigurationManager.AppSettings["Controller_5_Port"];

            //Disable unuse controller
            gbCtrl1.Enabled = !lblCtrlName1.Text.Equals("");
            gbCtrl2.Enabled = !lblCtrlName2.Text.Equals("");
            gbCtrl3.Enabled = !lblCtrlName3.Text.Equals("");
            gbCtrl4.Enabled = !lblCtrlName4.Text.Equals("");
            gbCtrl5.Enabled = !lblCtrlName5.Text.Equals("");

        }

        private void btnE1ReadID_Click(object sender, EventArgs e)
        {
            sendCommand(readRFID(Const.STK_ELPT1));
        }

        private string readRFID(string port)
        {
            //$1GET:CSTID:LPT[CR]
            //LPT：P1 = ELPT1 P2 = ELPT2
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1GET:CSTID:P1;";
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1GET:CSTID:P2;";
                    break;
            }
            return cmd;
        }


        private void sendCommands(Object obj)
        {
            ArrayList cmds = (ArrayList)obj;
            Command.oCmdScript.Clear();//clear script
            //create script
            foreach (String cmd in cmds)
            {
                FormMainUpdate.addScriptCmd(cmd);
            }
            FormMainUpdate.ChangeRunTab(5);
            FormMainUpdate.refreshScriptSet();
        }
        public void sendCommand(string cmd)
        {
            string deviceName = "";
            if (cmd.StartsWith("$1"))
            {
                deviceName = ConfigurationManager.AppSettings["Controller_1_Name"];
            }
            else if (cmd.StartsWith("$2"))
            {
                deviceName = ConfigurationManager.AppSettings["Controller_2_Name"];
            }
            else if (cmd.StartsWith("$3"))
            {
                deviceName = ConfigurationManager.AppSettings["Controller_3_Name"];
            }
            else if (cmd.StartsWith("$4"))
            {
                deviceName = ConfigurationManager.AppSettings["Controller_4_Name"];
            }
            else if (cmd.StartsWith("$5"))
            {
                deviceName = ConfigurationManager.AppSettings["Controller_5_Name"];
            }
            sendCommand(deviceName, cmd);
        }

        public void sendCommand(string deviceName, string cmd)
        {

            CtrlManager.controllers.TryGetValue(deviceName, out TcpCommClient device);

            if (device == null)
            {
                MessageBox.Show("無對應設備!!");
            }
            else if (cmd == null || cmd.Trim().Equals(""))
            {
                MessageBox.Show("無指令!!");
            }
            else
            {
                logUpdate(cmd);
                currentCmd = cmd;
                device.Send(cmd + "\r");
            }
        }

        private void connDevice(string device, DeviceConfig config)
        {
            // 暫時沒有連線
            config.Vendor = "SANWA";
            if (!IPAddress.TryParse(config.IPAdress, out IPAddress temp))
                return;//IP 格式錯誤, 不自動連線

            if (!CtrlManager.controllers.ContainsKey(device))
            {
                TcpCommClient ctrl = new TcpCommClient(config, this);
                ctrl.Start();
                CtrlManager.controllers.Add(device, ctrl);
            }
            else
            {
                TcpCommClient ctrl = CtrlManager.controllers[device];
                ctrl.Start();
            }
            

            //switch (device)
            //{
            //    case Const.CONTROLLER_STK:
            //        ctrlSTK = new TcpCommClient(config, this);
            //        ctrlSTK.Start();
            //        break;
            //    case Const.CONTROLLER_WHR:
            //        ctrlWHR = new TcpCommClient(config, this);
            //        ctrlWHR.Start();
            //        break;
            //    case Const.CONTROLLER_CTU_PTZ:
            //        ctrlCTU = new TcpCommClient(config, this);
            //        ctrlCTU.Start();
            //        break;
            //}
        }

        private void btnCtrlCon_Click(object sender, EventArgs e)
        {
            //決定設備
            string btnName = ((Button)sender).Name;
            string ip = "";
            int port = 0;
            string device_name = "";
            switch (btnName)
            {
                case "btnCtrl1Con":
                    ip = tbCtrl1_IP.Text;
                    int.TryParse(tbCtrl1_Port.Text, out port);
                    device_name = lblCtrlName1.Text;
                    break;
                case "btnCtrl2Con":
                    ip = tbCtrl2_IP.Text;
                    int.TryParse(tbCtrl2_Port.Text, out port);
                    device_name = lblCtrlName2.Text;
                    break;
                case "btnCtrl3Con":
                    ip = tbCtrl3_IP.Text;
                    int.TryParse(tbCtrl3_Port.Text, out port);
                    device_name = lblCtrlName3.Text;
                    break;
                case "btnCtrl4Con":
                    ip = tbCtrl4_IP.Text;
                    int.TryParse(tbCtrl4_Port.Text, out port);
                    device_name = lblCtrlName4.Text;
                    break;
                case "btnCtrl5Con":
                    ip = tbCtrl5_IP.Text;
                    int.TryParse(tbCtrl5_Port.Text, out port);
                    device_name = lblCtrlName5.Text;
                    break;
            }
            if (device_name.Equals(""))
                return;//未定義連線，不處理
            DeviceConfig config = new DeviceConfig();
            config.IPAdress = ip;
            config.Port = port;
            config.ConnectionType = "Socket";
            config.DeviceName = device_name;
            //logUpdate("----------" + device_name + "----------");
            connDevice(device_name, config);

        }

        Dictionary<string, int> mapCollection = new Dictionary<string, int>();
        void IConnectionReport.On_Connection_Message(object Msg)
        {
            string[] MsgAry = ((string)Msg).Split(new string[] { "\r" }, StringSplitOptions.None);
            foreach (string replyMsg in MsgAry)
            {
                string msg = replyMsg.Split('/')[0].Replace(";","").Replace("|ERROR","");
                string msgType = msg.Substring(2, 3);
                int lastIdx = msg.LastIndexOf(":") > 5 ? msg.LastIndexOf(":") : msg.Length;
                string func = msg.Substring(6, lastIdx - 6);// 前六碼為固定前修飾詞, 例如 $1CMD: $2MCR:
                //string errMsg = "";
                if(!replyMsg.Contains("RELIO"))
                    logUpdate("Receive <= " + replyMsg );//IO LOG 太多，不寫
                //Thread.Sleep(200);
                switch (msgType)
                {
                    case "NAK"://NAK 指令錯誤, 無法辨識
                        showError(replyMsg);
                        setIsRunning(false);//CAN  or  NAK stop script
                        isScriptRunning = false;
                        Marco.runMode = Marco.RunMode.Normal;
                        isCmdFin = true;
                        break;
                    case "CAN"://CAN 指令取消, 狀態不允許執行
                        showError(replyMsg);
                        setIsRunning(false);//CAN  or  NAK stop script
                        isScriptRunning = false;
                        Marco.runMode = Marco.RunMode.Normal;
                        isCmdFin = true;
                        break;
                    case "ABS"://動作異常結束
                        FormMainUpdate.AlarmUpdate(true);
                        showError(replyMsg);
                        setIsRunning(false);//ABS stop script
                        isScriptRunning = false;
                        Marco.runMode = Marco.RunMode.Normal;
                        isCmdFin = true;
                        switch (func)
                        {
                            case "CHECK_DNM_SLAVE":
                                logUpdate("Device Net Slave NG");
                                break;
                        }
                        break;
                    case "EVT"://事件發報
                        if (!isScriptRunning)
                        {
                            setIsRunning(false);
                        }
                        isCmdFin = true;
                        break;
                    case "ACK"://動作開始執行
                        setIsRunning(true);
                        isCmdFin = false;
                        break;
                    case "INF"://動作成功結束
                        //$1INF:ELPT_CLAMP/1/ON;
                        //string cmd = replyMsg.Substring(6, replyMsg.IndexOf("/") - 6);//取INF: 後到第一個/的資料
                        switch (func)
                        {
                            case "GET_FOUPS"://動作成功結束
                                setFoupPresenceByFoups(replyMsg);//UPDATE 畫面在席狀況
                                break;
                            case "GET_RELIO":
                                FormMainUpdate.IOUpdate(replyMsg);
                                break;
                            case "GET_NMEIO":
                                FormMainUpdate.IONameUpdate(replyMsg);
                                break;
                            case "CHECK_DNM_SLAVE":
                                logUpdate("Device Net Slave OK");
                                break;
                            case "ROBOT_GETPUT":
                                FormMainUpdate.RobotGetPutFinishUpdate(robotGetPutSTime);
                                break;
                            default:
                                break;

                        }
                        if (!isScriptRunning)
                        {
                            setIsRunning(false);
                        }
                        isCmdFin = true;
                        break;
                }
            }
        }

        public void showError(string msg)
        {
            //return;
            string factor = "";
            try
            {
                msg = msg.ToUpper();
                string msgType = msg.Substring(2, 3);
                int factorLen = 0;
                int startIdx = 0;
                switch (msgType)
                {
                    case "NAK":
                        //$1NAK:MSG|Message*;
                        factorLen = msg.IndexOf("|") - 6;
                        startIdx = 6;
                        break;
                    case "CAN":
                        //$1CAN:Message*|Factor/Place;
                        startIdx = msg.IndexOf("|") + 1;
                        factorLen = msg.LastIndexOf("/") > startIdx ? msg.LastIndexOf("/") - startIdx : msg.Length - startIdx -1;
                        break;
                    case "ABS":
                        //$1ABS:Message*|ERROR/Factor/Place;
                        //$1ABS:LOAD/Pnxx/ARM1|ERROR/CLAMPON/ARM1;
                        startIdx = msg.IndexOf("|ERROR/") + 7;
                        factorLen = msg.LastIndexOf("/") > startIdx ? msg.LastIndexOf("/") - startIdx : msg.Length - startIdx -1;//去掉;
                        break;
                }
                factor = msg.Substring(startIdx, factorLen);
                //Console.WriteLine(factor);
            }
            catch (Exception e)
            {
                logUpdate("異常資訊解析失敗" + e.StackTrace);
            }
            string desc = "未定義異常";
            string axis = "";
            if (factor.EndsWith("00") && factor.StartsWith("9"))
            {
                axis = factor.Substring(factor.Length - 3, 3);
                switch (axis)
                {
                    case "000":
                        axis = "R軸";
                        break;
                    case "100":
                        axis = "L軸";
                        break;
                    case "200":
                        axis = "S軸";
                        break;
                    case "300":
                        axis = "Z軸";
                        break;
                    case "400":
                        axis = "X軸";
                        break;
                    case "500":
                        axis = "R1軸";
                        break;
                    case "600":
                        axis = "L1軸";
                        break;
                    case "700":
                        axis = "S1軸";
                        break;
                    case "800":
                        axis = "Z1軸";
                        break;
                    case "900":
                        axis = "X1軸";
                        break;
                    case "A00":
                        axis = "R2軸";
                        break;
                    case "B00":
                        axis = "L2軸";
                        break;
                    case "C00":
                        axis = "S2軸";
                        break;
                    case "D00":
                        axis = "Z2軸";
                        break;
                    case "E00":
                        axis = "X2軸";
                        break;
                }
                factor = factor.Substring(0, factor.Length - 3) + "000";
            }
            error_codes.TryGetValue(factor.ToUpper(), out desc);
            desc = desc == null ? factor.ToUpper() : desc;
            logUpdate("異常描述: " + desc + axis);
        }

        void IConnectionReport.On_Connection_Connecting(string Msg)
        {
            logUpdate(Msg);
        }

        void IConnectionReport.On_Connection_Connected(object Msg)
        {
            logUpdate(Msg.ToString());
        }

        void IConnectionReport.On_Connection_Disconnected(string Msg)
        {
            logUpdate(Msg);
        }

        void IConnectionReport.On_Connection_Error(string Msg)
        {
            logUpdate(Msg);
        }

        private void btnE2ReadID_Click(object sender, EventArgs e)
        {
            sendCommand(readRFID(Const.STK_ELPT2));
        }

        private void btnE1Clamp_Click(object sender, EventArgs e)
        {
            string cmd = getELPTClamp(Const.STK_ELPT1, Const.STATUS_ON);
            sendCommand(cmd);
        }

        private string getELPTClamp(string port, string status)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:ELPT_CLAMP/P1;";//呼叫 Marco 指令:ELPT_CLAMP, 參數: P1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:ELPT_CLAMP/P2;";//呼叫 Marco 指令:ELPT_CLAMP, 參數: P2(ELPT2)
                    break;
            }
            return cmd;
        }
        private string getELPTUnClamp(string port, string status)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:ELPT_UNCLAMP/P1;";//呼叫 Marco 指令:ELPT_UNCLAMP, 參數: P1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:ELPT_UNCLAMP/P2;";//呼叫 Marco 指令:ELPT_UNCLAMP, 參數: P2(ELPT2)
                    break;
            }
            return cmd;
        }

        private void btnE2Clamp_Click(object sender, EventArgs e)
        {
            string cmd = getELPTClamp(Const.STK_ELPT2, Const.STATUS_ON);
            sendCommand(cmd);
        }

        private void btnE1UnClamp_Click(object sender, EventArgs e)
        {
            string cmd = getELPTUnClamp(Const.STK_ELPT1, Const.STATUS_OFF);
            sendCommand(cmd);
        }

        private void btnE2UnClamp_Click(object sender, EventArgs e)
        {
            string cmd = getELPTUnClamp(Const.STK_ELPT2, Const.STATUS_OFF);
            sendCommand(cmd);
        }

        /// <summary>
        ///     $1SET: SV___: NO,SW[CR] 電磁閥控制命令, SW: On => OPEN or CLAMP
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="sw">ON,OFF</param>
        private string STK_SET_SV(string unit, string sw)
        {
            /*
             * NO：
             *   17 = ELPT1 Clamp
             *   18 = ELPT2 Clamp
             *   19 = ELPT1 Shutter
             *   20 = ELPT2 Shutter
             * SW：
             *   0 = Close(Unclamp)
             *   1 = Open(Clamp)
             */
            string cmd = "";
            switch (unit)
            {
                case Const.SV_STK_ELPT1_CLAMP:
                    //cmd = "$1SET:SV___:17," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    cmd = "$1MCR:ELCLP:1,1," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    break;
                case Const.SV_STK_ELPT2_CLAMP:
                    //cmd = "$1SET:SV___:18," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    cmd = "$1MCR:ELCLP:2,2," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    break;
                case Const.SV_STK_ELPT1_SHUTTER:
                    //cmd = "$1SET:SV___:19," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    cmd = "$1MCR:ELSTR:1,1," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    break;
                case Const.SV_STK_ELPT2_SHUTTER:
                    //cmd = "$1SET:SV___:20," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    cmd = "$1MCR:ELSTR:2,2," + (sw.Equals(Const.SV_STATUS_ON) ? "1" : "0");
                    break;
            }
            return cmd;
        }

        private void btnE1OpenShutter_Click(object sender, EventArgs e)
        {
            string cmd = getELPTShutter(Const.STK_ELPT1, Const.STATUS_OPEN);
            sendCommand(cmd);
        }

        /// <summary>
        /// $1MCR:ELPT_SHUTTER/LTP/STATUS[CR]
        /// LPT：    
        ///     1 = ELPT1
        ///     2 = ELPT2
        /// STATUS：    
        ///      OPEN
        ///     CLOSE
        /// </summary>
        /// <param name="port">指定 port</param>
        /// <param name="status">clamp 狀態</param>
        /// <returns></returns>
        private string getELPTShutter(string port, string status)
        {
            string cmd = "";
            string marco_name = "";
            switch (status)
            {
                case Const.STATUS_OPEN:
                    marco_name = "ELPT_SHUTTER_OPEN";//OPEN Shutter
                    break;
                case Const.STATUS_CLOSE:
                    marco_name = "ELPT_SHUTTER_CLOSE";//OPEN Shutter
                    break;
            }
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:" + marco_name + "/P1;";//呼叫 Marco 指令, 參數: P1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:" + marco_name + "/P2;";//呼叫 Marco 指令, 參數: P2(ELPT2)
                    break;
            }
            return cmd;
        }

        private void btnE1CloseShutter_Click(object sender, EventArgs e)
        {
            string cmd = getELPTShutter(Const.STK_ELPT1, Const.STATUS_CLOSE);
            sendCommand(cmd);
        }

        private void btnE2OpenShutter_Click(object sender, EventArgs e)
        {
            string cmd = getELPTShutter(Const.STK_ELPT2, Const.STATUS_OPEN);
            sendCommand(cmd);
        }

        private void btnE2CloseShutter_Click(object sender, EventArgs e)
        {
            string cmd = getELPTShutter(Const.STK_ELPT2, Const.STATUS_CLOSE);
            sendCommand(cmd);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string msg = "";
            //Check Key Pro
            if(!Util.checkKeyPro( ref msg))
            {
                MessageBox.Show(msg,"Notice",MessageBoxButtons.OK,MessageBoxIcon.Error);
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("USB Key Owner: " + msg, "Welcom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string mctype = "";

                if (Marco.machineType == Marco.MachineType.Normal)
                {
                    mctype = "Mini Stocker Test Utility(18 port)";
                }
                else
                {
                    mctype = "Mini Stocker Test Utility(26 port)";
                }

                this.Text = mctype + " (Version: " + version + ") User: " + msg;
            }
            



            //GUI 訊息處理器
            GUICmdCtrl Comm = new GUICmdCtrl();
            Comm.Start();

            //連線
            //btnCtrl1Con_Click(sender, e);
            //btnCtrlWHRCon_Click(sender, e);
            //btnCtrlCTUCon_Click(sender, e);
            //自動連線
            btnCtrlCon_Click(btnCtrl1Con, e);
            btnCtrlCon_Click(btnCtrl2Con, e);
            btnCtrlCon_Click(btnCtrl3Con, e);
            btnCtrlCon_Click(btnCtrl4Con, e);
            btnCtrlCon_Click(btnCtrl5Con, e);
            tabMode.SelectedIndex = 2;
            //Initial_I_O();
            Initial_Error();
            Initial_Command();

            cbSource.Items.Clear();
            cbDestination.Items.Clear();
            cmbN2PurgeNo.Items.Clear();

            //介面更新
            string[] positions;
            if (Marco.machineType == Marco.MachineType.Normal)
            {
                positions = new string[] { "ELPT1","ELPT2","ILPT1","ILPT2",
                "SHELF1-1","SHELF1-2", "SHELF1-3", "SHELF1-4", "SHELF1-5",
                "SHELF2-1","SHELF2-2", "SHELF2-3", "SHELF2-4", "SHELF2-5",
                "SHELF3-1","SHELF3-2", "SHELF3-3", "SHELF3-4",
                "SHELF4-1","SHELF4-2", "SHELF4-3", "SHELF4-4"};

                pn18PortFoupPresence.Visible = true;
                pn26PortFoupPresence.Visible = false;

                tabStocker2.Parent = null;

                maxFloorNo = 5;
            }
            else
            {
                positions = new string[] { "ELPT1","ELPT2","ILPT1","ILPT2",
                "SHELF1-1","SHELF1-2", "SHELF1-3", "SHELF1-4", "SHELF1-5", "SHELF1-6","SHELF1-7",
                "SHELF2-1","SHELF2-2", "SHELF2-3", "SHELF2-4", "SHELF2-5", "SHELF2-6","SHELF2-7",
                "SHELF3-1","SHELF3-2", "SHELF3-3", "SHELF3-4", "SHELF3-5", "SHELF3-6",
                "SHELF4-1","SHELF4-2", "SHELF4-3", "SHELF4-4", "SHELF4-5", "SHELF4-6"};

                pn18PortFoupPresence.Visible = false;
                pn26PortFoupPresence.Visible = true;

                pn26PortFoupPresence.Location = new Point(pn18PortFoupPresence.Left, pn18PortFoupPresence.Top);

                maxFloorNo = 7;
            }

            foreach (string itemname in positions)
            {
                cbSource.Items.Add(itemname);
                cbDestination.Items.Add(itemname);

                if (itemname.Contains("SHELF"))
                {
                    cmbN2PurgeNo.Items.Add(itemname);
                }
            }

            cmbN2PurgeNo.SelectedIndex = 0;
            cmbN2BlowTime.SelectedIndex = 0;
            tbN2FlowControl.Text = tkbrN2FlowControl.Value.ToString();

            //Add IO From
            FormIO form = new FormIO();
            foreach (Control foo in pnlIO.Controls)
            {
                pnlIO.Controls.Remove(foo);
                foo.Dispose();
            }
            form.TopLevel = false;
            form.AutoScroll = true;
            pnlIO.Controls.Add(form);
            form.Show();
            ThreadPool.QueueUserWorkItem(new WaitCallback(runScript));// Script 執行續建立

            timerScript.Enabled = true;
        }

        /// <summary>
        /// $1MCR:ELPT_MOVE/LTP/STATUS[CR]
        /// </summary>
        /// LPT：    
        ///     1 = ELPT1
        ///     2 = ELPT2
        /// STATUS：    
        ///      IN:Stock in
        ///     OUT:Stock Out
        /// <param name="port"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string getELPTMove(string port, string status)
        {
            string marco = "";
            string cmd = "";
            switch (status)
            {
                case Const.POSITION_ELPT_STOCK_IN:
                    marco = "ELPT_MOVEIN";//Move in stocker
                    break;
                case Const.POSITION_ELPT_STOCK_OUT:
                    marco = "ELPT_MOVEOUT";//Move Out stocker
                    break;
            }
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:" + marco + "/P1;";//呼叫 Marco 指令:ELPT_MOVE, 參數: 1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:" + marco + "/P2;";//呼叫 Marco 指令:ELPT_MOVE, 參數: 2(ELPT2)
                    break;
            }
            return cmd;
        }

        private void btnE1MoveIn_Click(object sender, EventArgs e)
        {
            string cmd = getELPTMove(Const.STK_ELPT1, Const.POSITION_ELPT_STOCK_IN);
            sendCommand(cmd);
        }
        
        private void btnE1MoveOut_Click(object sender, EventArgs e)
        {
            string cmd = getELPTMove(Const.STK_ELPT1, Const.POSITION_ELPT_STOCK_OUT);
            sendCommand(cmd);
        }

        private void btnE2MoveIn_Click(object sender, EventArgs e)
        {
            string cmd = getELPTMove(Const.STK_ELPT2, Const.POSITION_ELPT_STOCK_IN);
            sendCommand(cmd);
        }

        private void btnE2MoveOut_Click(object sender, EventArgs e)
        {
            string cmd = getELPTMove(Const.STK_ELPT2, Const.POSITION_ELPT_STOCK_OUT);
            sendCommand(cmd);
        }

        

        private void btnI1Load_Click(object sender, EventArgs e)
        {
            string cmd = ILPT_Load(Const.STK_ILPT1);
            sendCommand(cmd);
        }

        private void btnI2Load_Click(object sender, EventArgs e)
        {
            string cmd = ILPT_Load(Const.STK_ILPT2);
            sendCommand(cmd);
        }

        /// <summary>
        /// $1MCR:ILPT_OPEN/LTP[CR]
        /// LPT：    
        ///     1 = ILPT1
        ///     2 = ILPT2
        /// </summary>
        /// <param name="port"></param>
        private string ILPT_Load(string port)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ILPT1:
                    cmd = "$1MCR:ILPT_LOAD/P3;";//呼叫 Marco 指令:ILPT_LOAD, 參數: 1(ILPT1)
                    break;
                case Const.STK_ILPT2:
                    cmd = "$1MCR:ILPT_LOAD/P4;";//呼叫 Marco 指令:ILPT_LOAD, 參數: 2(ILPT2)
                    break;
            }
            return cmd;
        }

        /// <summary>
        /// $1MCR:ILPT_CLOSE:LTP[CR]
        /// LPT：    
        ///     1 = ILPT1
        ///     2 = ILPT2
        /// </summary>
        /// <param name="port"></param>
        private string ILPT_Unload(string port)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ILPT1:
                    cmd = "$1MCR:ILPT_UNLOAD/P3;";//呼叫 Marco 指令:ILPT_UNLOAD, 參數: 1(ILPT1)
                    break;
                case Const.STK_ILPT2:
                    cmd = "$1MCR:ILPT_UNLOAD/P4;";//呼叫 Marco 指令:ILPT_UNLOAD, 參數: 2(ILPT2)
                    break;
            }
            return cmd;
        }

        private void btnI1UnLoad_Click(object sender, EventArgs e)
        {
            string cmd = ILPT_Unload(Const.STK_ILPT1);
            sendCommand(cmd);
        }

        private void btnI2UnLoad_Click(object sender, EventArgs e)
        {
            string cmd = ILPT_Unload(Const.STK_ILPT2);
            sendCommand(cmd);
        }
        

        //private void autoRunELPT(string portName)
        //{
        //    readRFID(portName);
        //    this.s
        //}

        private void btnE1Init_Click(object sender, EventArgs e)
        {
            ELPT_INIT(Const.STK_ELPT1);
        }

        private void ELPT_INIT(string port)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    //cmd = "$1MCR:MEORG:1,6";//$1MCR:MEORG:MC,MO
                    cmd = "$1MCR:ELINI:1,1"; //$1MCR: ELINI: MC,MO
                    break;
                case Const.STK_ELPT2:
                    //cmd = "$1MCR:MEORG:2,7";//$1MCR:MEORG:MC,MO
                    cmd = "$1MCR:ELINI:2,2"; //$1MCR: ELINI: MC,MO
                    break;
            }
            sendCommand(cmd);
        }

        private void ILPT_INIT(string port)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ILPT1:
                    cmd = "$1MCR:ILINI:3,1"; //$1MCR:ILINI:MC,LTP
                    break;
                case Const.STK_ILPT2:
                    cmd = "$1MCR:ILINI:4,2"; //$1MCR:ILINI:MC,LTP
                    break;
            }
            sendCommand(cmd);
        }

        private void btnE2Init_Click(object sender, EventArgs e)
        {
            ELPT_INIT(Const.STK_ELPT2);
        }

        private void tbI1Init_Click(object sender, EventArgs e)
        {
            ILPT_INIT(Const.STK_ILPT1);
        }

        private void tbI2Init_Click(object sender, EventArgs e)
        {
            ILPT_INIT(Const.STK_ILPT2);
        }

        private void btnE1Reset_Click(object sender, EventArgs e)
        {
            ResetController(Const.CONTROLLER_STK);
        }

        private void btnE2Reset_Click(object sender, EventArgs e)
        {
            ResetController(Const.CONTROLLER_STK);
        }

        private void tbI1Reset_Click(object sender, EventArgs e)
        {
            ResetController(Const.CONTROLLER_STK);
        }

        private void tbI2Reset_Click(object sender, EventArgs e)
        {
            ResetController(Const.CONTROLLER_STK);
        }

        /// <summary>
        /// MC：Macro Container(Always 5)
        /// STN：
        /// ALL = ALL
        /// ROB = Robot Arm
        /// P1 = ELPT1
        /// P2 = ELPT2
        /// P3 = ILPT1
        /// P4 = ILPT2
        /// 6~21 = SHELF1 ~SHLEF16
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSTKRefresh_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("查詢Stocker 在席!");
            //string cmd = "$1GET:BRDIO:20,1,05";
            string cmd = "$1MCR:GET_FOUPS/ALL;";
            sendCommand(cmd);
            //測試用
            //setFoupPresenceByBoard("$1ACK:BRDIO:20,1,5,00143,00241,00128,00055,00155");
            //setFoupPresenceByFoups("$1FIN:MCR__:5,00000000,1,2,1,2,1,0,0,0,1,0,0,0,1,2,1,0,0,0,1,2,1");
            //setFoupPresenceByFoups("$1FIN:MCR__:5,00000000,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1");
        }
        //$1EVT:PGSTS:15,0
        private void receivedMFCStopEVT(string msg)
        {
            string[] receivedMsg;
            receivedMsg = msg.Split(':');
            //receivedMsg[0] = $1EVT
            //receivedMsg[1] = PGSTS
            //receivedMsg[2] = 15,0

            string[] message;
            message = receivedMsg[2].Split(',');

            string areaID = message[0].Substring(0, 1);

            string plateID = message[0].Substring(1, 1);

            FormMainUpdate.N2PurgeStatusUpdate(Convert.ToInt32(plateID), Convert.ToInt32(areaID), "0", "0", "0", false);

        }
        //"指定 STAGE
        //1. RSLT
        //2. PLATE 
        //3. STAGE
        //4. MFC
        //  目前流量設定
        //5. STS 0: 關閉, 1: 開啟
        //6. TIME 已開啟時間(ms)
        /// xxxxxxxxxxxx/$1FIN:MCR__:00000000,PLATE,STAGE,MFC,STS,TIME
        private void setN2PurgeStatus(string msg)
        {
            string results = msg.Substring(msg.LastIndexOf('/') + 1);
            results = msg.Substring(msg.LastIndexOf(':') + 1);

            //異常狀況不變化
            if (!results.Contains("00000000")) return;

            //將00000000,過濾掉
            results = msg.Substring(msg.IndexOf(',') + 1);
            results = results.Substring(0, results.Length - 1);//去掉結尾的 ;

            string[] rsltPresence;
            rsltPresence = results.Split(',');

            int floor; //機架的編號與N2 Purge的編號是顛倒的
            int plate;
            int stage;

            if (rsltPresence.Length == 5)   //僅回傳單一站資訊
            {
                if (!int.TryParse(rsltPresence[0], out plate))  return;
                if (!int.TryParse(rsltPresence[1], out stage))  return;

                floor = maxFloorNo - plate + 1;

                //1.Floor
                //2.STAGE
                //3.MFC
                //4.STS
                //5.TIME
                //6.SET(或者是GET)
                FormMainUpdate.N2PurgeStatusUpdate(floor, stage,
                                rsltPresence[2].Trim(), rsltPresence[3].Trim(), rsltPresence[4].Trim(), false);
            }
            else// 回傳所有站資訊
            {
                //0.PLATE
                //1.MFC
                //2.STS-N-1
                //3.TIME-N-1
                //4.STS-N-2
                //5.TIME-N- 2
                //6.STS-N-3
                //7.TIME-N-3
                //8.STS-N-4
                //9.TIME-N-4

                if (!int.TryParse(rsltPresence[0], out plate)) return;
                floor = maxFloorNo - plate + 1;

                string mfc = rsltPresence[1].Trim();

                for (int i = 0; i<4; i++)
                {
                    //最高層只有兩區
                    if (floor >= maxFloorNo && i> 2) break;

                    FormMainUpdate.N2PurgeStatusUpdate(floor,  i+1,
                                    mfc, rsltPresence[(i + 1)*2].Trim(), rsltPresence[(i + 1)* 2 + 1].Trim(), false);
                }
                
            }

        }

        /// <summary>
        /// 0 = ALL        /// 1 = Robot Arm    /// 2 = ELPT1
        /// 3 = ELPT2      /// 4 = ILPT1        /// 5 = ILPT2
        /// 6~21 = SHELF1 ~SHLEF16
        ///old $1FIN:MCR__:5,00000000,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1
        /// xxxxxxxxxxxx/1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1
        private void setFoupPresenceByFoups(string msg)
        {
            int ItemCount;
            string[] rsltPresence;

            if (Marco.machineType == Marco.MachineType.Normal)
            {
                ItemCount = 23;
            }
            else
            {
                ItemCount = 31;
            }

            rsltPresence = new string[ItemCount];

            string results = msg.Substring(msg.LastIndexOf('/') + 1);
            results = results.Substring(0, results.Length - 1);//去掉結尾的 ;

            if (results.Length != ItemCount)
                return;
            int idx = 0;
            for (int i = 0; i < results.Length; i++, idx++)
            {
                rsltPresence[idx] = results.Substring(i,1);
            }
            FormMainUpdate.updateFoupPresenceByFoups(rsltPresence);
        }

        private void btnFoupRotSwitch_Click(object sender, EventArgs e)
        {
            int srcIdx = cbSource.SelectedIndex;
            int destIdx = cbDestination.SelectedIndex;
            cbSource.SelectedIndex = destIdx;
            cbDestination.SelectedIndex = srcIdx;
        }

        private void btnFoupRotGetWait_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_GetW(cbSource.Text);
                sendCommand(cmd);
            }
        }

        private Boolean checkFoupRobotSrc()
        {
            if (cbSource.Text.Equals(""))
            {
                MessageBox.Show("請選擇 Source!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean checkFoupRobotDest()
        {
            if (cbDestination.Text.Equals(""))
            {
                MessageBox.Show("請選擇 Destination!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean checkN2Purge()
        {
            if (cmbN2PurgeNo.Text.Equals(""))
            {
                MessageBox.Show("請選擇 N2 Purge No.!!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean checkN2BlowTime()
        {
            if (cmbN2BlowTime.Text.Equals(""))
            {
                MessageBox.Show("請選擇 N2 Blow Time!!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean checkO2Threshold()
        {
            if (tbO2ThresholdLow.Text.Equals(""))
            {
                MessageBox.Show("輸入 O2 門檻值!!");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// PNO：P1 = ELPT1, P2 = ELPT2, P3 = ILPT1, P4 = ILPT2
        ///     BF11 = SHELF1, BF12 = SHELF2, BF13 = SHELF3, BF21 = SHELF4, BF31 = SHELF5
        ///     BF22 = SHELF6, BF33 = SHELF7, BF41 = SHELF8, BF42 = SHELF9, BF43 = SHELF10
        ///     BF51 = SHELF11, BF52 = SHELF12, BF53 = SHELF13, BF61 = SHELF14, BF62 = SHELF15,BF63 = SHELF16
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string STK_GET_POSITION(string name)
        {
            string result = "";
            switch (name)
            {
                case Const.STK_ELPT1:
                    result = "P1";
                    break;
                case Const.STK_ELPT2:
                    result = "P2";
                    break;
                case Const.STK_ILPT1:
                    result = "P3";
                    break;
                case Const.STK_ILPT2:
                    result = "P4";
                    break;
                case Const.STK_SHELF1_1:
                    result = "BF11";
                    break;
                case Const.STK_SHELF1_2:
                    result = "BF12";
                    break;
                case Const.STK_SHELF1_3:
                    result = "BF13";
                    break;
                case Const.STK_SHELF1_4:
                    result = "BF14";
                    break;
                case Const.STK_SHELF1_5:
                    result = "BF15";
                    break;
                case Const.STK_SHELF1_6:
                    result = "BF16";
                    break;
                case Const.STK_SHELF1_7:
                    result = "BF17";
                    break;
                case Const.STK_SHELF2_1:
                    result = "BF21";
                    break;
                case Const.STK_SHELF2_2:
                    result = "BF22";
                    break;
                case Const.STK_SHELF2_3:
                    result = "BF23";
                    break;
                case Const.STK_SHELF2_4:
                    result = "BF24";
                    break;
                case Const.STK_SHELF2_5:
                    result = "BF25";
                    break;
                case Const.STK_SHELF2_6:
                    result = "BF26";
                    break;
                case Const.STK_SHELF2_7:
                    result = "BF27";
                    break;
                case Const.STK_SHELF3_1:
                    result = "BF31";
                    break;
                case Const.STK_SHELF3_2:
                    result = "BF32";
                    break;
                case Const.STK_SHELF3_3:
                    result = "BF33";
                    break;
                case Const.STK_SHELF3_4:
                    result = "BF34";
                    break;
                case Const.STK_SHELF3_5:
                    result = "BF35";
                    break;
                case Const.STK_SHELF3_6:
                    result = "BF36";
                    break;
                case Const.STK_SHELF4_1:
                    result = "BF41";
                    break;
                case Const.STK_SHELF4_2:
                    result = "BF42";
                    break;
                case Const.STK_SHELF4_3:
                    result = "BF43";
                    break;
                case Const.STK_SHELF4_4:
                    result = "BF44";
                    break;
                case Const.STK_SHELF4_5:
                    result = "BF45";
                    break;
                case Const.STK_SHELF4_6:
                    result = "BF46";
                    break;
                    //case Const.STK_SHELF5_1:
                    //    result = "BF51";
                    //    break;
                    //case Const.STK_SHELF5_2:
                    //    result = "BF52";
                    //    break;
                    //case Const.STK_SHELF5_3:
                    //    result = "BF53";
                    //    break;
                    //case Const.STK_SHELF6_1:
                    //    result = "BF61";
                    //    break;
                    //case Const.STK_SHELF6_2:
                    //    result = "BF62";
                    //    break;
                    //case Const.STK_SHELF6_3:
                    //    result = "BF63";
                    //    break;
            }
            return result;
        }

        /// <summary>
        /// (最新版) $1MCR:ROBOT_GETW/POINT[CR]
        /// </summary>
        /// <param name="source"></param>
        private string FoupRobot_GetW(string source)
        {
            string cmd = "";
            string position = STK_GET_POSITION(source);
            if (position != null && !position.Trim().Equals(""))
            {
                cmd = "$1MCR:ROBOT_GETW/" + position + ";";
            }
            return cmd;
        }

        private void btnFoupRotGet_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_Get(cbSource.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (最新命令)$1MCR:ROBOT_GET/POINT[CR]
        /// </summary>
        /// <param name="source"></param>
        private string FoupRobot_Get(string source)
        {
            string cmd = "";
            string position = STK_GET_POSITION(source);
            if (position != null && !position.Trim().Equals(""))
            {
                cmd = "$1MCR:ROBOT_GET/" + position + ";";
            }
            return cmd;
        }

        private void btnFoupRotPutWait_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_PutW(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (最新命令)$1MCR:ROBOT_PUTW:POSITION[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_PutW(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                cmd = "$1MCR:ROBOT_PUTW/" + position + ";";
            }
            return cmd;
        }

        private void btnFoupRotPut_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_Put(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (最新命令)$1MCR:ROBOT_PUT:POSITION[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Put(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                cmd = "$1MCR:ROBOT_PUT/" + position + ";";
            }
            return cmd;
        }

        private void btnFoupRotExtendSrc_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_Extend_Src(cbSource.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新命令)$1MCR:RBETD:MC,PNO,ZPOS[CR]
        /// MC：Macro Container(Always 0)
        /// PNO: same as PreparePick
        /// ZPOS : Z Position
        /// 0 : Bottom(Get)
        /// 1 : Top(Put)
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Extend_Src(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD: GETST:" + position + ",001,1,2";
                cmd = "$1MCR:RBETD:0," + position + ",0";
            }
            return cmd;
        }

        private void btnFoupRotUpSrc_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_Up_Src(cbSource.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新指令)$1MCR:RBUP_:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Up_Src(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD: GETST: " + position + ",001,1,5";
                cmd = "$1MCR:RBUP_:0";
            }
            return cmd;
        }

        private void btnFoupRotGrab_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_Grab_Src(cbSource.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新命令)$1CMD:WHLD_:1,0[CR]
        /// (最新指令)$1MCR:RBHLD:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Grab_Src(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD:GETST:" + position + ",001,1,4";
                //cmd = "$1CMD:WHLD_:1,0";
                cmd = "$1MCR:RBHLD:0";
            }
            return cmd;
        }

        private void btnFoupRotRetractSrc_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc())
            {
                string cmd = FoupRobot_Retract_Src(cbSource.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新指令)$1CMD:RET__[CR]
        /// (最新命令)$1MCR:RET__:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Retract_Src(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD: GETST:" + position + ",001,1,0";
                //cmd = "$1CMD:RET__";
                cmd = "$1MCR:RET__:0";
            }
            return cmd;
        }

        private void btnFoupRotExtendDest_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_Extend_Dest(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新命令)$1MCR:RBETD:MC,PNO,ZPOS[CR]
        /// MC：Macro Container(Always 0)
        /// PNO: same as PreparePick
        /// ZPOS : Z Position
        /// 0 : Bottom(Get)
        /// 1 : Top(Put)
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Extend_Dest(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD:PUTST:" + position + ",001,1,2";
                cmd = "$1MCR:RBETD:0," + position + ",1";
            }
            return cmd;
        }

        private void btnFoupRotRelease_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_Release_Dest(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新指令)$1CMD:WRLS_:1,0[CR]
        /// (最新指令)$1MCR:RBRLS:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Release_Dest(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD:PUTST:" + position + ",001,1,4";
                //cmd = "$1CMD:WRLS_:1,0";
                cmd = "$1MCR:RBRLS:0";
            }
            return cmd;
        }

        private void btnFoupRotRetractDest_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_Retract_Dest(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新指令)$1CMD:RET__[CR]
        /// (最新指令)$1MCR:RET__:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Retract_Dest(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD:PUTST:" + position + ",001,1,0";
                //cmd = "$1CMD:RET__";
                cmd = "$1MCR:RET__:0";
            }
            return cmd;
        }

        private void btnFoupRotDownDest_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotDest())
            {
                string cmd = FoupRobot_Down_Dest(cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (舊命令)$1CMD:PUTST:PNO,001,1,0[CR]
        /// (新命令)$1MCR:RBDWN:MC[CR]
        /// </summary>
        /// <param name="dest"></param>
        private string FoupRobot_Down_Dest(string dest)
        {
            string cmd = "";
            string position = STK_GET_POSITION(dest);
            if (position != null && !position.Trim().Equals(""))
            {
                //cmd = "$1CMD:PUTST:" + position + ",001,1,5";
                cmd = "$1MCR:RBDWN:0";
            }
            return cmd;
        }

        private void btnE1Auto_Click(object sender, EventArgs e)
        {
            showAutoDialog();
            ArrayList cmds = new ArrayList();
            cmds.Add(readRFID(Const.STK_ELPT1));//read rfid
            //load
            cmds.Add(getELPTMove(Const.STK_ELPT1, Const.POSITION_ELPT_STOCK_IN));//move in
            //unload
            cmds.Add(getELPTMove(Const.STK_ELPT1, Const.POSITION_ELPT_STOCK_OUT));//move out  
            ThreadPool.QueueUserWorkItem(new WaitCallback(sendCommands), cmds);
            //sendCommands(cmds);
        }
        private void sendCmds(Object obj)
        {
            sendCommands((ArrayList)obj);
        }

        private void btnE2Auto_Click(object sender, EventArgs e)
        {
            showAutoDialog();
            ArrayList cmds = new ArrayList();
            cmds.Add(readRFID(Const.STK_ELPT2));//read rfid
            //readRFID(Const.STK_ELPT2);
            //load
            cmds.Add(getELPTMove(Const.STK_ELPT2, Const.POSITION_ELPT_STOCK_IN));//move in
            //unload
            cmds.Add(getELPTMove(Const.STK_ELPT2, Const.POSITION_ELPT_STOCK_OUT));//move out   
            sendCommands(cmds);
        }

        private void btnI1Auto_Click(object sender, EventArgs e)
        {
            showAutoDialog();
            ArrayList cmds = new ArrayList();
            cmds.Add(ILPT_Load(Const.STK_ILPT1));
            cmds.Add(ILPT_Unload(Const.STK_ILPT1));
            sendCommands(cmds);
        }

        private void btnFoupRotAuto_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc() && checkFoupRobotDest())
            {
                showAutoDialog();
                ArrayList cmds = new ArrayList();
                //get source
                cmds.Add(FoupRobot_GetW(cbSource.Text));//prepare pick
                cmds.Add(FoupRobot_Get(cbSource.Text));//pick
                //put destination
                cmds.Add(FoupRobot_PutW(cbDestination.Text));//prepare place
                cmds.Add(FoupRobot_Put(cbDestination.Text));//place
                //home
                cmds.Add(FoupRobot_Home());//Home
                //get destination
                cmds.Add(FoupRobot_GetW(cbDestination.Text));//prepare pick
                cmds.Add(FoupRobot_Get(cbDestination.Text));//pick
                //put source
                cmds.Add(FoupRobot_PutW(cbSource.Text));//prepare place
                cmds.Add(FoupRobot_Put(cbSource.Text));//place
                //send commands
                sendCommands(cmds);
            }
        }

        private void btnI2Auto_Click(object sender, EventArgs e)
        {
            showAutoDialog();
            ArrayList cmds = new ArrayList();
            cmds.Add(ILPT_Load(Const.STK_ILPT2));
            cmds.Add(ILPT_Unload(Const.STK_ILPT2));
            sendCommands(cmds);
        }


        private void showAutoDialog()
        {
            //return;
            ////關閉主頁功能
            //FormMainUpdate.SetFormEnable("FormMain", false);
            ////FormUpdate.SetTextBoxEmpty("FormAuto", "tbMsg");
            ////顯示自動功能
            //FormAuto autoForm = new FormAuto();
            //autoForm.Show();
          
        }

        private void btnFoupRotHome_Click(object sender, EventArgs e)
        {
            string cmd = FoupRobot_Home();
            sendCommand(cmd);
        }

        private string FoupRobot_Home()
        {
            string cmd = "$1MCR:ROBOT_HOME;";
            return cmd;
        }
        

        /// <summary>
        /// PNO：301 = ILPT1-Clean, 302 = ILPT2-Clean, 303 = CTU-Clean
        ///      311 = ILPT1-Dirty, 312 = ILPT2-Dirty, 313 = CTU-Dirty
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string WHR_ACCESS_POSITION(string name)
        {
            string result = "";
            switch (name)
            {
                case Const.WHR_ILPT1_CLEAN:
                    //result = "301";
                    result = "1";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
                case Const.WHR_ILPT2_CLEAN:
                    //result = "302";
                    result = "2";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
                case Const.WHR_CTU_CLEAN:
                    //result = "303";
                    result = "3";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
                case Const.WHR_ILPT1_DIRTY:
                    //result = "311";
                    result = "1";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
                case Const.WHR_ILPT2_DIRTY:
                    //result = "312";
                    result = "2";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
                case Const.WHR_CTU_DIRTY:
                    //result = "313";
                    result = "3";//1: ILPT1, 2:ILPT2, 3:CTU
                    break;
            }
            return result;
        }

        /// <summary>
        /// POS：  0 = WHR   1 = PTZ
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CTU_ACCESS_POSITION(string name)
        {
            string result = "";
            switch (name)
            {
                case Const.DEVICE_WHR:
                    result = "0";
                    break;
                case Const.DEVICE_PTZ:
                    result = "1";
                    break;
            }
            return result;
        }
        
        private void btnClearMsg_Click(object sender, EventArgs e)
        {
            rtbMsg.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (cbCmd.Text.Trim().Equals(""))
                FormMainUpdate.ShowMessage("Command text is empty.");
            else
            {
                sendCommand(cbCmd.Text);
            }
        }

        private void ResetController(string device)
        {
            string address = "";
            switch (device)
            {
                case Const.CONTROLLER_STK:
                    address = "1";
                    break;
                case Const.CONTROLLER_WHR:
                    address = "2";
                    break;
                case Const.CONTROLLER_CTU_PTZ:
                    address = "3";
                    break;
            }
            string cmd = "$" + address + "SET:RESET";
            sendCommand(cmd);
        }
       
        private void btnAddScript_Click(object sender, EventArgs e)
        {
            if (cbCmd.Text.Trim().Equals(""))
            {
                FormMainUpdate.ShowMessage("No command data!");
                return;
            }

            dgvCmdScript.DataSource = null;
            Command.addScriptCmd(cbCmd.Text);
            FormMainUpdate.refreshScriptSet();
        }
        
        private Boolean checkSelctData()
        {
            Boolean result = false;
            try
            {
                if (dgvCmdScript.RowCount == 0)
                {
                    FormMainUpdate.ShowMessage("No data exists!");
                    return result;
                }
                if (dgvCmdScript.CurrentCell == null)
                {
                    FormMainUpdate.ShowMessage("Please select one row!");
                    return result;
                }
                if (isScriptRunning)
                {
                    FormMainUpdate.ShowMessage("Script is running , please stop it first!");
                    return result;
                }
                result = true;
            }
            catch (Exception e)
            {
                FormMainUpdate.ShowMessage(e.Message);
                //logger.Info(e.StackTrace);
            }
            return result;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (!checkSelctData())
                return;
            int idx = dgvCmdScript.CurrentCell.RowIndex;
            try
            {
                if (idx > 0)
                {
                    CmdScript preItem = Command.getCmdList().FirstOrDefault(predicate: d => d.Seq == idx);
                    CmdScript selItem = Command.getCmdList().FirstOrDefault(predicate: d => d.Seq == idx + 1);
                    preItem.Seq = idx + 1; // change sequence
                    selItem.Seq = idx;
                    Command.oCmdScript = new BindingList<CmdScript>(Command.oCmdScript.OrderBy(x => x.Seq).ToList());
                    dgvCmdScript.DataSource = Command.oCmdScript;
                    dgvCmdScript.ClearSelection();
                    dgvCmdScript.CurrentCell = dgvCmdScript.Rows[idx - 1].Cells[0];
                    dgvCmdScript.Rows[idx - 1].Selected = true;
                }
            }
            catch (Exception ex)
            {
                FormMainUpdate.ShowMessage(ex.Message + ":" + ex.ToString());
            }
        }

        private void setSelectRow(int idx)
        {
            dgvCmdScript.ClearSelection();
            if (dgvCmdScript.RowCount <= 0)
                return;
            else if (dgvCmdScript.RowCount == 1)
                idx = 0;//only one record 
            else if (idx >= dgvCmdScript.RowCount)
                idx = dgvCmdScript.RowCount - 1;//idx more than last 
            dgvCmdScript.CurrentCell = dgvCmdScript.Rows[idx].Cells[0];
            dgvCmdScript.Rows[idx].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (!checkSelctData())
                return;
            int idx = dgvCmdScript.CurrentCell.RowIndex;
            try
            {
                if (idx < dgvCmdScript.RowCount - 1)
                {
                    CmdScript nexItem = Command.getCmdList().FirstOrDefault(predicate: d => d.Seq == idx + 2);
                    CmdScript selItem = Command.getCmdList().FirstOrDefault(predicate: d => d.Seq == idx + 1);
                    nexItem.Seq = idx + 1; // change sequence
                    selItem.Seq = idx + 2;
                    Command.oCmdScript = new BindingList<CmdScript>(Command.oCmdScript.OrderBy(x => x.Seq).ToList());
                    dgvCmdScript.DataSource = Command.oCmdScript;
                    setSelectRow(idx + 1);
                }
            }
            catch (Exception ex)
            {
                FormMainUpdate.ShowMessage(ex.Message + ":" + ex.ToString());
            }
        }

        private void btnStepRun_Click(object sender, EventArgs e)
        {
            if (!checkSelctData())
                return;

            int idx = dgvCmdScript.CurrentCell.RowIndex;
            string cmd = (string)dgvCmdScript.Rows[idx].Cells["Command"].Value;
            sendCommand(cmd);
            //change index
            if (idx < dgvCmdScript.RowCount - 1)
            {
                setSelectRow(idx + 1);
            }
            else
            {
                setSelectRow(0);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (!checkSelctData())
                return;
            string msg = "Are you sure to delete this item?";
            DialogResult confirm = MessageBox.Show(msg, "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (confirm != System.Windows.Forms.DialogResult.Yes)
                return;

            int idx = dgvCmdScript.CurrentCell.RowIndex;
            int delSeq = idx + 1;
            Command.oCmdScript.RemoveAt(idx);
            foreach (CmdScript element in Command.oCmdScript)
            {
                if (element.Seq > delSeq)
                    element.Seq--;
            }
            Command.oCmdScript = new BindingList<CmdScript>(Command.oCmdScript.OrderBy(x => x.Seq).ToList());
            dgvCmdScript.DataSource = Command.oCmdScript;
            setSelectRow(idx);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1;
            StreamReader myStream = null;

            try
            {
                openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "json files (*.json)|*.json";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string line = string.Empty;

                    using (myStream = new StreamReader(openFileDialog1.FileName))
                    {
                        line = myStream.ReadToEnd();
                    }

                    Command.oCmdScript = (BindingList<CmdScript>)Newtonsoft.Json.JsonConvert.DeserializeObject(line, (typeof(BindingList<CmdScript>)));
                    FormMainUpdate.refreshScriptSet();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvCmdScript.Rows.Count == 0)
            {
                return;
            }

            SaveFileDialog saveFileDialog1;
            StreamWriter sw;

            try
            {
                saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "Save file";
                saveFileDialog1.InitialDirectory = ".\\";
                saveFileDialog1.Filter = "json files (*.json)|*.json";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sw = new StreamWriter(saveFileDialog1.FileName.ToString());

                    sw.WriteLine(JsonConvert.SerializeObject(Command.oCmdScript, Formatting.Indented));

                    sw.Close();

                    MessageBox.Show("Done it.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void loadScript(string filename, object sender, EventArgs e)
        {
            StreamReader myStream = null;
            string line = string.Empty;

            using (myStream = new StreamReader(filename))
            {
                line = myStream.ReadToEnd();
            }

            Command.oCmdScript = (BindingList<CmdScript>)Newtonsoft.Json.JsonConvert.DeserializeObject(line, (typeof(BindingList<CmdScript>)));
            FormMainUpdate.refreshScriptSet();
            //btnScriptRun_Click(sender, e);
        }
        private void btnInitAll_Click(object sender, EventArgs e)
        {
            tbTimes.Text = "1";
            loadScript("wts_init.json", sender, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbTimes.Text = "1";
            loadScript("wts_reset.json", sender, e);
        }

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            tbTimes.Text = "99999";
            loadScript("wts_run.json", sender, e);
        }

        private void btnScriptRun_Click(object sender, EventArgs e)
        {
            if (dgvCmdScript.RowCount == 0)
            {
                FormMainUpdate.ShowMessage("No data exists!");
                return;
            }
            isScriptRunning = false;//取消 Script 執行中
            Thread.Sleep(500);

            scriptSTime = DateTime.Now;
            setIsRunning(true);//set Script 執行中
            isScriptRunning = true;//set Script 執行中
            Marco.runMode = Marco.RunMode.SrcScriptRun;

            //ThreadPool.QueueUserWorkItem(new WaitCallback(runScript));
        }

        private void updateCont(object data)
        {
            FormMainUpdate.CounterUpdate(data.ToString());
        }
        private void updateLog(object data)
        {
            FormMainUpdate.Log(data.ToString());
            //lock (rtbMsg)
            //{
            //    FormMainUpdate.LogUpdate(data.ToString());
            //}
        }

        private void logUpdate(string log)
        {
            //FormMainUpdate.LogUpdate(log);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(updateLog), log);//順序錯誤~ 先 MARK 掉
            updateLog(log);
        }

        private void runScript(object data)
        {
            while (true)
            {
                while (!isScriptRunning)
                {
                    SpinWait.SpinUntil(() => isScriptRunning, 99999999);// 60000
                    if (!isScriptRunning)
                    {
                        Console.WriteLine("Script is not running ~ SpinWait Again!!");
                    }
                }
                if (!FormMainUpdate.isAlarmSet && isScriptRunning)
                {
                    runScript();
                }
            }
        }

        private void runScript()
        {
            Random crandom = new Random();
            string txn_id = crandom.Next(1, 999999999).ToString();
            int repeatTimes = 0;
            int.TryParse(tbTimes.Text, out repeatTimes);
            //The efem motion is not allowed when the alarm occurs,please reset alarm first.
            int cnt = 1;
            while (cnt <= repeatTimes && !FormMainUpdate.isAlarmSet && isScriptRunning)
            {
                Thread.Sleep(20);//讓畫面有時間更新, 順序不錯亂
                updateLog("**************  Run Script: " + cnt + "  **************");
                //FormMainUpdate.Log("**************  Run Script: " + cnt + "  **************");
                //FormMainUpdate.LogUpdate("\n**************  Run Script: " + cnt + "  **************");//不另起多執行緒
                //logUpdate("\n**************  Run Script: " + cnt + "  **************");
                ThreadPool.QueueUserWorkItem(new WaitCallback(updateCont), cnt);
                foreach (CmdScript element in Command.oCmdScript)
                {
                    Thread.Sleep(10);//讓畫面有時間更新, 順序不錯亂
                    SpinWait.SpinUntil(() => !isPause, 3600000);// wait for pause 
                    if (isPause)
                    {
                        FormMainUpdate.ShowMessage("Pause Timeout");
                        FormMainUpdate.AlarmUpdate(true);
                        return;//exit for
                    }
                    if (!isScriptRunning)
                    {
                        FormMainUpdate.ShowMessage("Script stop !!");
                        return;//exit for
                    }

                    string cmd = element.Command;
                    isCmdFin = false;
                    //updateLog("kuma:" + txn_id);//kuma
                    sendCommand(cmd);
                    SpinWait.SpinUntil(() => isCmdFin, intCmdTimeOut);// wait for command complete       
                    if (!isCmdFin)
                    {
                        FormMainUpdate.ShowMessage("Command Timeout");
                        FormMainUpdate.AlarmUpdate(true);
                        return;//exit for
                    }
                    //resummn after motion complete               
                    if (FormMainUpdate.isAlarmSet)
                    {
                        FormMainUpdate.ShowMessage("Execute " + cmd + " error.");
                        return;//exit for
                    }
                    currentCmd = ""; //clear command
                    //FormMainUpdate.LogUpdate("**************  Script Commnad Finish  **************");
                    //logUpdate("**************  Script Commnad Finish  **************");//此 Log 會比動作完成還早出現, 所以取消
                    //SpinWait.SpinUntil(() => false, 500);
                }
                cnt++;
            }
            //FormMainUpdate.ShowMessage("Command Script done.");Tony他們說訊息看了很煩!!
            setIsRunning(false);//執行結束
            isScriptRunning = false;//執行結束
            Marco.runMode = Marco.RunMode.Normal;

        }

        private void btnScriptStop_Click(object sender, EventArgs e)
        {
            FormMainUpdate.AlarmUpdate(false);
            isPause = false;
            //FormMainUpdate.LogUpdate("\n*************   Manual Stop     *************");
            logUpdate("\n*************   Manual Stop     *************");
            isScriptRunning = false;
            setIsRunning(false);//執行結束
            isCmdFin = true;
            Marco.runMode = Marco.RunMode.Normal;
        }

        private void dgvCmdScript_DoubleClick(object sender, EventArgs e)
        {
            if (dgvCmdScript.RowCount == 0 || dgvCmdScript.SelectedCells[0].ColumnIndex < 1)
                return;// not command cell
            string o_value = dgvCmdScript.SelectedCells[0].Value.ToString();
            string n_value = ShowDialog("Update", "New Command:", o_value);
            if (n_value.Equals(""))
                return;//cancel update
            else
            {
                CmdScript cmd = Command.oCmdScript.ElementAt(dgvCmdScript.CurrentCell.RowIndex);
                cmd.Command = n_value;
                FormMainUpdate.refreshScriptSet();
            }
        }

        public static string ShowDialog(string title, string label, string text)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = label, Width = 200 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, Text = text };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK, Height = 35 };
            textLabel.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void btnNewScript_Click(object sender, EventArgs e)
        {
            try
            {
                Command.oCmdScript.Clear();//remove list
                FormMainUpdate.refreshScriptSet();
            }
            catch (Exception ex)
            {
                FormMainUpdate.ShowMessage(ex.Message + ":" + ex.ToString());
            }
        }

        private void btnSTKServoOn_Click(object sender, EventArgs e)
        {
            string cmd = "$1SET:SERVO:1";
            sendCommand(cmd);
        }

        public string[] ShowMarcoDialog()
        {
            string[] result = new string[] { "", "" };
            Form prompt = new Form()
            {
                Width = 450,
                Height = 280,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Macro info",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label lblUser = new Label() { Left = 30, Top = 20, Text = "Name", Width = 200 };
            TextBox tbUser = new TextBox() { Left = 30, Top = 50, Width = 350, Text = macroName };
            Label lblPassword = new Label() { Left = 30, Top = 90, Text = "Index", Width = 200 };
            TextBox tbPassword = new TextBox() { Left = 30, Top = 120, Width = 350, Text = index };
            //tbPassword.PasswordChar = '';
            Button confirmation = new Button() { Text = "Ok", Left = 280, Width = 100, Top = 170, DialogResult = DialogResult.OK, Height = 35 };
            lblUser.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblPassword.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tbUser.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tbPassword.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(lblUser);
            prompt.Controls.Add(tbUser);
            prompt.Controls.Add(tbPassword);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(lblPassword);
            prompt.AcceptButton = confirmation;
            tbPassword.Focus();
            //tbUser.Enabled = false;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                result[0] = tbUser.Text;
                result[1] = tbPassword.Text;
            }
            return result;
        }

        class Command_Marco
        {
            public string EachCommand { get; set; }
        }
        
        
       

        private void tabSetting_Click(object sender, EventArgs e)
        {

        }
        

        private void Initial_Error()
        {
            string line;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"error_code.csv");
                while ((line = file.ReadLine()) != null)
                {
                    string[] raw = line.Split(',');
                    error_codes.Add(raw[0], raw[1]);
                }
            }
            catch (Exception e)
            {
                //FormMainUpdate.LogUpdate(e.Message);
                logUpdate(e.Message + " " + e.StackTrace );
            }
        }
        private void Initial_Command()
        {
            string line;
            ArrayList temp = new ArrayList();
            ArrayList cmds = new ArrayList();
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"cmd_list.csv");
                while ((line = file.ReadLine()) != null)
                {
                    temp.Add(line.Replace("\"", "").Trim());
                }
                temp.Sort();
                foreach (string foo in temp)
                {
                    cmds.Add("");
                    cmds.Add(foo);
                }
                cbCmd.Items.Clear();
                cbCmd.DataSource = cmds;
            }
            catch (Exception e)
            {
                //FormMainUpdate.LogUpdate(e.Message);
                logUpdate(e.Message);
            }
        }
        

        
        
        private void btnFoupRotCarry_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc() && checkFoupRobotDest())
            {
                string cmd = FoupRobot_Carry(cbSource.Text, cbDestination.Text);
                sendCommand(cmd);
            }
        }

        /// <summary>
        /// (新命令)$1MCR:CARRY:MC,SSTN,DSTN[CR]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private string FoupRobot_Carry(string source, string destination)
        {
            string fromP = STK_GET_POSITION(source);
            string toP = STK_GET_POSITION(destination);
            //string cmd = "$1CMD:CARRY:" + spno  + ",1,1," + dpno + ",1,1";
            string cmd = "$1MCR:ROBOT_CARRY:" + fromP + "," + toP;
            return cmd;
        }

        private void btnFoupRotMap_Click(object sender, EventArgs e)
        {
            string cmd = FoupRobot_Map();
            sendCommand(cmd);
        }

        private string FoupRobot_Map()
        {
            string cmd = "$1MCR:RBMAP:0";
            return cmd;
        }

        private void login(object sender, EventArgs e)
        {
            if (isAdmin)
            {
                MessageBox.Show("Change to normal mode.");
                isAdmin = false;
            }
            else
            {
                //check 密碼
                string[] use_info = ShowLoginDialog();
                string user_id = use_info[0];
                string password = use_info[1];
                string config_password = "27774061";
                if (password.Equals(config_password))
                {
                    isAdmin = true;
                    MessageBox.Show("Success!! Change to administrator mode.","Notice",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                }
                else
                {
                    isAdmin = false;
                    MessageBox.Show("Login fail! Change to normal mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            }

        public static string[] ShowLoginDialog()
        {
            string[] result = new string[] { "", "" };
            Form prompt = new Form()
            {
                Width = 450,
                Height = 280,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Authority check",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label lblUser = new Label() { Left = 30, Top = 20, Text = "User", Width = 200 };
            TextBox tbUser = new TextBox() { Left = 30, Top = 50, Width = 350, Text = "Administrator" };
            Label lblPassword = new Label() { Left = 30, Top = 90, Text = "Password", Width = 200 };
            TextBox tbPassword = new TextBox() { Left = 30, Top = 120, Width = 350 };
            tbPassword.PasswordChar = '*';
            Button confirmation = new Button() { Text = "Ok", Left = 280, Width = 100, Top = 170, DialogResult = DialogResult.OK, Height = 35 };
            lblUser.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblPassword.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tbUser.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tbPassword.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(lblUser);
            prompt.Controls.Add(tbUser);
            prompt.Controls.Add(tbPassword);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(lblPassword);
            prompt.AcceptButton = confirmation;
            tbPassword.Focus();
            tbUser.Enabled = false;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                result[0] = tbUser.Text;
                result[1] = tbPassword.Text;
            }
            return result;
        }

        /// <summary>
        /// $1MCR:SET_MODE:MODE[CR]
        /// MODE:
        ///     NORMAL,DRY
        /// </summary>
        /// <param name="DEVICE">設備</param>
        /// <param name="MODE">模式</param>
        private void btnChangeMode_Click(object sender, EventArgs e)
        {
            string cmd = "";
            switch (((Control)sender).Name)
            {
                //Normal Mode
                case "btnCtrlModeN_1":
                    cmd = "$1MCR:SET_MODE:NORMAL";//Controller 1
                    break;
                case "btnCtrlModeN_2":
                    cmd = "$2MCR:SET_MODE:NORMAL";//Controller 2
                    break;
                case "btnCtrlModeN_3":
                    cmd = "$3MCR:SET_MODE:NORMAL";//Controller 3
                    break;
                case "btnCtrlModeN_4":
                    cmd = "$4MCR:SET_MODE:NORMAL";//Controller 4
                    break;
                case "btnCtrlModeN_5":
                    cmd = "$5MCR:SET_MODE:NORMAL";//Controller 5
                    break;
                //Dry Mode
                case "btnCtrlModeD_1":
                    cmd = "$1MCR:SET_MODE:DRY";//Controller 1
                    break;
                case "btnCtrlModeD_2":
                    cmd = "$2MCR:SET_MODE:DRY";//Controller 2
                    break;
                case "btnCtrlModeD_3":
                    cmd = "$3MCR:SET_MODE:DRY";//Controller 3
                    break;
                case "btnCtrlModeD_4":
                    cmd = "$4MCR:SET_MODE:DRY";//Controller 4
                    break;
                case "btnCtrlModeD_5":
                    cmd = "$5MCR:SET_MODE:DRY";//Controller 5
                    break;
                default:
                    break;
            }
            sendCommand(cmd);
        }

        private void btnFoupRotInit_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:ROBOT_INIT;";
            sendCommand(cmd);
        }

        private void btnFoupRotReset_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:ROBOT_RESET;";
            sendCommand(cmd);
        }


        private void btnFoupRotServoOn_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:ROBOT_SERVO_ON;";
            sendCommand(cmd);
        }

        private void btnFoupRotOrg_Click(object sender, EventArgs e)
        {

            string cmd = "$1MCR:ROBOT_ORG;";
            sendCommand(cmd);

        }


        private void btnE1ServoOn_Click(object sender, EventArgs e)
        {
            string cmd = getELPTServo(Const.STK_ELPT1, Const.STATUS_ON);
            sendCommand(cmd);
        }

        private void btnE1ServoOff_Click(object sender, EventArgs e)
        {
            string cmd = getELPTServo(Const.STK_ELPT1, Const.STATUS_OFF);
            sendCommand(cmd);
        }
        
        private string getELPTOrg(string port)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:ELPT_ORG/P1;";//呼叫 Marco 指令:ELPT_ORG, 參數: P1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:ELPT_ORG/P2;";//呼叫 Marco 指令:ELPT_ORG, 參數: P2(ELPT2)
                    break;
            }
            return cmd;
        }
        private string getELPTServo(string port, string status)
        {
            string cmd = "";
            switch (port)
            {
                case Const.STK_ELPT1:
                    cmd = "$1MCR:ELPT_SERVO/P1";//呼叫 Marco 指令:ELPT_SERVO, 參數: P1(ELPT1)
                    break;
                case Const.STK_ELPT2:
                    cmd = "$1MCR:ELPT_SERVO/P2";//呼叫 Marco 指令:ELPT_SERVO, 參數: P2(ELPT2)
                    break;
            }
            switch (status)
            {
                case Const.STATUS_ON:
                    cmd = cmd + "/1;";//Servo on
                    break;
                case Const.STATUS_OFF:
                    cmd = cmd + "/0;";//Servo off
                    break;
            }
            return cmd;
        }

        private void btnE2ServoOn_Click(object sender, EventArgs e)
        {
            string cmd = getELPTServo(Const.STK_ELPT2, Const.STATUS_ON);
            sendCommand(cmd);
        }

        private void btnE2ServoOff_Click(object sender, EventArgs e)
        {
            string cmd = getELPTServo(Const.STK_ELPT2, Const.STATUS_OFF);
            sendCommand(cmd);
        }

        private void btnE1Org_Click(object sender, EventArgs e)
        {
            string cmd = getELPTOrg(Const.STK_ELPT1);
            sendCommand(cmd);
        }

        private void btnE2Org_Click(object sender, EventArgs e)
        {
            string cmd = getELPTOrg(Const.STK_ELPT2);
            sendCommand(cmd);
        }

        private void btnCheckSlave_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:CHECK_DNM_SLAVE;";
            sendCommand(cmd);
        }

        private void btnILPT_Click(object sender, EventArgs e)
        {
            string func = ((Button)sender).Name;
            string cmd = "";
            switch (func)
            {
                case "btnI1Clamp":
                    cmd = "$1MCR:ILPT_CLAMP/P3;";
                    break;
                case "btnI2Clamp":
                    cmd = "$1MCR:ILPT_CLAMP/P4;";
                    break;
                case "btnI1UnClamp":
                    cmd = "$1MCR:ILPT_UNCLAMP/P3;";
                    break;
                case "btnI2UnClamp":
                    cmd = "$1MCR:ILPT_UNCLAMP/P4;";
                    break;
                case "btnI1Dock":
                    cmd = "$1MCR:ILPT_DOCK/P3;";
                    break;
                case "btnI2Dock":
                    cmd = "$1MCR:ILPT_DOCK/P4;";
                    break;
                case "btnI1UnDock":
                    cmd = "$1MCR:ILPT_UNDOCK/P3;";
                    break;
                case "btnI2UnDock":
                    cmd = "$1MCR:ILPT_UNDOCK/P4;";
                    break;
                case "btnI1VacOn":
                    cmd = "$1MCR:ILPT_VAC_ON/P3;";
                    break;
                case "btnI2VacOn":
                    cmd = "$1MCR:ILPT_VAC_ON/P4;";
                    break;
                case "btnI1VacOff":
                    cmd = "$1MCR:ILPT_VAC_OFF/P3;";
                    break;
                case "btnI2VacOff":
                    cmd = "$1MCR:ILPT_VAC_OFF/P4;";
                    break;
                case "btnI1LatchRls":
                    cmd = "$1MCR:ILPT_LATCH_RELEASE/P3;";
                    break;
                case "btnI2LatchRls":
                    cmd = "$1MCR:ILPT_LATCH_RELEASE/P4;";
                    break;
                case "btnI1LatchFix":
                    cmd = "$1MCR:ILPT_LATCH_FIX/P3;";
                    break;
                case "btnI2LatchFix":
                    cmd = "$1MCR:ILPT_LATCH_FIX/P4;";
                    break;
                case "btnI1Backward":
                    cmd = "$1MCR:ILPT_BACKWARD/P3;";
                    break;
                case "btnI2Backward":
                    cmd = "$1MCR:ILPT_BACKWARD/P4;";
                    break;
                case "btnI1Forward":
                    cmd = "$1MCR:ILPT_FORWARD/P3;";
                    break;
                case "btnI2Forward":
                    cmd = "$1MCR:ILPT_FORWARD/P4;";
                    break;
                case "btnI1Open":
                    cmd = "$1MCR:ILPT_OPEN/P3;";
                    break;
                case "btnI2Open":
                    cmd = "$1MCR:ILPT_OPEN/P4;";
                    break;
                case "btnI1Close":
                    cmd = "$1MCR:ILPT_CLOSE/P3;";
                    break;
                case "btnI2Close":
                    cmd = "$1MCR:ILPT_CLOSE/P4;";
                    break;
                case "btnI1Left":
                    cmd = "$1MCR:ILPT_LEFT/P3;";
                    break;
                case "btnI2Left":
                    cmd = "$1MCR:ILPT_LEFT/P4;";
                    break;
                case "btnI1Right":
                    cmd = "$1MCR:ILPT_RIGHT/P3;";
                    break;
                case "btnI2Right":
                    cmd = "$1MCR:ILPT_RIGHT/P4;";
                    break;

            }
            if (cmd.Equals(""))
            {
                MessageBox.Show(func + " not define!");
            }
            else
            {
                //MessageBox.Show(cmd,"Notice",MessageBoxButtons.OK,MessageBoxIcon.Question);
                sendCommand(cmd);
            }
        }

        private void btnI1Org_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:ILPT_ORG/P3;";
            sendCommand(cmd);
        }

        private void btnI2Org_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:ILPT_ORG/P4;";
            sendCommand(cmd);
        }

        private void btnReConn_Click(object sender, EventArgs e)
        {
            Marco.ConnDevice();//連接設備
        }

        private void tabMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(tabMode.SelectedTab.Name);
            FormMainUpdate.setShowCommand(tabMode.SelectedTab.Name);
        }

        private void btnSetSpeed_Click(object sender, EventArgs e)
        {
            if (cbSpeed.Text != "")
            {
                string cmd = "$1MCR:ROBOT_SPEED/" + (cbSpeed.Text.Equals("100")?"00": cbSpeed.Text) + ";";
                sendCommand(cmd);
            }
            
        }

        private void tkbrN2FlowControl_ValueChanged(object sender, EventArgs e)
        {
            tbN2FlowControl.Text = tkbrN2FlowControl.Value.ToString();
        }

        private void btnPurgeOn_Click(object sender, EventArgs e)
        {
            if (checkN2Purge() && checkN2BlowTime())
            {
                string cmd;
                cmd = "$1MCR:N2PURGE_ON/";

                //機架位置與通訊的層數定義顛倒
                int plateIDInStr = cmbN2PurgeNo.Text.Trim().IndexOf("-") + 1;
                string plateId = cmbN2PurgeNo.Text.Trim().Substring(plateIDInStr);

                //加入層數與區域編號
                string areaId = getN2PurgeAreaId();
                cmd = cmd + areaId + getN2PurgePlateId() + "/";

                //加入流量控制
                cmd = cmd + tkbrN2FlowControl.Value.ToString() + "/";

                //加入時間控制
                cmd = cmd + cmbN2BlowTime.Text.Trim().ToString() + ";";

                sendCommand(cmd);

                //更新介面
                FormMainUpdate.N2PurgeStatusUpdate(int.Parse(plateId), int.Parse(areaId), 
                            tkbrN2FlowControl.Value.ToString(), "1" ,cmbN2BlowTime.Text.Trim().ToString() ,true);

            }
        }

        private string getN2PurgePlateId()
        {
            string shelfid;
            int shelfIdx = cmbN2PurgeNo.Text.Trim().IndexOf("-");
            shelfid = cmbN2PurgeNo.Text.Trim().Substring(shelfIdx + 1);

            return shelfid.ToString();
        }

        private string getN2PurgeAreaId()
        {
            string returnid;

            returnid = cmbN2PurgeNo.Text.Replace("SHELF", "").Trim();
            returnid = returnid.Substring(0, returnid.IndexOf("-"));

            return returnid;
        }

        private void btnPurgeOff_Click(object sender, EventArgs e)
        {
            if (checkN2Purge())
            {
                string cmd;
                cmd = "$1MCR:N2PURGE_OFF/";

                //機架位置與通訊的層數定義顛倒
                int plateIndex = cmbN2PurgeNo.Text.Trim().IndexOf("-") + 1;
                string plateId = cmbN2PurgeNo.Text.Trim().Substring(plateIndex);

                //加入層數與區域編號
                string areaId = getN2PurgeAreaId();
                cmd = cmd + areaId + getN2PurgePlateId() + ";";

                sendCommand(cmd);

                //setN2PurgeStatus(cmbN2PurgeNo.Text.Trim(), false);

                //更新介面
                FormMainUpdate.N2PurgeStatusUpdate(int.Parse(plateId), int.Parse(areaId),
                            tkbrN2FlowControl.Value.ToString(), "0", cmbN2BlowTime.Text.Trim().ToString(), true);
            }
        }

        private void btnO2ThresholdSet_Click(object sender, EventArgs e)
        {
            if (checkO2Threshold())
            {
                tbO2ThresholdLow.BackColor = Color.White;
                tbO2ThresholdHigh.BackColor = Color.White;

                string cmd;
                cmd = "$1MCR:O2_SET_THRESHOLD/";
                cmd = cmd + tbO2ThresholdLow.Text.Trim() +"/"+ tbO2ThresholdHigh.Text.Trim() + ";";
                sendCommand(cmd);
            }
        }

        private void btnN2PurgeInit_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = "$1MCR:N2PURGE_INIT;";
            sendCommand(cmd);
        }

        private void btnGetN2PurgeStatus_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = "$1MCR:N2PURGE_GET_STS/";

            int plateIdx = cmbN2PurgeNo.Text.Trim().IndexOf("-") + 1;

            //加入層數與區域編號
            cmd = cmd + getN2PurgeAreaId() + getN2PurgePlateId() + ";";

            sendCommand(cmd);
        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void btnGetAllN2PurgeStatus_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = "$1MCR:N2PURGE_GET_STS/";

            //機架位置與通訊的層數定義顛倒
            //int plateIdx = cmbN2PurgeNo.Text.Trim().IndexOf("-") + 1;

            //加入層數與區域編號
            cmd = cmd + getN2PurgePlateId() + "/0;";

            sendCommand(cmd);
        }

        private void btnO2ThresholdGet_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = "$1MCR:O2_GET_STS;";

            sendCommand(cmd);
        }

        private void cmbN2PurgeNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int plateIndex = cmbN2PurgeNo.Text.Trim().IndexOf("-") + 1;
            string plateId = cmbN2PurgeNo.Text.Trim().Substring(plateIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Marco.Test();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string cmd;

            tbO2ThresholdLow.BackColor = Color.White;
            tbO2ThresholdHigh.BackColor = Color.White;

            cmd = "$1MCR:N2PURGE_RESET;";
            sendCommand(cmd);
        }

        private void btnClearN2Result_Click(object sender, EventArgs e)
        {
            rtbExcuteN2Result.Clear();
        }

        private void rtbExcuteN2Result_TextChanged(object sender, EventArgs e)
        {
            rtbExcuteN2Result.SelectionStart = rtbExcuteN2Result.TextLength;
            rtbExcuteN2Result.ScrollToCaret();
        }

        private void cmbMFCStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMFCStage.SelectedIndex > -1 && cmbMFCBaudSetting.SelectedIndex > -1)
            {
                btnMFCSetting.Enabled = true;

                switch (cmbMFCStage.SelectedIndex)
                {
                    case 0:
                        pnMFCGroup1.BackColor = Color.GreenYellow;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 1:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.GreenYellow;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 2:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.GreenYellow;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 3:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.GreenYellow;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 4:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.GreenYellow;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 5:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.GreenYellow;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                    case 6:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.GreenYellow;
                        break;
                    default:
                        pnMFCGroup1.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup2.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup3.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup4.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup5.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup6.BackColor = Color.DarkSeaGreen;
                        pnMFCGroup7.BackColor = Color.DarkSeaGreen;
                        break;
                }
            }
        }

        private void btnMFCSetting_Click(object sender, EventArgs e)
        {
            string cmd = "$1MCR:N2PURGE_SETMFC";

            if (cmbMFCBaudSetting.SelectedIndex == 0)
            {
                cmd += "/38400";
            }
            else
            {
                cmd += "/115200";
            }

            switch (cmbMFCStage.SelectedIndex)
            {
                case 0:
                    cmd += "/1;";
                    break;
                case 1:
                    cmd += "/2;";
                    break;
                case 2:
                    cmd += "/3;";
                    break;
                case 3:
                    cmd += "/4;";
                    break;
                case 4:
                    cmd += "/5;";
                    break;
                case 5:
                    cmd += "/6;";
                    break;
                case 6:
                    cmd += "/7;";
                    break;
                default:
                    break;
            }

            sendCommand(cmd);
        }

        private void btnGetAndPut_Click(object sender, EventArgs e)
        {
            if (checkFoupRobotSrc() && checkFoupRobotDest())
            {
                string cmd = "";
                string sourcePosition = STK_GET_POSITION(cbSource.Text);
                string destinationPosition = STK_GET_POSITION(cbDestination.Text);

                if (sourcePosition != null && !sourcePosition.Trim().Equals("") &&
                    destinationPosition != null && !destinationPosition.Trim().Equals(""))
                {
                    lbTaktTime.Text = "0.0";
                    robotGetPutSTime = DateTime.Now;
                    cmd = "$1MCR:ROBOT_GETPUT/" + sourcePosition + "/"+ destinationPosition + ";";
                    sendCommand(cmd);
                }
            }
        }

        private void timerScript_Tick(object sender, EventArgs e)
        {
            if (Marco.runMode == Marco.RunMode.SrcScriptRun)
            {
                if(isScriptRunning)
                {
                    lbScriptDay.Text = (DateTime.Now - scriptSTime).Days.ToString();
                    lbScriptHour.Text = (DateTime.Now - scriptSTime).Hours.ToString();
                    lbScriptMintue.Text = (DateTime.Now - scriptSTime).Minutes.ToString();
                    lbScriptSecond.Text = (DateTime.Now - scriptSTime).Seconds.ToString();
                }
            }
        }
    }
}
