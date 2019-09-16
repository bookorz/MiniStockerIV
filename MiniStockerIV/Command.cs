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
        
        public static void addScriptCmd(string cmd)
        {
            int seq = Command.oCmdScript.Count + 1;
            Command.oCmdScript.Add(new CmdScript { Seq = seq, Command = cmd });
        }
    }
}
