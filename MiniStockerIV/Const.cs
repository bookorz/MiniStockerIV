using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniStockerIV
{
    class Const
    {
        //常數宣告
        //Controller
        public const string CONTROLLER_STK = "CTRL_STK";
        public const string CONTROLLER_WHR = "CTRL_WHR";
        public const string CONTROLLER_CTU_PTZ = "CTRL_CTU";
        //位置
        public const string POSITION_ELPT_STOCK_IN = "STOCK_IN";
        public const string POSITION_ELPT_STOCK_OUT = "STOCK_OUT";
        //device
        public const string DEVICE_STK = "STOCK";
        public const string DEVICE_ILPT = "ILPT";
        public const string DEVICE_ELPT = "ELPT";
        public const string DEVICE_FOUP_ROBOT = "FOUP_ROBOT";
        public const string DEVICE_WHR = "WHR";
        public const string DEVICE_CTU = "CTU";
        public const string DEVICE_PTZ = "PTZ";

        public const string STATUS_ON = "ON";
        public const string STATUS_OFF = "OFF";
        public const string STATUS_OPEN = "OPEN";
        public const string STATUS_CLOSE = "CLOSE";

        //電磁閥
        public const string SV_STATUS_ON = "SV_ON_CLAMP_OPEN";
        public const string SV_STATUS_OFF = "SV_OFF_UNCLAMP_CLOSE";
        public const string SV_STK_ELPT1_CLAMP = "ELPT1_CLAMP";
        public const string SV_STK_ELPT2_CLAMP = "ELPT2_CLAMP";
        public const string SV_STK_ELPT1_SHUTTER = "ELPT1_SHUTTER";
        public const string SV_STK_ELPT2_SHUTTER = "ELPT2_SHUTTER";

        //STK Position 
        public const string STK_ELPT1 = "ELPT1";
        public const string STK_ELPT2 = "ELPT2";
        public const string STK_ILPT1 = "ILPT1";
        public const string STK_ILPT2 = "ILPT2";
        public const string STK_SHELF1_1 = "SHELF1-1";
        public const string STK_SHELF1_2 = "SHELF1-2";
        public const string STK_SHELF1_3 = "SHELF1-3";
        public const string STK_SHELF1_4 = "SHELF1-4";
        public const string STK_SHELF1_5 = "SHELF1-5";
        public const string STK_SHELF1_6 = "SHELF1-6";  //for 26Port add 
        public const string STK_SHELF1_7 = "SHELF1-7";  //for 26Port add
        public const string STK_SHELF2_1 = "SHELF2-1";
        public const string STK_SHELF2_2 = "SHELF2-2";
        public const string STK_SHELF2_3 = "SHELF2-3";
        public const string STK_SHELF2_4 = "SHELF2-4";
        public const string STK_SHELF2_5 = "SHELF2-5";
        public const string STK_SHELF2_6 = "SHELF2-6";  //for 26Port add 
        public const string STK_SHELF2_7 = "SHELF2-7";  //for 26Port add
        public const string STK_SHELF3_1 = "SHELF3-1";
        public const string STK_SHELF3_2 = "SHELF3-2";
        public const string STK_SHELF3_3 = "SHELF3-3";
        public const string STK_SHELF3_4 = "SHELF3-4";
        public const string STK_SHELF3_5 = "SHELF3-5";  //for 26Port add 
        public const string STK_SHELF3_6 = "SHELF3-6";  //for 26Port add 
        public const string STK_SHELF4_1 = "SHELF4-1";
        public const string STK_SHELF4_2 = "SHELF4-2";
        public const string STK_SHELF4_3 = "SHELF4-3";
        public const string STK_SHELF4_4 = "SHELF4-4";
        public const string STK_SHELF4_5 = "SHELF4-5";  //for 26Port add 
        public const string STK_SHELF4_6 = "SHELF4-6";  //for 26Port add 
        //public const string STK_SHELF5_1 = "SHELF5-1";
        //public const string STK_SHELF5_2 = "SHELF5-2";
        //public const string STK_SHELF5_3 = "SHELF5-3";
        //public const string STK_SHELF6_1 = "SHELF6-1";
        //public const string STK_SHELF6_2 = "SHELF6-2";
        //public const string STK_SHELF6_3 = "SHELF6-3";

        //WHR Position
        public const string WHR_ILPT1_CLEAN = "ILPT1-Clean";
        public const string WHR_ILPT2_CLEAN = "ILPT2-Clean";
        public const string WHR_CTU_CLEAN = "CTU-Clean";
        public const string WHR_ILPT1_DIRTY = "ILPT1-Dirty";
        public const string WHR_ILPT2_DIRTY = "ILPT2-Dirty";
        public const string WHR_CTU_DIRTY = "CTU-Dirty";

        // Transfer path
        public const string PATH_CLEAN = "Clean";
        public const string PATH_DIRTY = "Dirty";

        //CTU
        public const string CTU_ACCESS_WHR = "ACCESS_WHR";
        public const string CTU_ACCESS_PTZ = "ACCESS_PTZ";
        public const string CTU_ACTION_PREPARE = "Prepare";
        public const string CTU_ACTION_PICK = "Pick";
        public const string CTU_ACTION_PLACE = "Place";

        //PTZ
        public const string PTZ_POSITION_ODD = "Odd";
        public const string PTZ_POSITION_EVEN = "Even";
        public const string PTZ_POSITION_HOME = "Home";
        public const string PTZ_POSITION_PTR = "PTR";
        public const string PTZ_DIRECTION_FACE = "Face";
        public const string PTZ_DIRECTION_BACK = "Back";

        // Auto Button
        /***************************************** STK *****************************************/
        public const string AUTO_RUN_STK_ELPT1 = "btnE1Auto";
        public const string AUTO_RUN_STK_ELPT2 = "btnE2Auto";
        public const string AUTO_RUN_STK_ILPT1 = "btnI1Auto";
        public const string AUTO_RUN_STK_ILPT2 = "btnI2Auto";
        public const string AUTO_RUN_STK_FOUP_ROBOT = "btnFoupRotAuto";

        /***************************************** WHR *****************************************/
        public const string AUTO_RUN_WHR_TO_PORT = "btnWHRPortAuto";
        public const string AUTO_RUN_WHR_TO_CTU = "btnWHRCTUAuto";
        public const string AUTO_RUN_WHR_ALL = "btnWHRAuto";

        /***************************************** PTZ *****************************************/
        /*
         * RORATE + TRANSFER + MOVE HOME
         */
        public const string AUTO_RUN_PTZ = "btnPTZAuto";

        /***************************************** CTU *****************************************/
        /*
         * [CTU] PREPARE PICK + [WHR] TO PICK +  [CTU] RELEASE + [WHR] Complete PICK 
         * [CTU] PREPARE PLACE + [WHR] TO PLACE +  [CTU] GRAB + [WHR] Complete PLACE 
         */
        public const string AUTO_RUN_CTU_TO_WHR = "btnCTUAutoWHR";
        /*
         * CTU 無片 => [CTU] PREPARE PICK + [PTZ] TRANSFER +  [CTU] PICK + [PTZ] MOVE HOME + [CTU] MOVE HOME
         * CTU 有片 => [CTU] PREPARE PLACE + [PTZ] PREPARE(同TRANSFER) +  [CTU] PLACE + [PTZ] MOVE HOME + [CTU] MOVE HOME
         */
        public const string AUTO_RUN_CTU_TO_PTZ = "btnCTUAutoPTZ";
        /*
         *  CTU get from WHR => PUT PTZ
         *  CTU get from PTZ => PUT WHR
         */
        public const string AUTO_RUN_CTU_ALL = "btnCTUAuto";

        public const string SCRIPT_COMMAND_SEND = "cmd_send";
        public const string SCRIPT_COMMAND_ACK = "cmd_ack";
        public const string SCRIPT_COMMAND_FIN = "cmd_finish";
        public const string SCRIPT_IDLE = "script_idle";
        public const string SCRIPT_PAUSE = "script_pause";
        public const string SCRIPT_ERROR = "script_error";
        public const string SCRIPT_RUN = "script_run";
        public const string SCRIPT_RESULT_NORMAL = "complete";
        public const string SCRIPT_RESULT_ABNORMAL = "abnormal";
        public const string SCRIPT_RESULT_TIMEOUT = "timeout";
        public const string SCRIPT_RESULT_STOP = "stop";
    }
}
