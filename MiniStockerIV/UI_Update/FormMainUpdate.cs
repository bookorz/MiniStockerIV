using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SanwaMarco;
using Microsoft.VisualBasic.PowerPacks;

namespace MiniStockerIV.UI_Update
{
    class FormMainUpdate
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(FormMainUpdate));
        public static Boolean isAlarmSet = false;
        public static Boolean isShowCmd = true;
        private static string[] rsltPresence = new string[0];
        public static string[] RsltPresence { get => rsltPresence; set => rsltPresence = value; }
        private static ArrayList cmdList = new ArrayList();// = new ArrayList();

        delegate void UpdateLog(string msg);
        delegate void UpdateIO(string msg);
        delegate void UpdateN2PurgeStatus(int plate, int stage, string mfc, string enabled, string time, bool set);
        delegate void UpdateN2PurgeGetStatus(string stagePlate, string mfc, string time, string enabled);
        delegate void UpdateN2PurgeEnabledStatus(string stagePlate, string enabled);
        delegate void UpdateO2Value(string value);
        delegate void UpdateO2Monitor(string value);
        delegate void UpdateRecevicedEvt(string value);
        delegate void UpdateRobotGetPutFinish(DateTime startTime);
        delegate ArrayList RefreshIO(string tcName, string panelIn, string panelOut);
        delegate void UpdateAlarm(Boolean isAlarm);
        delegate void UpdateBtnEnable(Boolean isRun);
        delegate void MessageShow(string msg);
        delegate void EnableForm(string formName, Boolean enable);
        delegate void ClearMsg(string formName, string tboxName);
        delegate void ChgRunTab(int index);
        delegate void RefreshScript();
        delegate void AddScript(string cmd);
        delegate void UpdateFoupPresence();
        delegate void UpdateUI(string msg);
        delegate void SetShowCommand(string msg);

        public static void addScriptCmd(string cmd)
        {
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                AddScript ph = new AddScript(addScriptCmd);
                form.BeginInvoke(ph, cmd);
            }
            else
            {
                int seq = Command.oCmdScript.Count + 1;
                Command.oCmdScript.Add(new CmdScript { Seq = seq, Command = cmd });
            }
        }

        public static void refreshScriptSet()
        {
            Form form = Application.OpenForms["FormMain"];
            DataGridView dgvCmdScript = form.Controls.Find("dgvCmdScript", true).FirstOrDefault() as DataGridView;
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                RefreshScript ph = new RefreshScript(refreshScriptSet);
                form.BeginInvoke(ph);
            }
            else
            {
                dgvCmdScript.DataSource = Command.oCmdScript;
                if (dgvCmdScript.RowCount > 0)
                {
                    dgvCmdScript.Columns[0].Width = 50;
                    dgvCmdScript.Columns[1].Width = 700;
                }
            }
        }

        public static void ChangeRunTab(int index)
        {
            Form form = Application.OpenForms["FormMain"];
            TabControl tab = form.Controls.Find("tabMode", true).FirstOrDefault() as TabControl;
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                ChgRunTab ph = new ChgRunTab(ChangeRunTab);
                form.BeginInvoke(ph, index);
            }
            else
            {
                tab.SelectedIndex = index;
            }
        }

        public static void SetTextBoxEmpty(string formName, string tboxName)
        {
            Form form = Application.OpenForms[formName];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                ClearMsg ph = new ClearMsg(SetTextBoxEmpty);
                form.BeginInvoke(ph, formName, tboxName);
            }
            else
            {
                RichTextBox btnUp = form.Controls.Find(tboxName, true).FirstOrDefault() as RichTextBox;
                btnUp.Text = "";
            }
        }
        public static void SetFormEnable(string formName, Boolean enable)
        {
            Form form = Application.OpenForms[formName];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                EnableForm ph = new EnableForm(SetFormEnable);
                form.BeginInvoke(ph, formName, enable);
            }
            else
            {
                form.Enabled = enable;
            }

        }

        public static void ShowMessage(string msg)
        {
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                MessageShow ph = new MessageShow(ShowMessage);
                form.BeginInvoke(ph, msg);
            }
            else
            {
                MessageBox.Show(form, msg);
            }

        }

        public static void SetRunBtnEnable(Boolean isRun)
        {
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;
            Button btnScriptRun = form.Controls.Find("btnScriptRun", true).FirstOrDefault() as Button;
            Button btnDel = form.Controls.Find("btnDel", true).FirstOrDefault() as Button;
            Button btnUp = form.Controls.Find("btnUp", true).FirstOrDefault() as Button;
            Button btnDown = form.Controls.Find("btnDown", true).FirstOrDefault() as Button;
            Button btnStepRun = form.Controls.Find("btnStepRun", true).FirstOrDefault() as Button;
            Button btnImport = form.Controls.Find("btnImport", true).FirstOrDefault() as Button;
            Button btnExport = form.Controls.Find("btnExport", true).FirstOrDefault() as Button;
            Button btnSend = form.Controls.Find("btnSend", true).FirstOrDefault() as Button;
            Button btnAddScript = form.Controls.Find("btnAddScript", true).FirstOrDefault() as Button;
            Button btnNewScript = form.Controls.Find("btnNewScript", true).FirstOrDefault() as Button;

            if (btnScriptRun == null)
            {
                return;//隱藏模式
            }

            if (form.InvokeRequired)
            {
                UpdateBtnEnable ph = new UpdateBtnEnable(SetRunBtnEnable);
                form.BeginInvoke(ph, isRun);
            }
            else
            {
                btnScriptRun.Enabled = !isRun;
                btnDel.Enabled = !isRun;
                btnUp.Enabled = !isRun;
                btnDown.Enabled = !isRun;
                btnStepRun.Enabled = !isRun;
                btnImport.Enabled = !isRun;
                btnExport.Enabled = !isRun;
                btnSend.Enabled = !isRun;
                btnAddScript.Enabled = !isRun;
                btnNewScript.Enabled = !isRun;
            }

        }

        public static void AlarmUpdate(Boolean isAlarm)
        {
            string status = isAlarm ? "Alarm set" : "Alarm clear";
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;
            Label W = form.Controls.Find("lbl_alarm", true).FirstOrDefault() as Label;
            Button btnReset = form.Controls.Find("btnReset", true).FirstOrDefault() as Button;
            Button btnHold = form.Controls.Find("btnHold", true).FirstOrDefault() as Button;
            Button btnAbort = form.Controls.Find("btnAbort", true).FirstOrDefault() as Button;
            Button btnRestart = form.Controls.Find("btnRestart", true).FirstOrDefault() as Button;

            if (W == null)
                return;

            if (W.InvokeRequired)
            {
                UpdateAlarm ph = new UpdateAlarm(AlarmUpdate);
                W.BeginInvoke(ph, isAlarm);
            }
            else
            {
                W.Text = status;
                switch (status)
                {
                    case "Alarm clear":
                        W.BackColor = Color.LimeGreen;
                        //btnReset.Enabled = false; //20180914 change to  always open
                        isAlarmSet = false;
                        btnHold.Visible = true;
                        btnAbort.Visible = false;
                        btnRestart.Visible = false;
                        break;
                    case "Alarm set":
                        W.BackColor = Color.Red;
                        //btnReset.Enabled = true; //20180914 change to  always open
                        isAlarmSet = true;
                        break;
                }
            }
        }

        public static void ConnectUpdate(string state)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];
                Label W;
                if (form == null)
                    return;

                W = form.Controls.Find("lbl_ConnectState", true).FirstOrDefault() as Label;
                Button btnDisConn = form.Controls.Find("btnDisConn", true).FirstOrDefault() as Button;
                Button btnConn = form.Controls.Find("btnConn", true).FirstOrDefault() as Button;
                if (W == null)
                    return;

                if (W.InvokeRequired)
                {
                    UpdateLog ph = new UpdateLog(ConnectUpdate);
                    W.BeginInvoke(ph, state);
                }
                else
                {
                    W.Text = state;
                    switch (state)
                    {
                        case "Connected":
                            W.BackColor = Color.LimeGreen;
                            btnConn.Enabled = false;
                            btnDisConn.Enabled = true;
                            break;
                        case "Disconnected":
                            W.BackColor = Color.Gray;
                            btnConn.Enabled = true;
                            btnDisConn.Enabled = false;
                            break;
                        case "Connection_Error":
                            W.BackColor = Color.Red;
                            btnConn.Enabled = true;
                            btnDisConn.Enabled = false;
                            break;
                        case "Connecting":
                            W.BackColor = Color.Yellow;
                            btnConn.Enabled = false;
                            btnDisConn.Enabled = false;
                            break;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Info(e.StackTrace);
            }
        }

        public static void IONameUpdate(string msg)
        {
            try
            {

                Form form = Application.OpenForms["FormMain"];
                Label lbl_i = null;
                Label lbl_o = null;
                string address = msg.Substring(0, 1);
                string id_i = address + "_" + msg.Substring(12, msg.IndexOf("/") - 12) + "_IN";
                string id_o = address + "_" + msg.Substring(12, msg.IndexOf("/") - 12) + "_OUT";

                if (form == null)
                    return;

                if (form.InvokeRequired)
                {
                    UpdateIO ph = new UpdateIO(IONameUpdate);
                    form.BeginInvoke(ph, msg);
                }
                else
                {
                    lbl_i = form.Controls.Find(id_i, true).FirstOrDefault() as Label;
                    lbl_o = form.Controls.Find(id_o, true).FirstOrDefault() as Label;
                    if (lbl_i != null)
                    {
                        if (msg.EndsWith("ON;"))
                            lbl_i.ForeColor = Color.LimeGreen;
                        else
                            lbl_i.ForeColor = Color.Red;
                    }
                    if (lbl_o != null)
                    {
                        if (msg.EndsWith("ON;"))
                            lbl_o.ForeColor = Color.LimeGreen;
                        else
                            lbl_o.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void IOUpdate(string msg)
        {
            try
            {

                Form form = Application.OpenForms["FormMain"];
                Label lbl_i = null;
                Label lbl_o = null;
                string address = msg.Substring(1, 1);
                //$1ACK:RIO__:no,vl[CR]
                //AddressNo + "_" + ID + "_" + Type
                //$1INF:GET_RELIO/103/0;
                string ioNo = msg.Split('/')[1];
                string id_i = address + "_" + ioNo + "_IN";
                string id_o = address + "_" + ioNo + "_OUT";

                if (form == null)
                    return;

                if (form.InvokeRequired)
                {
                    UpdateIO ph = new UpdateIO(IOUpdate);
                    form.BeginInvoke(ph, msg);
                }
                else
                {
                    lbl_i = form.Controls.Find(id_i, true).FirstOrDefault() as Label;
                    lbl_o = form.Controls.Find(id_o, true).FirstOrDefault() as Label;
                    if (lbl_i != null)
                    {
                        if (msg.EndsWith("1;"))
                            lbl_i.ForeColor = Color.LimeGreen;
                        else
                            lbl_i.ForeColor = Color.Red;
                    }
                    if (lbl_o != null)
                    {
                        if (msg.EndsWith("1;"))
                            lbl_o.ForeColor = Color.LimeGreen;
                        else
                            lbl_o.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }

        //plate:層數(以機架的編號來看)  
        //stage:區域(Area)
        //mfc:流量控制
        //enabled:(是否啟用)
        //time:(開啟氣閥時間)
        //set:Set\Get
        public static void N2PurgeStatusUpdate(int plate, int stage, string mfc, string enabled, string time, bool set)
        {
            if (plate < 1 || plate > FormMain.maxFloorNo) return;
            if (stage < 1 || stage > 4) return;
            if (!enabled.Equals("0") && !enabled.Equals("1")) return;

            bool onoff = enabled.Equals("1") ? true : false;
            try
            {

                string controlName;
                string controlPurgeTime;
                //string controlMFC;
                //暫時針對26 Port
                controlName = "tbN2Purge_" + stage.ToString() + "_" + plate.ToString();
                controlPurgeTime = "N2_PurgeTime_" + stage.ToString() + "_" + plate.ToString();
                //controlMFC = "tbN2PurgeMFC_" + plate.ToString();
                string rectName = "rtN2Purge_" + stage.ToString() + "_" + plate.ToString();


                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateN2PurgeStatus ph = new UpdateN2PurgeStatus(N2PurgeStatusUpdate);
                    form.BeginInvoke(ph, plate, stage, mfc, enabled, time, set);
                }
                else
                {

                    TextBox tb = form.Controls.Find(controlName, true).FirstOrDefault() as TextBox;
                    string msg;
                    msg = stage.ToString() + "_" + plate.ToString() + ":";
                    tb.Text = onoff ? "On" : "Off";
                    tb.Text = msg + tb.Text;
                    tb.ForeColor = onoff ? SystemColors.Highlight : Color.Red;
                    tb.BackColor = onoff ? (set ? Color.White : Color.Lime) : (set ? Color.LightGray : Color.LimeGreen);

                    Label lb = form.Controls.Find(controlPurgeTime, true).FirstOrDefault() as Label;
                    lb.Text = time.ToString();

                    //tb = form.Controls.Find(controlMFC, true).FirstOrDefault() as TextBox;
                    ///tb.Text = mfc.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void N2PurgeEnabledStatusUpdate(string stagePlate, string enabled)
        {
            int plate = Convert.ToInt32(stagePlate.Substring(1, 1));
            int stage = Convert.ToInt32(stagePlate.Substring(0, 1));

            if (plate < 1 || plate > FormMain.maxFloorNo) return;
            if (stage < 1 || stage > 4) return;
            if (!enabled.Equals("0") && !enabled.Equals("1")) return;

            bool onoff = enabled.Equals("1") ? true : false;
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateN2PurgeEnabledStatus ph = new UpdateN2PurgeEnabledStatus(N2PurgeEnabledStatusUpdate);
                    form.BeginInvoke(ph, stagePlate, enabled);
                }
                else
                {
                    string controlName = "tbN2Purge_" + stage.ToString() + "_" + plate.ToString();
                    TextBox tb = form.Controls.Find(controlName, true).FirstOrDefault() as TextBox;
                    string msg;
                    msg = stage.ToString() + "_" + plate.ToString() + ":";
                    tb.Text = onoff ? "On" : "Off";
                    tb.Text = msg + tb.Text;
                    tb.ForeColor = onoff ? SystemColors.Highlight : Color.Red;
                    tb.BackColor = onoff ? Color.Lime : Color.LimeGreen;

                    RichTextBox rtb = form.Controls.Find("rtbExcuteN2Result", true).FirstOrDefault() as RichTextBox;
                    if (rtb.SelectionColor != Color.Blue)
                        rtb.SelectionColor = Color.Blue;
                    string showMsg = "";

                    if (onoff)
                    {
                        showMsg = "N2 Purge 狀態: Plate " + plate.ToString() + ", Stage " + stage.ToString() + ", 開啟\r\n";
                    }
                    else
                    {
                        showMsg = "N2 Purge 狀態: Plate " + plate.ToString() + ", Stage " + stage.ToString() + ", 關閉\r\n";

                        controlName = "N2_PurgeTime_" + stage.ToString() + "_" + plate.ToString();
                        Label lb = form.Controls.Find(controlName, true).FirstOrDefault() as Label;
                        lb.Text = "0";
                    }

                    rtb.AppendText(showMsg);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void N2PurgeGetStatusUpdate(string stagePlate, string mfc, string time, string enabled)
        {
            int plate = Convert.ToInt32(stagePlate.Substring(1, 1));
            int stage = Convert.ToInt32(stagePlate.Substring(0, 1));

            if (plate < 1 || plate > FormMain.maxFloorNo) return;
            if (stage < 1 || stage > 4) return;
            if (!enabled.Equals("0") && !enabled.Equals("1")) return;

            bool onoff = enabled.Equals("1") ? true : false;
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateN2PurgeGetStatus ph = new UpdateN2PurgeGetStatus(N2PurgeGetStatusUpdate);
                    form.BeginInvoke(ph, stagePlate, mfc, time, enabled);
                }
                else
                {
                    RichTextBox rtb = form.Controls.Find("rtbExcuteN2Result", true).FirstOrDefault() as RichTextBox;
                    if (rtb.SelectionColor != Color.Blue)
                        rtb.SelectionColor = Color.Blue;
                    string showMsg = "";

                    if (onoff)
                    {
                        showMsg = "N2 Purge 狀態(開啟): Plate " + plate.ToString() + ", Stage " + stage.ToString() + ", 流量 "+ mfc + ", 時間 "+ time + "(秒)\r\n";
                    }
                    else
                    {
                        showMsg = "N2 Purge 狀態(關閉): Plate " + plate.ToString() + ", Stage " + stage.ToString() + ", 流量 "+ mfc + ",時間 " + time + "(秒)\r\n";
                    }

                    rtb.AppendText(showMsg);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }


        }
        public static void O2MonitorUpdate(string value)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateO2Monitor ph = new UpdateO2Monitor(O2MonitorUpdate);
                    form.BeginInvoke(ph, value);
                }
                else
                {
                    TextBox tb1 = form.Controls.Find("tbO2ThresholdLow", true).FirstOrDefault() as TextBox;
                    TextBox tb2 = form.Controls.Find("tbO2ThresholdHigh", true).FirstOrDefault() as TextBox;

                    bool UpLimit = value.Equals("1") ? true : false;
                    tb1.BackColor = UpLimit ? Color.White : Color.Red;
                    tb2.BackColor = UpLimit ? Color.Red : Color.White;

                    RichTextBox rtb = form.Controls.Find("rtbExcuteN2Result", true).FirstOrDefault() as RichTextBox;
                    if (rtb.SelectionColor != Color.Red)
                        rtb.SelectionColor = Color.Red;
                    string showMsg = "";

                    if (UpLimit)
                    {
                        showMsg = "O2 監控: 超過上限\r\n";
                    }
                    else
                    {
                        showMsg = "O2 監控: 超過下限\r\n";
                    }

                    rtb.AppendText(showMsg);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void O2ValueUpdate(string value)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateO2Value ph = new UpdateO2Value(O2ValueUpdate);
                    form.BeginInvoke(ph, value);
                }
                else
                {
                    TextBox tb = form.Controls.Find("tbO2Value", true).FirstOrDefault() as TextBox;
                    tb.Text = value;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void RecevicedEvtUpdate(string value)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateRecevicedEvt ph = new UpdateRecevicedEvt(RecevicedEvtUpdate);
                    form.BeginInvoke(ph, value);
                }
                else
                {
                    RichTextBox rtb = form.Controls.Find("rtbMsg", true).FirstOrDefault() as RichTextBox;
                    rtb.AppendText(value);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static void RobotGetPutFinishUpdate(DateTime startTime)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];

                if (form.InvokeRequired)
                {
                    UpdateRobotGetPutFinish ph = new UpdateRobotGetPutFinish(RobotGetPutFinishUpdate);
                    form.BeginInvoke(ph, startTime);
                }
                else
                {
                    Label rtb = form.Controls.Find("lbTaktTime", true).FirstOrDefault() as Label;
                    rtb.Text = (DateTime.Now - startTime).TotalSeconds.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        public static ArrayList getIORefreshCmds(string tcName, string pnlInName, string pnlOutName)
        {
                try
                {
                    Form form = Application.OpenForms["FormIO"];
                    if (form == null)
                        return null;

                    if (form.InvokeRequired)
                    {
                        RefreshIO ph = new RefreshIO(getIORefreshCmds);
                        form.Invoke(ph, tcName, pnlInName, pnlOutName);
                    }
                    else
                    {
                        cmdList = new ArrayList();
                        TabControl tc = form.Controls.Find(tcName, true).FirstOrDefault() as TabControl;
                        Panel p_in = form.Controls.Find(pnlInName, true).FirstOrDefault() as Panel;
                        Panel p_out = form.Controls.Find(pnlOutName, true).FirstOrDefault() as Panel;
                        if (tc.SelectedTab.Text.Equals("IN"))
                        {
                            foreach (Control foo in p_in.Controls)
                            {
                                if (!foo.GetType().Name.Equals("Label"))
                                {
                                    continue;
                                }
                                else if (!foo.Text.Equals("■"))
                                {
                                    string address = foo.Name.Split('_')[0];
                                    string rio = foo.Text.Substring(0, foo.Text.IndexOf("("));
                                    cmdList.Add("$" + address + "MCR:GET_RELIO/" + rio + ";");
                                }
                            }
                        }
                        else
                        {
                            foreach (Control foo in p_out.Controls)
                            {
                                if (!foo.GetType().Name.Equals("Label"))
                                {
                                    continue;
                                }
                                else if (!foo.Text.Equals("■"))
                                {
                                    string address = foo.Name.Split('_')[0];
                                    //Console.WriteLine(foo.Name);
                                    string rio = foo.Text.Substring(0, foo.Text.IndexOf("("));
                                    cmdList.Add("$" + address + "MCR:GET_RELIO/" + rio + ";");
                                }
                            }
                        }
                    }
                    return cmdList;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace);
                    return null;
                }
        }

        public static void CounterUpdate(string cnt)
        {
            try
            {
                Form form = Application.OpenForms["FormMain"];
                if (form == null)
                    return;
                TextBox tbCounter = form.Controls.Find("tbCounter", true).FirstOrDefault() as TextBox;
                if (tbCounter == null)
                    return;
                if (tbCounter.InvokeRequired)
                {
                    UpdateUI ph = new UpdateUI(CounterUpdate);
                    tbCounter.BeginInvoke(ph, cnt);
                }
                else
                {
                    tbCounter.Text = cnt;
                }
            }
            catch
            {

            }
        }
        public static void Log(string msg)
        {
            if (!isShowCmd)
                return;
            logger.Info(msg);
        }

        //static object obj = new object();
        //public static void LogUpdate(string msg)
        //{
        //    //lock (obj)
        //    //{
        //        try
        //        {
        //            Form form = Application.OpenForms["FormMain"];
        //            RichTextBox W;
        //            TabControl tabMode;
        //            if (form == null)
        //                return;

        //            W = form.Controls.Find("rtbMsg", true).FirstOrDefault() as RichTextBox;
        //            tabMode = form.Controls.Find("tabMode", true).FirstOrDefault() as TabControl;

        //            if (W == null)
        //                return;

        //            if (W.InvokeRequired)
        //            {
        //                UpdateLog ph = new UpdateLog(LogUpdate);
        //                //W.BeginInvoke(ph, msg);
        //                W.Invoke(ph, msg);
        //            }
        //            else
        //            {
        //                W.SelectionStart = W.TextLength;
        //                W.SelectionLength = 0;
        //                if (msg.ToUpper().Contains("FIN"))
        //                {
        //                    W.SelectionColor = Color.Green;
        //                }
        //                else if (msg.ToUpper().Contains("RECEIVE"))
        //                {
        //                    W.SelectionColor = Color.Blue;
        //                }
        //                else if (msg.ToUpper().Contains("CMD") || msg.ToUpper().Contains("GET") || msg.ToUpper().Contains("SET"))
        //                {
        //                    W.SelectionColor = Color.Coral;
        //                }
        //                else if (msg.ToUpper().Contains("MCR"))
        //                {
        //                    W.SelectionColor = Color.Coral;
        //                }
        //                else if (msg.ToUpper().Contains("異常描述"))
        //                {
        //                    W.SelectionColor = Color.Red;
        //                }
        //                else
        //                {
        //                    W.SelectionColor = Color.DimGray;
        //                }
        //                W.AppendText(msg + "\n");
        //                W.SelectionColor = W.ForeColor;

        //                W.ScrollToCaret();//移動卷軸到最後
        //                if (W.Lines.Length > 1000)//只保留最後一千行
        //                {
        //                    W.Select(0, W.GetFirstCharIndexFromLine(W.Lines.Length - 1000));
        //                    W.SelectedText = "";
        //                }
        //                //if (tabMode.SelectedIndex != 0)
        //                //    tabMode.SelectedIndex = 0;
        //            }
        //        }
        //        catch
        //        {

        //        }
        //    //}
        //}

        public static void Update_IO(string key,string val)
        {
            Form form = Application.OpenForms["FormMain"];
            Label signal = form.Controls.Find(key, true).FirstOrDefault() as Label;
            if (form == null)
                return;
            if (signal == null)
                return;

            if (signal.InvokeRequired)
            {
                ClearMsg ph = new ClearMsg(Update_IO);
                signal.BeginInvoke(ph,key,val);
            }
            else
            {
                if (val.Equals("ON"))
                {
                    signal.ForeColor = Color.LimeGreen;
                }
                else
                {
                    signal.ForeColor = Color.Red;
                }
            }
        }

        internal static void updateFoupPresenceByFoups()
        {
            //string[] positions = new string[] { "tbPresRobot","tbPresELPT1","tbPresELPT2","tbPresILPT1","tbPresILPT2",
            //"tbPresShelf1_1", "tbPresShelf1_2", "tbPresShelf1_3", "tbPresShelf1_4", "tbPresShelf1_5",
            //"tbPresShelf2_1", "tbPresShelf2_2", "tbPresShelf2_3", "tbPresShelf2_4", "tbPresShelf2_5",
            //"tbPresShelf3_1", "tbPresShelf3_2", "tbPresShelf3_3", "tbPresShelf3_4",
            //"tbPresShelf4_1", "tbPresShelf4_2", "tbPresShelf4_3", "tbPresShelf4_4"
            //};
            string[] positions;

            if (Marco.machineType == Marco.MachineType.Normal)
            {
                positions = new string[] { "tbPresRobot","tbPresELPT1","tbPresELPT2","tbPresILPT1","tbPresILPT2",
                                            "tbPresShelf1_1", "tbPresShelf1_2", "tbPresShelf1_3", "tbPresShelf1_4", "tbPresShelf1_5",
                                            "tbPresShelf2_1", "tbPresShelf2_2", "tbPresShelf2_3", "tbPresShelf2_4", "tbPresShelf2_5",
                                            "tbPresShelf3_1", "tbPresShelf3_2", "tbPresShelf3_3", "tbPresShelf3_4",
                                            "tbPresShelf4_1", "tbPresShelf4_2", "tbPresShelf4_3", "tbPresShelf4_4"
                                            };

            }
            else
            {
                positions = new string[] { "tbPresRobot","tbPresELPT1","tbPresELPT2","tbPresILPT1","tbPresILPT2",
                                            "tb26P_PresShelf1_1", "tb26P_PresShelf1_2", "tb26P_PresShelf1_3", "tb26P_PresShelf1_4",
                                            "tb26P_PresShelf1_5", "tb26P_PresShelf1_6", "tb26P_PresShelf1_7",
                                            "tb26P_PresShelf2_1", "tb26P_PresShelf2_2", "tb26P_PresShelf2_3", "tb26P_PresShelf2_4",
                                            "tb26P_PresShelf2_5", "tb26P_PresShelf2_6", "tb26P_PresShelf2_7",
                                            "tb26P_PresShelf3_1", "tb26P_PresShelf3_2", "tb26P_PresShelf3_3", "tb26P_PresShelf3_4",
                                            "tb26P_PresShelf3_5", "tb26P_PresShelf3_6",
                                            "tb26P_PresShelf4_1", "tb26P_PresShelf4_2", "tb26P_PresShelf4_3", "tb26P_PresShelf4_4",
                                            "tb26P_PresShelf4_5", "tb26P_PresShelf4_6"
                                            };
            }



            string[] presences = RsltPresence;
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                UpdateFoupPresence ph = new UpdateFoupPresence(updateFoupPresenceByFoups);
                form.BeginInvoke(ph);
            }
            else
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    TextBox tb = form.Controls.Find(positions[i], true).FirstOrDefault() as TextBox;
                    if (tb == null)
                        continue;
                    if (presences[i].Equals("1"))
                        tb.BackColor = Color.LimeGreen;//有Foup
                    else if (presences[i].Equals("0"))
                        tb.BackColor = SystemColors.Control;//無Foup
                    else
                        tb.BackColor = Color.Red;//在席異常
                    ToolTip hint = new ToolTip();
                    hint.AutomaticDelay = 50;
                    hint.AutoPopDelay = 20000;
                    hint.SetToolTip(tb, presences[i]);
                }
            }
        }

        internal static void updateFoupPresenceByBoard()
        {
            string[] shelfs = new string[] { "tbPresShelf1_1", "tbPresShelf1_2", "tbPresShelf1_3",
            "tbPresShelf2_1","tbPresILPT1","tbPresILPT2",
            "tbPresShelf3_1", "tbPresShelf3_2", "tbPresShelf3_3",
            "tbPresShelf4_1", "tbPresShelf4_2", "tbPresShelf4_3",
            "tbPresShelf5_1", "tbPresShelf5_2", "tbPresShelf5_3",
            "tbPresShelf6_1", "tbPresShelf6_2", "tbPresShelf6_3",
            };
            string[] presences = RsltPresence;
            Form form = Application.OpenForms["FormMain"];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                UpdateFoupPresence ph = new UpdateFoupPresence(updateFoupPresenceByBoard);
                form.BeginInvoke(ph);
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    TextBox tb = form.Controls.Find(shelfs[i], true).FirstOrDefault() as TextBox;
                    if (tb == null)
                        continue;
                    if (presences[i].Replace("1", "").Length == 0)
                        tb.BackColor = Color.LimeGreen;//有Foup
                    else if (presences[i].Replace("0", "").Length == 0)
                        tb.BackColor = SystemColors.Control;//無Foup
                    else
                        tb.BackColor = Color.Red;//在席異常
                    ToolTip hint = new ToolTip();
                    hint.SetToolTip(tb, presences[i]);
                }
            }
        }
        internal static void updateFoupPresenceByFoups(string[] value)
        {
            RsltPresence = value;
            updateFoupPresenceByFoups();
        }

        internal static void setShowCommand(string tabName)
        {

            Form form = Application.OpenForms["FormIO"];
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                SetShowCommand ph = new SetShowCommand(setShowCommand);
                form.BeginInvoke(ph, tabName);
            }
            else
            {
                CheckBox cbAutoRefresh = form.Controls.Find("cbAutoRefresh", true).FirstOrDefault() as CheckBox;
                if (cbAutoRefresh == null)
                    return;
                if (!tabName.Equals("tabIO"))
                {
                    cbAutoRefresh.Checked = false;//取消IO自動更新
                    isShowCmd = true;
                }else if (!cbAutoRefresh.Checked)
                {
                    isShowCmd = true;
                }
                else
                {
                    isShowCmd = false;
                }
            }
        }
    }
}
