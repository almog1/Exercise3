using System;
using Exercise3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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

        [HttpGet]
        public ActionResult display(string ip, int port, int time)
        {
            InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;

            InfoModel.Instance.ReadData("Dor");

            Session["time"] = time;
        
            return View();
        }

        [HttpGet]
        public ActionResult displayPicture(string ip, string port)
        {
           
            return View();
        }

        [HttpPost]
        public string GetEmployee()
        {
            var emp = InfoModel.Instance.Employee;

            emp.Salary = rnd.Next(1000);

            return ToXml(emp);
        }

        private string ToXml(Employee employee)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Employess");

            employee.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }


        // POST: First/Search
        [HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);

            return ToXml(InfoModel.Instance.Employee);
        }

    }
}