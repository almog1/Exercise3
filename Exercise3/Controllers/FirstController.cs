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
            // TcpCommands.Instance.time = time;

            TcpCommands.Instance.ConnectToServer();
            //    TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
            //  TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);

            //InfoModel.Instance.ReadData("Dor");

            Session["time"] = 4;

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
            /*     info.lon = info.lon - 20;
                 if(info.lon < -180)
                 {
                     info.lon = -30;
                 }
                 info.lat = info.lat + 15;
                 if (info.lat>90)
                 {
                     info.lat = 10;
                 }
                 */

            //  ViewBag.lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
            // ViewBag.lat = 

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
        public string GetMessageFromSimulator()
        {
            //call to a function in the command channel that gets the values from the simulator

            Random rnd = new Random();
            double lon = rnd.Next(1, 50);
            double lat = rnd.Next(1, 50);

            //todo :to change and need to send throttle and rudder???
            return ToXml(lon, lat, 0, 0);
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

            //writer.WriteElementString("Lon", InfoModel.Lon.ToString());
            //writer.WriteElementString("Lat", InfoModel.Lat.ToString());
            writer.WriteElementString("newVar", lon.ToString());
            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lon.ToString());
            //writer.WriteElementString("Throttle", lon.ToString());
            //writer.WriteElementString("Rudder", lon.ToString());

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

      /*[HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);
            return ToXml(InfoModel.Instance.Employee);
        }*/

    }
}