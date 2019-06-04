using System;
using Exercise3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Net;

// path lon /position/longitude-deg
// path lan /position/latitude-deg

namespace Exercise3.Controllers
{
    public class FirstController : Controller
    {
        private static Random rnd = new Random();

        // GET: First
        public ActionResult Index()
        {
            return View();
        }


        //first mission - gets a point and draw it on the map
        [HttpGet]
        public ActionResult displayPicture(string ip, int port)
        {
            //check if its the right action
            IPAddress clientIpAddr;
            bool success = IPAddress.TryParse(ip, out clientIpAddr);
            //if its not ip address
            if (!success)
            {
                return RedirectToAction("DisplayLoadData", new { fileName = ip,time = port });
            }
            //need to save ip and port
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;

            //need to check if we connected already 
            TcpCommands.Instance.ConnectToServer();

            //saves alements in viewBag
            ViewBag.Lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
            ViewBag.Lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);

            return View();
        }

        //second mission - draw the way of the plane on the map
        [HttpGet]
        public ActionResult connectServer(string ip, int port)
        {
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            TcpCommands.Instance.ConnectToServer();

            return View();
        }

        [HttpGet]
        public ActionResult displayWay(string ip, int port, int time)
        {
            //infoModel - connection with the simulator (the communication with the simulator)
            // InfoModel.Instance.ip = ip;
            // InfoModel.Instance.port = port.ToString();
            // InfoModel.Instance.time = time;
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            TcpCommands.Instance.ConnectToServer();

            //ViewBag.Lon = -157;
            //ViewBag.Lat = 21;
            Session["time"] = time;

            return View();
        }
       
        //second mission - draw the way of the plane on the map
        [HttpGet]
        public ActionResult DisplayLoadData(string fileName,int time)
        {
            Session["time"] = time;
            ViewBag.file = fileName;

            return View();
        }

        //second mission - draw the way of the plane on the map
        [HttpGet]
        public ActionResult SaveData(string ip,int port, int time,
                                      int seconds,string fileName)
        {
            //need to save ip and port
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            TcpCommands.Instance.ConnectToServer();
            Session["time"] = time;
            Session["seconds"] = seconds;
            ViewBag.FileName = fileName;

            //need to check if connected 
            if (TcpCommands.Instance.IsConnect)
            {
                
            }
            return View();
        }

        //gets a data from the simulator with gets commands 
        [HttpPost]
        public string GetFlyInfo()
        {
            var info = TcpCommands.Instance.flyInformation;

            if (TcpCommands.Instance.IsConnect == false)
            {
                // TcpCommands.Instance.ConnectToServer();
            }
            else
            {

                info.lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
                info.lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);
            }
            return FlyInfoToXml(info);
        }

        private string FlyInfoToXml(FlyInformation info)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FlyInfo");

            info.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
        [HttpPost]
        public string GetFlyFullInfo(string fileName)
        {
            var info = TcpCommands.Instance.flyInformation;

            if (TcpCommands.Instance.IsConnect == false)
            {
                // TcpCommands.Instance.ConnectToServer();
            }
            else
            {
                info.lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
                info.lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);
                info.rudder = TcpCommands.Instance.GetValues(FlyInformation.RUDDER_PATH);
                info.throttle = TcpCommands.Instance.GetValues(FlyInformation.THROTTLE_PATH);
                //write the data to the file
                TcpCommands.Instance.WriteData(fileName);
            }
            return FlyInfoToXml(info);
        }

        [HttpPost]
        public string ReadFlyInfo(string fileName,int count)
        {
            //read the data from the file
            TcpCommands.Instance.ReadData(fileName, count);
            //take the information from the flyInformation
            var info = TcpCommands.Instance.flyInformation;
            //convert to xml
            return FlyInfoToXml(info);
        }

        [HttpPost]
        public string GetMessageFromSimulator()
        {
            //call to a function in the command channel that gets the values from the simulator

          //  double lon = rnd.Next(-180, 180);
          //  double lat = rnd.Next(-90, 90);

            Random random = new Random();
            double lon = random.NextDouble() * (180 - (-180)) + (-180);
            double lat = random.NextDouble() * (90 - (-90)) + (-90);

            Random random2 = new Random();
            //between 0 - 1
            double throttle = random.NextDouble();
            double rudder = random.NextDouble();
            return ToXml(lon, lat, throttle, rudder);
        }

        //write the values of lon and lat to a format of xml
        private string ToXml(double lon, double lat, double throttle, double rudder)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Location");

            writer.WriteElementString("LonInfo", lon.ToString());
            writer.WriteElementString("LatInfo", lat.ToString());
            writer.WriteElementString("ThrottleInfo", throttle.ToString());
            writer.WriteElementString("RudderInfo", rudder.ToString());

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        //close the client
        public void CloseClient()
        {
            TcpCommands.Instance.Client.Close();
        }
      /*[HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);
            return ToXml(InfoModel.Instance.Employee);
        }*/

    }
}