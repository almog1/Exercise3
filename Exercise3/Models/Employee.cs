using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Exercise3.Models
{
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Employee");
            writer.WriteElementString("FirstName", this.FirstName);
            writer.WriteElementString("LastName", this.LastName);
            writer.WriteElementString("Salary", this.Salary.ToString());
            writer.WriteEndElement();
        }
    }
}