using MiniStockerIV.UI_Update;
using SanwaMarco.Comm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniStockerIV
{
    public partial class FormIO : Form
    {
        Boolean isAutoRefresh = false;
        int currentY_I = 15;//Input Y 軸座標
        int currentY_O = 15;//Output Y 軸座標

        public FormIO()
        {
            InitializeComponent();
        }

        private void FormIO_Load(object sender, EventArgs e)
        {
            //設定IO模組名稱
            gbIO_1.Text = ConfigurationManager.AppSettings["IO_Catrgory_1"];
            gbIO_2.Text = ConfigurationManager.AppSettings["IO_Catrgory_2"];
            gbIO_3.Text = ConfigurationManager.AppSettings["IO_Catrgory_3"];
            //產生IO物件
            Initial_I_O();
        }
        private void On_IO_Click(object sender, EventArgs e)
        {
            string key = ((Button)sender).Name;
            string type = key.Substring(key.LastIndexOf("_") + 1);
            key = key.Substring(0, key.LastIndexOf("_"));
            string address = key.Split('_')[0];
            string io = key.Split('_')[1];
            string ioCmd = cbUseIOName.Checked ? "MCR:SET_NMEIO/" : "MCR:SET_RELIO/";
            string cmd = "$" + address + ioCmd + io + "/";
            switch (type.ToUpper())
            {
                case "ON":
                    FormMainUpdate.Update_IO(key, "ON");
                    cmd = cmd + "1;";
                    break;

                case "OFF":
                    FormMainUpdate.Update_IO(key, "OFF");
                    cmd = cmd + "0;";
                    break;
            }
            sendCommand(cmd);
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
                device.Send(cmd + "\r"); //暫時先不送指令, 先跳
            }
        }
        private void updateLog(object data)
        {
            FormMainUpdate.Log(data.ToString());
            //FormMainUpdate.LogUpdate(data.ToString());
        }
        private void logUpdate(string log)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback(updateLog), log);
            updateLog(log);
        }
        private void InsertIO(string AddressNo, string ID, string Name, string desc, string Type, Panel P)
        {
            int currentY = 0;
            if (Type.ToUpper().Equals("IN"))
            {
                currentY = currentY_I;
                currentY_I += 30;
            }
            else
            {
                currentY = currentY_O;
                currentY_O += 30;
            }
            Label value = new Label();
            if (cbUseIOName.Checked)
                //value.Name = AddressNo + "_" + Name.Replace("-", "_") + "_" + Type;
                value.Name = AddressNo + "_" + Name + "_" + Type;
            else
                value.Name = AddressNo + "_" + ID + "_" + Type;
            value.Text = "■";//"●"
            //value.ForeColor = Color.Red;
            value.ForeColor = Color.DimGray;//預設為未知
            value.Location = new System.Drawing.Point(0, currentY);
            value.Font = new Font(new FontFamily(value.Font.Name), 12, value.Font.Style);
            //value.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            value.Size = new System.Drawing.Size(20, 20);
            P.Controls.Add(value);

            Label id = new Label();
            if (cbUseIOName.Checked)
                //value.Name = AddressNo + "_" + Name.Replace("-", "_") + "_" + Type;
                id.Name = AddressNo + "_" + Name + "_" + Type + "_ID";
            else
                id.Name = AddressNo + "_" + ID + "_" + Type + "_ID";
            //value.Text = "■";//"●"
            id.Text = ID + "(" + Name + ")";
            id.Location = new System.Drawing.Point(20, currentY);
            if (Type.ToUpper().Equals("IN"))
            {
                id.Size = new System.Drawing.Size(230, 20);
            }
            else
            {
                id.Size = new System.Drawing.Size(180, 20);
            }
            //id.Font = new Font(new FontFamily(id.Font.Name), 12, id.Font.Style);
            id.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            hint.SetToolTip(id, desc);
            P.Controls.Add(id);

            if (Type.ToUpper().Equals("OUT"))
            {
                Button On = new Button();
                On.Text = "On";
                if (cbUseIOName.Checked)
                    //On.Name = AddressNo + "_" + Name.Replace("-", "_") + "_" + Type + "_ON";
                    On.Name = AddressNo + "_" + Name + "_" + Type + "_ON";
                else
                    On.Name = AddressNo + "_" + ID + "_" + Type + "_ON";
                //On.Name = AddressNo + "_" + ID + "_" + Type + "_ON";
                On.Click += On_IO_Click;
                On.Location = new System.Drawing.Point(200, currentY);
                //On.Font = new Font(new FontFamily(On.Font.Name), 9, On.Font.Style);
                On.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                On.Size = new System.Drawing.Size(38, 25);
                P.Controls.Add(On);

                Button Off = new Button();
                Off.Text = "Off";
                if (cbUseIOName.Checked)
                    //Off.Name = AddressNo + "_" + Name.Replace("-", "_") + "_" + Type + "_OFF";
                    Off.Name = AddressNo + "_" + Name + "_" + Type + "_OFF";
                else
                    Off.Name = AddressNo + "_" + ID + "_" + Type + "_OFF";
                //Off.Name = AddressNo + "_" + ID + "_" + Type + "_OFF";
                Off.Click += On_IO_Click;
                Off.Location = new System.Drawing.Point(240, currentY);
                //Off.Font = new Font(new FontFamily(On.Font.Name), 9, On.Font.Style);
                Off.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                Off.Size = new System.Drawing.Size(38, 25);
                P.Controls.Add(Off);
            }

        }
        private void Initial_I_O()
        {

            string line;
            string filename;

            //System.IO.StreamReader file =  new System.IO.StreamReader(@"Stocker_IO.csv");
            filename = ConfigurationManager.AppSettings["IO_Define_1"];
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            Category1_I_List.Controls.Clear();
            Category1_O_List.Controls.Clear();
            Category2_I_List.Controls.Clear();
            Category2_O_List.Controls.Clear();
            Category3_I_List.Controls.Clear();
            Category3_O_List.Controls.Clear();
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    string[] raw = line.Split(',');
                    if (raw[4].ToUpper().Equals("IN"))
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category1_I_List);
                    }
                    else
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category1_O_List);
                    }

                }
                catch (Exception e)
                {
                    //MessageBox.Show("Stocker_IO.csv read err:" + line + "\n" + e.StackTrace);
                    MessageBox.Show("Stocker_IO_Name.csv read err:" + line + "\n" + e.StackTrace);
                }
            }

            file.Close();
            currentY_I = 15;
            currentY_O = 15;
            filename = ConfigurationManager.AppSettings["IO_Define_2"];
            //file =  new System.IO.StreamReader(@"WHR_IO.csv");
            file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    string[] raw = line.Split(',');
                    if (raw[4].ToUpper().Equals("IN"))
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category2_I_List);
                    }
                    else
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category2_O_List);
                    }

                }
                catch (Exception e)
                {
                    //MessageBox.Show("WHR_IO.csv read err:" + line + "\n" + e.StackTrace);
                    MessageBox.Show("WHR_IO_Name.csv read err:" + line + "\n" + e.StackTrace);
                }
            }

            file.Close();
            currentY_I = 15;
            currentY_O = 15;
            filename = ConfigurationManager.AppSettings["IO_Define_3"];
            //file =  new System.IO.StreamReader(@"CTU_PTZ_IO.csv");
            file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    string[] raw = line.Split(',');
                    if (raw[4].ToUpper().Equals("IN"))
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category3_I_List);
                    }
                    else
                    {
                        InsertIO(raw[0], raw[1], raw[2], raw[3], raw[4], Category3_O_List);
                    }

                }
                catch (Exception e)
                {
                    //MessageBox.Show("CTU_PTZ_IO.csv read err:" + line + "\n" + e.StackTrace);
                    MessageBox.Show("CTU_PTZ_IO_Name.csv read err:" + line + "\n" + e.StackTrace);
                }
            }

            file.Close();


            //for (int i = 0; i < 100; i++)
            //{
            //    InsertIO("1", "9528"+i, "Buzzer1"+i, "OUT", Stocker_IO_List);
            //}
        }
        
        private void QryIO(TabControl tc, Panel p_in, Panel p_out)
        {
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
                        getRELIO(address, rio);
                    }
                    //else
                    //{
                    //    MessageBox.Show(foo.Name);
                    //}
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
                        getRELIO(address, rio);
                    }
                }
            }
        }

        private void getNMEIO(string address, string name)
        {
            string cmd = "$" + address + "MCR:GET_NMEIO/" + name + ";";
            sendCommand(cmd);
        }

        private void getRELIO(string address, string id)
        {
            string cmd = "$" + address + "MCR:GET_RELIO/" + id + ";";
            sendCommand(cmd);
        }
        private void setRELIO(string address, string id, string value)
        {
            string cmd = "$" + address + "MCR:SET_RELIO/" + id + "/" + value + ";";
            sendCommand(cmd);
        }
        private void setRELIOS(string address, string id, string value)
        {
            string cmd = "$" + address + "MCR:SET_RELIOS/" + id + "/" + value + ";";
            sendCommand(cmd);
        }
        private void QryIOByName( TabControl tc, Panel p_in, Panel p_out)
        {
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
                        int start_idx = foo.Text.IndexOf("(") + 1;
                        int length = foo.Text.IndexOf(")") - start_idx;
                        string rio = foo.Text.Substring(start_idx, length);
                        getNMEIO(address, rio);
                    }
                    //else
                    //{
                    //    MessageBox.Show(foo.Name);
                    //}
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
                        int start_idx = foo.Text.IndexOf("(") + 1;
                        int length = foo.Text.IndexOf(")") - start_idx;
                        string rio = foo.Text.Substring(start_idx, length);
                        getNMEIO(address, rio);
                    }
                }
            }
        }
        

        private void cbUseIOName_CheckedChanged(object sender, EventArgs e)
        {
            currentY_I = 15;
            currentY_O = 15;
            //Initial_I_O();
        }

        private void btnQryIO1_Click(object sender, EventArgs e)
        {
            if (tabIOControl1.SelectedTab.Text.Equals("IN") || tabIOControl1.SelectedTab.Text.Equals("OUT"))
            {
                string cmd = "$1MCR:I7565DNM_REFRESH;";
                sendCommand(cmd);
                if (cbUseIOName.Checked)
                    //QryIOByName("1", tabIOControl1, Category1_I_List, Category1_O_List);
                    QryIOByName(tabIOControl1, Category1_I_List, Category1_O_List);
                else
                    QryIO(tabIOControl1, Category1_I_List, Category1_O_List);
            }
            else
            {
                Dictionary<string, string> ioMap = new Dictionary<string, string>();
                foreach (Control foo in Category1_O_List.Controls)
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
                        if (ioMap.ContainsKey(address))
                            ioMap[address] = ioMap[address] + ";" + rio;
                        else
                            ioMap[address] = rio;
                        string key = foo.Name.Substring(0, foo.Name.LastIndexOf("_"));
                        FormMainUpdate.Update_IO(key, "OFF");
                    }
                }
                foreach (KeyValuePair<string, string> ios in ioMap)
                {
                    setRELIOS(ios.Key, ios.Value, "0");//clear output
                }
            }
        }

        private void btnQryIO2_Click(object sender, EventArgs e)
        {
            if (tabIOControl2.SelectedTab.Text.Equals("IN") || tabIOControl2.SelectedTab.Text.Equals("OUT"))
            {
                string cmd = "$1MCR:I7565DNM_REFRESH;";
                sendCommand(cmd);
                if (cbUseIOName.Checked)
                    QryIOByName(tabIOControl2, Category2_I_List, Category2_O_List);
                //QryIOByName("2", tabIOControl2, Category2_I_List, Category2_O_List);
                else
                    QryIO(tabIOControl2, Category2_I_List, Category2_O_List);
            }
            else
            {
                Dictionary<string, string> ioMap = new Dictionary<string, string>();
                foreach (Control foo in Category2_O_List.Controls)
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
                        if (ioMap.ContainsKey(address))
                            ioMap[address] = ioMap[address] + ";" + rio;
                        else
                            ioMap[address] = rio;
                        string key = foo.Name.Substring(0, foo.Name.LastIndexOf("_"));
                        FormMainUpdate.Update_IO(key, "OFF");
                    }
                }
                foreach (KeyValuePair<string, string> ios in ioMap)
                {
                    setRELIOS(ios.Key, ios.Value, "0");//clear output
                }
            }
        }

        private void btnQryIO3_Click(object sender, EventArgs e)
        {
            if (tabIOControl3.SelectedTab.Text.Equals("IN") || tabIOControl3.SelectedTab.Text.Equals("OUT"))
            {
                string cmd = "$1MCR:I7565DNM_REFRESH;";
                sendCommand(cmd);
                if (cbUseIOName.Checked)
                    QryIOByName(tabIOControl3, Category3_I_List, Category3_O_List);
                //QryIOByName("3", tabIOControl3, Category3_I_List, Category3_O_List);
                else
                    QryIO(tabIOControl3, Category3_I_List, Category3_O_List);
            }
            else
            {
                Dictionary<string, string> ioMap = new Dictionary<string, string>();
                foreach (Control foo in Category3_O_List.Controls)
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
                        if (ioMap.ContainsKey(address))
                            ioMap[address] = ioMap[address] + ";" + rio;
                        else
                            ioMap[address] = rio;
                        string key = foo.Name.Substring(0, foo.Name.LastIndexOf("_"));
                        FormMainUpdate.Update_IO(key, "OFF");
                    }
                }
                foreach (KeyValuePair<string, string> ios in ioMap)
                {
                    setRELIOS(ios.Key, ios.Value, "0");//clear output
                }
            }
        }

        private void cbAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            isAutoRefresh = cbAutoRefresh.Checked;
            FormMainUpdate.isShowCmd = !cbAutoRefresh.Checked;
            if (cbAutoRefresh.Checked)
            {
                cbUpdInterval.SelectedItem = "0.5";
                ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshIO));
                //ArrayList cmds1 = FormMainUpdate.getIORefreshCmds("tabIOControl1", "Category1_I_List", "Category1_O_List");
                //foreach (string item in cmds1)
                //{
                //    sendCommand(item);
                //}
            }
            else
            {
                cbUpdInterval.SelectedIndex = -1;
            }
        }
        private void RefreshIO(object obj)
        {
            while (isAutoRefresh)
            {
                try
                {
                    Thread.Sleep(20000);
                    string cmd = "$1MCR:I7565DNM_REFRESH;";
                    sendCommand(cmd);
                    ArrayList cmds1 = FormMainUpdate.getIORefreshCmds("tabIOControl1", "Category1_I_List", "Category1_O_List");
                    if (cmds1 != null)
                    {
                        foreach (string item in cmds1)
                        {
                            sendCommand(item);
                        }
                    }
                    ArrayList cmds2 = FormMainUpdate.getIORefreshCmds("tabIOControl2", "Category2_I_List", "Category2_O_List");
                    if (cmds2 != null)
                    {
                        foreach (string item in cmds2)
                        {
                            sendCommand(item);
                        }
                    }
                    ArrayList cmds3 = FormMainUpdate.getIORefreshCmds("tabIOControl3", "Category3_I_List", "Category3_O_List");
                    if (cmds3 != null)
                    {
                        foreach (string item in cmds3)
                        {
                            sendCommand(item);
                        }
                    }
                    //FormMainUpdate.cmdList.Clear();
                    //btnQryIO1_Click(null, null);
                    //btnQryIO2_Click(null, null);
                    //btnQryIO3_Click(null, null);
                }
                catch (Exception e)
                {
                    //logger.Error(e.StackTrace);
                    Console.Write(e.StackTrace);
                }
            }

        }
    }
}
