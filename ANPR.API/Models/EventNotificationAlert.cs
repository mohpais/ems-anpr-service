using System.Xml.Serialization;

namespace Microsoft.Lonsum.Services.ANPR.API.Models
{
    [XmlRoot(ElementName = "EventNotificationAlert", Namespace = "http://www.isapi.org/ver20/XMLSchema")]
    public class EventNotificationAlert
    {
        [XmlElement("ipAddress")]
        public string IpAddress { get; set; }

        [XmlElement("ipv6Address")]
        public string Ipv6Address { get; set; }

        [XmlElement("portNo")]
        public int PortNo { get; set; }

        [XmlElement("protocol")]
        public string Protocol { get; set; }

        [XmlElement("macAddress")]
        public string MacAddress { get; set; }

        [XmlElement("channelID")]
        public int ChannelID { get; set; }

        [XmlElement("dateTime")]
        public DateTime DateTime { get; set; }

        [XmlElement("activePostCount")]
        public int ActivePostCount { get; set; }

        [XmlElement("eventType")]
        public string EventType { get; set; }

        [XmlElement("eventState")]
        public string EventState { get; set; }

        [XmlElement("eventDescription")]
        public string EventDescription { get; set; }

        [XmlElement("channelName")]
        public string ChannelName { get; set; }

        [XmlElement("ANPR")]
        public ANPR ANPR { get; set; }

        [XmlElement("UUID")]
        public string UUID { get; set; }
    }

    public class ANPR
    {
        [XmlElement("licensePlate")]
        public string LicensePlate { get; set; }

        [XmlElement("line")]
        public int Line { get; set; }

        [XmlElement("confidenceLevel")]
        public int ConfidenceLevel { get; set; }

        [XmlElement("plateType")]
        public string PlateType { get; set; }

        [XmlElement("vehicleInfo")]
        public VehicleInfo VehicleInfo { get; set; }

        [XmlElement("plateColor")]
        public string PlateColor { get; set; }

        [XmlElement("licenseBright")]
        public int LicenseBright { get; set; }

        [XmlElement("vehicleType")]
        public string VehicleType { get; set; }

        [XmlElement("originalLicensePlate")]
        public string OriginalLicensePlate { get; set; }
    }

    public class VehicleInfo
    {
        [XmlElement("color")]
        public string Color { get; set; }
    }
}
