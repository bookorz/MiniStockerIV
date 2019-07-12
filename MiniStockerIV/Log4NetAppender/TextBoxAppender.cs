using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniStockerIV.Log4NetAppender
{
    public class TextBoxAppender : AppenderSkeleton
    {
        private RichTextBox _textBox;
        public string FormName { get; set; }
        public string TextBoxName { get; set; }
        delegate void PrintHandler(RichTextBox tb, string text);
        public static bool ShowDebug = false;
        public static bool ShowInfo = true;


        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_textBox == null)
            {
                if (String.IsNullOrEmpty(FormName) ||
                    String.IsNullOrEmpty(TextBoxName))
                    return;

                Form form = Application.OpenForms[FormName];
                if (form == null)
                {

                    return;
                }
                _textBox = form.Controls.Find(TextBoxName, true).FirstOrDefault() as RichTextBox;
                if (_textBox == null)
                    return;

                form.FormClosing += (s, e) => _textBox = null;
            }

            //Thread Td = new Thread(() => Print(_textBox, loggingEvent.RenderedMessage + Environment.NewLine));
            //Td.IsBackground = true;
            //Td.Start();

            switch (loggingEvent.Level.DisplayName)
            {
                case "DEBUG":
                    if (!ShowDebug)
                    {
                        return;
                    }
                    break;
                case "INFO":
                    if (!ShowInfo)
                    {
                        return;
                    }
                    break;
            }


            //Print(_textBox, loggingEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "[" + loggingEvent.Level.DisplayName + "] " + loggingEvent.RenderedMessage + Environment.NewLine);
            Print(_textBox, loggingEvent.RenderedMessage + Environment.NewLine);

        }

        public static void Print(RichTextBox tb, string msg)
        {
            //判斷這個TextBox的物件是否在同一個執行緒上
            if (tb.InvokeRequired)
            {
                //當InvokeRequired為true時，表示在不同的執行緒上，所以進行委派的動作!!
                PrintHandler ph = new PrintHandler(Print);
                tb.BeginInvoke(ph, tb, msg);
            }
            else
            {
                //表示在同一個執行緒上了，所以可以正常的呼叫到這個TextBox物件
                if (tb.Text.Length > 13000)
                {
                    tb.Text = tb.Text.Substring(tb.Text.Length - 7000);
                }
                tb.SelectionStart = tb.TextLength;
                tb.SelectionLength = 0;


                if (msg.ToUpper().Contains("FIN"))
                {
                    tb.SelectionColor = Color.Green;
                }
                else if (msg.ToUpper().Contains("RECEIVE"))
                {
                    tb.SelectionColor = Color.Blue;
                }
                else if (msg.ToUpper().Contains("CMD") || msg.ToUpper().Contains("GET") || msg.ToUpper().Contains("SET"))
                {
                    tb.SelectionColor = Color.Coral;
                }
                else if (msg.ToUpper().Contains("MCR"))
                {
                    tb.SelectionColor = Color.Coral;
                }
                else if (msg.ToUpper().Contains("異常描述"))
                {
                    tb.SelectionColor = Color.Red;
                }
                else
                {
                    tb.SelectionColor = Color.DimGray;
                }

                tb.AppendText(msg);
                tb.ScrollToCaret();
                if (tb.Lines.Length > 10000)//只保留最後5千行
                {
                    tb.Select(0, tb.GetFirstCharIndexFromLine(tb.Lines.Length - 5000));
                    tb.SelectedText = "";
                }
            }
        }
    }
}
