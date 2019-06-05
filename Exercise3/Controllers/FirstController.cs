using System;
using Exercise3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Net;

namespace Exercise3.Controllers
{
    public class FirstController : Controller
    {
        /**
         * The index is the default page
        **/
        public ActionResult Home()
        {
            return View();
        }


        //first mission - gets a point and draw it on the map
        /**
         * This action diaplay the current point of the plain .
         * get ip and port of the simulator
         **/
        [HttpGet]
        public ActionResult DisplayPicture(string ip, int port)
        {
            //check if its the right action
            IPAddress clientIpAddr;
            bool success = IPAddress.TryParse(ip, out clientIpAddr);
            //if its not ip address - send to other action
            if (!success)
            {
                return RedirectToAction("DisplayLoadData", new { fileName = ip, time = port });
            }
            //close if was old client
            TcpCommands.Instance.CloseClient();

            //need to save ip and port
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            //need to check if we connected already 
            TcpCommands.Instance.ConnectToServer();

            if (TcpCommands.Instance.IsConnect)
            {
                //saves alements in viewBag
                ViewBag.Lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
                ViewBag.Lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);
            }

            return View();
        }
        
        /**
         * Display the path that the plain id flying at
         * By taking points of lon&lan from the simultor .
         * Draw the point once in $time seconds
         * 
         **/
        [HttpGet]
        public ActionResult DisplayWay(string ip, int port, int time)
        {
            //close if was old client
            TcpCommands.Instance.CloseClient();
            //save the ip,port, time
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            TcpCommands.Instance.ConnectToServer();
            //save it for the view
            Session["time"] = time;

            return View();
        }

        /**
         * Load the path of the plain from the file .
         * Display it as annimation .
         * **/
        [HttpGet]
        public ActionResult DisplayLoadData(string fileName, int time)
        {
            Session["time"] = time;
            ViewBag.file = fileName;
            return View();
        }

        /**
         * Save the points where the plain is .
         * Save in fileName at app_data and draw it
         **/
        [HttpGet]
        public ActionResult SaveData(string ip, int port, int time,
                                      int seconds, string fileName)
        {
            //close if was old client
            TcpCommands.Instance.CloseClient();
            //need to save ip and port
            TcpCommands.Instance.ip = ip;
            TcpCommands.Instance.port = port;
            TcpCommands.Instance.ConnectToServer();
            Session["time"] = time;
            Session["seconds"] = seconds;
            ViewBag.FileName = fileName;
            
            return View();
        }

        /**
         * Get The info of the fly from the simulator .
         * return string by convert the details to xml
         **/
        [HttpPost]
        public string GetFlyInfo()
        {
            var info = TcpCommands.Instance.flyInformation;
            double lon, lat;
            
            if(TcpCommands.Instance.IsConnect)
            {
                lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
                lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);
                //check if connect or failed
                if (TcpCommands.Instance.IsConnect)
                {
                    info.lon = lon;
                    info.lat = lat;
                }
                //if not connect - keep the old values
            }
            return FlyInfoToXml(info);
        }

        /**
         * Convert the FLyinformation to xml
         * so the view can read its values
         **/
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
        /**
         * Get the full information and write it to the file
         **/
        [HttpPost]
        public string GetFlyFullInfo(string fileName)
        {
            var info = TcpCommands.Instance.flyInformation;
            double lon, lat,rudder,throttle;
            //check if connect to simulator
            if (TcpCommands.Instance.IsConnect)
            {
                lon = TcpCommands.Instance.GetValues(FlyInformation.LON_PATH);
                lat = TcpCommands.Instance.GetValues(FlyInformation.LAT_PATH);
                rudder = TcpCommands.Instance.GetValues(FlyInformation.RUDDER_PATH);
                throttle = TcpCommands.Instance.GetValues(FlyInformation.THROTTLE_PATH);
                //check if connect or failed
                if (TcpCommands.Instance.IsConnect)
                {
                    info.lon = lon;
                    info.lat = lat;
                    info.rudder = rudder;
                    info.throttle = throttle;
                    TcpCommands.Instance.WriteData(fileName);
                }
                //if not connect - keep the old values
            }
            
            return FlyInfoToXml(info);
        }

        /**
         * Read the information from the file .
         * count for know in which line need to send info from
         **/
        [HttpPost]
        public string ReadFlyInfo(string fileName, int count)
        {
            //read the data from the file
            //update the Tcp.Flyinformation values
            TcpCommands.Instance.ReadData(fileName, count);

            //take the information from the flyInformation
            var info = TcpCommands.Instance.flyInformation;
            //convert to xml
            return FlyInfoToXml(info);
        }
       

    }
}