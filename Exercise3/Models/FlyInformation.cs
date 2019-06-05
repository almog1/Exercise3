using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Exercise3.Models
{
    //save the currnt fly information
    public class FlyInformation
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public double rudder { get; set; }
        public double throttle { get; set; }

        //full paths in the simulator
        public const string LON_PATH = "/position/longitude-deg";
        public const string LAT_PATH = "/position/latitude-deg";
        public const string RUDDER_PATH = "/controls/flight/rudder";
        public const string THROTTLE_PATH = "/controls/engines/current-engine/throttle";

        //convert data to xml
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("MyFly");
            writer.WriteElementString("LonInfo", this.lon.ToString());
            writer.WriteElementString("LatInfo", this.lat.ToString());
            writer.WriteElementString("RudderInfo", this.rudder.ToString());
            writer.WriteElementString("ThrottleInfo", this.throttle.ToString());

            //write the info to xml
            writer.WriteEndElement(); 
        }

    }
}