using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exercise3.Models
{
    //here the connection to the simulator
    //send message "get" to take information
    public class TcpCommands
    {
        private static TcpCommands s_instace = null;
        //singelton
        public static TcpCommands Instance
        {
            get
            {
                if (s_instace == null)
                {
                    s_instace = new TcpCommands();
                }
                return s_instace;
            }
        }

        private TcpCommands()
        {
            //cretae new tcp commands - one time
        }
        public string ip { get; set; }
        public string port { get; set; }
        public int time { get; set; }


    }
}