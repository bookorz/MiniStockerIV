using SanwaMarco.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniStockerIV
{
    public class CtrlManager
    {
        //Controller
        public static Dictionary<string, TcpCommClient> controllers = new Dictionary<string, TcpCommClient>();

    }
}
