using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniStockerIV
{
    class CmdScript
    {
        public CmdScript() { }

        /// <summary>
        /// 指令順序。
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// 指令內容。
        /// </summary>
        public string Command { get; set; }

        public static explicit operator CmdScript(int v)
        {
            throw new NotImplementedException();
        }
    }
}
