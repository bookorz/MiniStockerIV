namespace MiniStockerIV
{
    partial class FormAuto
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
            this.tbMsg = new System.Windows.Forms.RichTextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbMsg
            // 
            this.tbMsg.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMsg.Location = new System.Drawing.Point(12, 12);
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.tbMsg.Size = new System.Drawing.Size(586, 523);
            this.tbMsg.TabIndex = 1;
            this.tbMsg.Text = "";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.OrangeRed;
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnStop.Location = new System.Drawing.Point(621, 327);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(106, 65);
            this.btnStop.TabIndex = 78;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Orange;
            this.btnPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPause.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnPause.Location = new System.Drawing.Point(621, 96);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(106, 65);
            this.btnPause.TabIndex = 78;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.SeaGreen;
            this.btnContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContinue.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContinue.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnContinue.Location = new System.Drawing.Point(621, 206);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(106, 65);
            this.btnContinue.TabIndex = 78;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // FormAuto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 552);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.tbMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormAuto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormAuto";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAuto_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbMsg;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnContinue;
    }
}