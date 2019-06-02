using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Exercise3.Models
{
    public class FlyInformation
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public double speed { get; set; }
        public double high { get; set; }

        public const string LON_PATH = "/position/longitude-deg";
        public const string LAT_PATH = "/position/latitude-deg";

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("MyFly");
            writer.WriteElementString("LonInfo", this.lon.ToString());
            writer.WriteElementString("LatInfo", this.lat.ToString());
            //write the info to xml
            writer.WriteEndElement(); 
        }

    }
}