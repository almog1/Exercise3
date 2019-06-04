using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Threading;
using System.Text;
using System.IO;

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
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                //open new client

                Client = new TcpClient();
                Client.Connect(ep); //connecting as client to the server
                                    //change to connected
                                    //Client.Connected          check if conected??
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
                try
                {
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
                    string[] wordsSecond = received.Split('\'');

                    val = Convert.ToDouble(wordsSecond[1]);

                }
                catch (Exception e)
                {
                    //close the connetion
                    Client.Close();
                    IsConnect = false;

                }
            }
            return val;
        }

        //the name of the text with the data from the simulator
        public const string SCENARIO_FILE = "~/App_Data/{0}.txt";           // The Path of the Secnario

        //save the data from the file
        public void WriteData(string fileName)
        {
            string path = HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, fileName));

            //write the data to the file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(flyInformation.lon);
                file.WriteLine(flyInformation.lat);
                file.WriteLine(flyInformation.rudder);
                file.WriteLine(flyInformation.throttle);
            }
        }

        //read the data from the file
        public void ReadData(string fileName, int count)
        {
            //get count which is the line to read from
            string path = HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, fileName));
            //check if the file exist
            if (File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                //need to write two lines
                if ((count + 1) < lines.Length)
                {
                    flyInformation.lon = double.Parse(lines[count]);
                    flyInformation.lat = double.Parse(lines[count + 1]);
                }
            }
        }


    }
}