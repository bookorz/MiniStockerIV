using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniStockerIV
{
    class Command
    {
        public static BindingList<CmdScript> oCmdScript = new BindingList<CmdScript>();
        public static IEnumerable<CmdScript> getCmdList()
        {
            return (IEnumerable<CmdScript>)Command.oCmdScript.ToList();
        }
        /// <summary>
        ///     Add put wafer command to the script。
        /// </summary>
        /// <param name="point">放片目的地點位。</param>
        /// <param name="arm">放片使用手臂。ARM1~ARM3</param>
        /// <returns>指令字串</returns>
        public static void Unload(string point, string arm)
        {
            //"MOV:UNLOAD/ALIGN1/ARM1;"
            StringBuilder cmd = new StringBuilder();
            cmd.Append("MOV:UNLOAD/");
            cmd.Append(point);
            cmd.Append("/");
            cmd.Append(arm);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
        }
        /// <summary>
        ///     Add get wafer command to the script。
        /// </summary>
        /// <param name="point">取片來源點位。</param>
        /// <param name="arm">>取片使用手臂。ARM1~ARM3 </param>
        /// <returns></returns>
        public static void Load(string point, string arm)
        {
            //"MOV:UNLOAD/ALIGN1/ARM1;"
            StringBuilder cmd = new StringBuilder();
            cmd.Append("MOV:LOAD/");
            cmd.Append(point);
            cmd.Append("/");
            cmd.Append(arm);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
        }


        /// <summary>
        /// Clamp
        /// </summary>
        /// <param name="point"></param>
        /// <param name="action">ON, OFF</param>
        public static void Clamp(string point, string action)
        {
            //SET:CLAMP/ALIGN1/ON;
            StringBuilder cmd = new StringBuilder();
            cmd.Append("SET:CLAMP/");
            cmd.Append(point);
            cmd.Append("/");
            cmd.Append(action);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
        }
        /// <summary>
        /// Align 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        public static void Align(string point, string angle)
        {
            StringBuilder cmd = new StringBuilder();
            //SET:ALIGN/ALIGN/D******;
            cmd.Append("SET:ALIGN/");
            cmd.Append(point);
            cmd.Append("/");
            cmd.Append("D" + angle);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
            //MOV:ALIGN/ALIGN1;
            cmd.Clear();
            cmd.Append("MOV:ALIGN/");
            cmd.Append(point);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
        }

        /// <summary>
        /// Home
        /// </summary>
        /// <param name="point"></param>
        public static void Home(string point)
        {
            StringBuilder cmd = new StringBuilder();
            //MOV:HOME/ALIGN1;
            cmd.Append("MOV:HOME/");
            cmd.Append(point);
            cmd.Append(";");
            addScriptCmd(cmd.ToString());
        }

        public static void addScriptCmd(string cmd)
        {
            int seq = Command.oCmdScript.Count + 1;
            Command.oCmdScript.Add(new CmdScript { Seq = seq, Command = cmd });
        }
    }
}
