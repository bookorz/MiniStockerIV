namespace MiniStockerIV
{
    partial class FormIO
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbIO_3 = new System.Windows.Forms.GroupBox();
            this.btnQryIO3 = new System.Windows.Forms.Button();
            this.tabIOControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.Category3_I_List = new System.Windows.Forms.Panel();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.Category3_O_List = new System.Windows.Forms.Panel();
            this.cbUseIOName = new System.Windows.Forms.CheckBox();
            this.gbIO_2 = new System.Windows.Forms.GroupBox();
            this.btnQryIO2 = new System.Windows.Forms.Button();
            this.tabIOControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Category2_I_List = new System.Windows.Forms.Panel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.Category2_O_List = new System.Windows.Forms.Panel();
            this.gbIO_1 = new System.Windows.Forms.GroupBox();
            this.btnQryIO1 = new System.Windows.Forms.Button();
            this.tabIOControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Category1_I_List = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Category1_O_List = new System.Windows.Forms.Panel();
            this.hint = new System.Windows.Forms.ToolTip(this.components);
            this.cbAutoRefresh = new System.Windows.Forms.CheckBox();
            this.cbUpdInterval = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbIO_3.SuspendLayout();
            this.tabIOControl3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.gbIO_2.SuspendLayout();
            this.tabIOControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.gbIO_1.SuspendLayout();
            this.tabIOControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbIO_3
            // 
            this.gbIO_3.Controls.Add(this.btnQryIO3);
            this.gbIO_3.Controls.Add(this.tabIOControl3);
            this.gbIO_3.Location = new System.Drawing.Point(654, 19);
            this.gbIO_3.Name = "gbIO_3";
            this.gbIO_3.Size = new System.Drawing.Size(318, 606);
            this.gbIO_3.TabIndex = 5;
            this.gbIO_3.TabStop = false;
            this.gbIO_3.Text = "CTU/PTZ";
            // 
            // btnQryIO3
            // 
            this.btnQryIO3.Location = new System.Drawing.Point(218, 27);
            this.btnQryIO3.Name = "btnQryIO3";
            this.btnQryIO3.Size = new System.Drawing.Size(78, 31);
            this.btnQryIO3.TabIndex = 0;
            this.btnQryIO3.Text = "Refresh";
            this.btnQryIO3.UseVisualStyleBackColor = true;
            this.btnQryIO3.Click += new System.EventHandler(this.btnQryIO3_Click);
            // 
            // tabIOControl3
            // 
            this.tabIOControl3.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabIOControl3.Controls.Add(this.tabPage5);
            this.tabIOControl3.Controls.Add(this.tabPage6);
            this.tabIOControl3.Location = new System.Drawing.Point(7, 28);
            this.tabIOControl3.Name = "tabIOControl3";
            this.tabIOControl3.SelectedIndex = 0;
            this.tabIOControl3.Size = new System.Drawing.Size(308, 572);
            this.tabIOControl3.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.Category3_I_List);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(300, 543);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "IN";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // Category3_I_List
            // 
            this.Category3_I_List.AutoScroll = true;
            this.Category3_I_List.Location = new System.Drawing.Point(3, 5);
            this.Category3_I_List.Name = "Category3_I_List";
            this.Category3_I_List.Size = new System.Drawing.Size(294, 526);
            this.Category3_I_List.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.Category3_O_List);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(300, 543);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "OUT";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // Category3_O_List
            // 
            this.Category3_O_List.AutoScroll = true;
            this.Category3_O_List.Location = new System.Drawing.Point(3, 4);
            this.Category3_O_List.Name = "Category3_O_List";
            this.Category3_O_List.Size = new System.Drawing.Size(314, 526);
            this.Category3_O_List.TabIndex = 1;
            // 
            // cbUseIOName
            // 
            this.cbUseIOName.AutoSize = true;
            this.cbUseIOName.Location = new System.Drawing.Point(233, 8);
            this.cbUseIOName.Name = "cbUseIOName";
            this.cbUseIOName.Size = new System.Drawing.Size(86, 16);
            this.cbUseIOName.TabIndex = 2;
            this.cbUseIOName.Text = "Use IO Name";
            this.cbUseIOName.UseVisualStyleBackColor = true;
            this.cbUseIOName.Visible = false;
            // 
            // gbIO_2
            // 
            this.gbIO_2.Controls.Add(this.btnQryIO2);
            this.gbIO_2.Controls.Add(this.tabIOControl2);
            this.gbIO_2.Location = new System.Drawing.Point(325, 19);
            this.gbIO_2.Name = "gbIO_2";
            this.gbIO_2.Size = new System.Drawing.Size(326, 606);
            this.gbIO_2.TabIndex = 4;
            this.gbIO_2.TabStop = false;
            this.gbIO_2.Text = "WHR";
            // 
            // btnQryIO2
            // 
            this.btnQryIO2.Location = new System.Drawing.Point(218, 27);
            this.btnQryIO2.Name = "btnQryIO2";
            this.btnQryIO2.Size = new System.Drawing.Size(78, 31);
            this.btnQryIO2.TabIndex = 0;
            this.btnQryIO2.Text = "Refresh";
            this.btnQryIO2.UseVisualStyleBackColor = true;
            this.btnQryIO2.Click += new System.EventHandler(this.btnQryIO2_Click);
            // 
            // tabIOControl2
            // 
            this.tabIOControl2.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabIOControl2.Controls.Add(this.tabPage3);
            this.tabIOControl2.Controls.Add(this.tabPage4);
            this.tabIOControl2.Location = new System.Drawing.Point(7, 28);
            this.tabIOControl2.Name = "tabIOControl2";
            this.tabIOControl2.SelectedIndex = 0;
            this.tabIOControl2.Size = new System.Drawing.Size(315, 572);
            this.tabIOControl2.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Category2_I_List);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(307, 543);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "IN";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Category2_I_List
            // 
            this.Category2_I_List.AutoScroll = true;
            this.Category2_I_List.Location = new System.Drawing.Point(3, 5);
            this.Category2_I_List.Name = "Category2_I_List";
            this.Category2_I_List.Size = new System.Drawing.Size(299, 526);
            this.Category2_I_List.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.Category2_O_List);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(307, 543);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "OUT";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Category2_O_List
            // 
            this.Category2_O_List.AutoScroll = true;
            this.Category2_O_List.Location = new System.Drawing.Point(3, 4);
            this.Category2_O_List.Name = "Category2_O_List";
            this.Category2_O_List.Size = new System.Drawing.Size(298, 526);
            this.Category2_O_List.TabIndex = 1;
            // 
            // gbIO_1
            // 
            this.gbIO_1.Controls.Add(this.btnQryIO1);
            this.gbIO_1.Controls.Add(this.tabIOControl1);
            this.gbIO_1.Location = new System.Drawing.Point(-4, 19);
            this.gbIO_1.Name = "gbIO_1";
            this.gbIO_1.Size = new System.Drawing.Size(326, 606);
            this.gbIO_1.TabIndex = 3;
            this.gbIO_1.TabStop = false;
            this.gbIO_1.Text = "Stocker";
            // 
            // btnQryIO1
            // 
            this.btnQryIO1.Location = new System.Drawing.Point(238, 27);
            this.btnQryIO1.Name = "btnQryIO1";
            this.btnQryIO1.Size = new System.Drawing.Size(78, 31);
            this.btnQryIO1.TabIndex = 0;
            this.btnQryIO1.Text = "Refresh";
            this.btnQryIO1.UseVisualStyleBackColor = true;
            this.btnQryIO1.Click += new System.EventHandler(this.btnQryIO1_Click);
            // 
            // tabIOControl1
            // 
            this.tabIOControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabIOControl1.Controls.Add(this.tabPage1);
            this.tabIOControl1.Controls.Add(this.tabPage2);
            this.tabIOControl1.Location = new System.Drawing.Point(7, 28);
            this.tabIOControl1.Name = "tabIOControl1";
            this.tabIOControl1.SelectedIndex = 0;
            this.tabIOControl1.Size = new System.Drawing.Size(313, 572);
            this.tabIOControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Category1_I_List);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(305, 543);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "IN";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Category1_I_List
            // 
            this.Category1_I_List.AutoScroll = true;
            this.Category1_I_List.Location = new System.Drawing.Point(3, 5);
            this.Category1_I_List.Name = "Category1_I_List";
            this.Category1_I_List.Size = new System.Drawing.Size(296, 526);
            this.Category1_I_List.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.Category1_O_List);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(305, 543);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "OUT";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Category1_O_List
            // 
            this.Category1_O_List.AutoScroll = true;
            this.Category1_O_List.Location = new System.Drawing.Point(3, 4);
            this.Category1_O_List.Name = "Category1_O_List";
            this.Category1_O_List.Size = new System.Drawing.Size(296, 526);
            this.Category1_O_List.TabIndex = 1;
            // 
            // cbAutoRefresh
            // 
            this.cbAutoRefresh.AutoSize = true;
            this.cbAutoRefresh.Font = new System.Drawing.Font("新細明體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbAutoRefresh.Location = new System.Drawing.Point(822, 3);
            this.cbAutoRefresh.Name = "cbAutoRefresh";
            this.cbAutoRefresh.Size = new System.Drawing.Size(68, 15);
            this.cbAutoRefresh.TabIndex = 6;
            this.cbAutoRefresh.Text = "自動更新";
            this.cbAutoRefresh.UseVisualStyleBackColor = true;
            this.cbAutoRefresh.CheckedChanged += new System.EventHandler(this.cbAutoRefresh_CheckedChanged);
            // 
            // cbUpdInterval
            // 
            this.cbUpdInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpdInterval.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbUpdInterval.FormattingEnabled = true;
            this.cbUpdInterval.Items.AddRange(new object[] {
            "0.3",
            "0.5",
            "1",
            "2",
            "3",
            "5",
            "10"});
            this.cbUpdInterval.Location = new System.Drawing.Point(891, 1);
            this.cbUpdInterval.Name = "cbUpdInterval";
            this.cbUpdInterval.Size = new System.Drawing.Size(46, 20);
            this.cbUpdInterval.TabIndex = 7;
            this.cbUpdInterval.SelectedIndexChanged += new System.EventHandler(this.cbUpdInterval_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(943, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 11);
            this.label1.TabIndex = 8;
            this.label1.Text = "秒";
            // 
            // FormIO
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(971, 644);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbUpdInterval);
            this.Controls.Add(this.cbAutoRefresh);
            this.Controls.Add(this.cbUseIOName);
            this.Controls.Add(this.gbIO_3);
            this.Controls.Add(this.gbIO_2);
            this.Controls.Add(this.gbIO_1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormIO";
            this.Text = "FormIO";
            this.Load += new System.EventHandler(this.FormIO_Load);
            this.gbIO_3.ResumeLayout(false);
            this.tabIOControl3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.gbIO_2.ResumeLayout(false);
            this.tabIOControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.gbIO_1.ResumeLayout(false);
            this.tabIOControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbIO_3;
        private System.Windows.Forms.Button btnQryIO3;
        private System.Windows.Forms.TabControl tabIOControl3;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Panel Category3_I_List;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Panel Category3_O_List;
        private System.Windows.Forms.CheckBox cbUseIOName;
        private System.Windows.Forms.GroupBox gbIO_2;
        private System.Windows.Forms.Button btnQryIO2;
        private System.Windows.Forms.TabControl tabIOControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel Category2_I_List;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel Category2_O_List;
        private System.Windows.Forms.GroupBox gbIO_1;
        private System.Windows.Forms.Button btnQryIO1;
        private System.Windows.Forms.TabControl tabIOControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel Category1_I_List;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel Category1_O_List;
        private System.Windows.Forms.ToolTip hint;
        private System.Windows.Forms.CheckBox cbAutoRefresh;
        private System.Windows.Forms.ComboBox cbUpdInterval;
        private System.Windows.Forms.Label label1;
    }
}