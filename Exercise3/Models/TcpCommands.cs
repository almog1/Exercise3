using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Threading;
using System.Text;

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

        public string ip { get; set; }
        public int port { get; set; }
        public int time { get; set; }
        public FlyInformation flyInformation { get; private set; }

        private TcpCommands()
        {
            //cretae new tcp commands - one time
            IsConnect = false;
            flyInformation = new FlyInformation();
        }

        private bool _isConnect;
        private TcpClient _client;

        public TcpClient Client
        {
            get { return _client; }
            set
            {
                _client = value;
            }
        }

        //to know if the client succed to connect to the simulator
        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
            set
            {
                _isConnect = value;
            }
        }

        //function to connect the server
        public void ConnectToServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip),port);
            try
            {
                //open new client

                Client = new TcpClient();
                Client.Connect(ep); //connecting as client to the server
                                    //change to connected
                
                IsConnect = true; //succed to connect
            }
            catch (SocketException)
            {
                Console.WriteLine("FAILED CONNECT COMMAND");
            }
            //  Console.WriteLine("connect in command channel");
        }

        //get values from the server
        public double GetValues(string path)
        {
            double val = 0;
            //check the connection
            if (IsConnect == true)
            {
                //open thread for get lat & lon
                //new Thread(() =>
               // {
                    byte[] sendBuffer = new byte[1024];
                    byte[] getBuffer = new byte[1024];
                    int readed = 0;

                    NetworkStream stream = Client.GetStream();
                    //path to be with \r\n
                    string line, newLine = " \r\n";
                    path = "get " + path;
                    line = path + newLine;
                    sendBuffer = (Encoding.ASCII).GetBytes(line);
                    Console.WriteLine("the message send to simulator:" + sendBuffer);
                    stream.Write(sendBuffer, 0, sendBuffer.Length);
                    Console.WriteLine("TCP client: Sending the actual data...");

                    readed = stream.Read(getBuffer, 0, getBuffer.Length);
                    string received = Encoding.ASCII.GetString(getBuffer, 0, readed);
                    Console.WriteLine("Response from client: {0}", received);
               //     string[] words = received.Split(' ');
                    string[] wordsSecond = received.Split('\'');

                val = Convert.ToDouble(wordsSecond[1]);
                    //do
                    // {
                    //     int read = stream.Read(getBuffer, readed, getBuffer.Length - readed);
                    //   readed += read;
                    // } while (Client.GetStream().DataAvailable);
                  //  Thread.Sleep(2000);
                    
                //}).Start();
            }
            return val;
        }
    }
}