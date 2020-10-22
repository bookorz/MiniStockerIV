namespace MiniStockerIV
{
    partial class FormMCType
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
            this.lbSetMCType = new System.Windows.Forms.Label();
            this.cmbSetMCType = new System.Windows.Forms.ComboBox();
            this.btnSetMCType = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbSetMCType
            // 
            this.lbSetMCType.AutoSize = true;
            this.lbSetMCType.Font = new System.Drawing.Font("Consolas", 14.25F);
            this.lbSetMCType.Location = new System.Drawing.Point(33, 35);
            this.lbSetMCType.Name = "lbSetMCType";
            this.lbSetMCType.Size = new System.Drawing.Size(233, 28);
            this.lbSetMCType.TabIndex = 0;
            this.lbSetMCType.Text = "Set Machine Type:";
            // 
            // cmbSetMCType
            // 
            this.cmbSetMCType.Font = new System.Drawing.Font("Consolas", 14.25F);
            this.cmbSetMCType.FormattingEnabled = true;
            this.cmbSetMCType.Items.AddRange(new object[] {
            "MiniStocker(18 port)",
            "MiniStocker(26 port)"});
            this.cmbSetMCType.Location = new System.Drawing.Point(38, 82);
            this.cmbSetMCType.Name = "cmbSetMCType";
            this.cmbSetMCType.Size = new System.Drawing.Size(339, 36);
            this.cmbSetMCType.TabIndex = 1;
            // 
            // btnSetMCType
            // 
            this.btnSetMCType.Font = new System.Drawing.Font("Consolas", 14.25F);
            this.btnSetMCType.Location = new System.Drawing.Point(292, 139);
            this.btnSetMCType.Name = "btnSetMCType";
            this.btnSetMCType.Size = new System.Drawing.Size(85, 38);
            this.btnSetMCType.TabIndex = 2;
            this.btnSetMCType.Text = "SET";
            this.btnSetMCType.UseVisualStyleBackColor = true;
            this.btnSetMCType.Click += new System.EventHandler(this.btnSetMCType_Click);
            // 
            // FormMCType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 209);
            this.Controls.Add(this.btnSetMCType);
            this.Controls.Add(this.cmbSetMCType);
            this.Controls.Add(this.lbSetMCType);
            this.Name = "FormMCType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Machine Type";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormMCType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbSetMCType;
        private System.Windows.Forms.ComboBox cmbSetMCType;
        private System.Windows.Forms.Button btnSetMCType;
    }
}