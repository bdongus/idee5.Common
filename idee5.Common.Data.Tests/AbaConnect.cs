using System.Collections.Generic;
using System.Xml.Serialization;

// Generated via http://xmltocsharp.azurewebsites.net/
namespace idee5.Common.Data.Tests.Abacus {
    [XmlRoot(ElementName = "Parameter")]
    public class Parameter
    {
        [XmlElement(ElementName = "Application")]
        public string Application { get; set; }

        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "MapId")]
        public string MapId { get; set; }

        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Mandant")]
        public string Mandant { get; set; }
    }

    [XmlRoot(ElementName = "Task")]
    public class Task
    {
        [XmlElement(ElementName = "Parameter")]
        public Parameter Parameter { get; set; }
    }

    [XmlRoot(ElementName = "Transaction")]
    public class Transaction
    {
        [XmlElement(ElementName = "Number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "NativeName")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "AbaConnectContainer")]
    public class AbaConnectContainer
    {
        [XmlElement(ElementName = "TaskCount")]
        public string TaskCount { get; set; }

        [XmlElement(ElementName = "Task")]
        public Task Task { get; set; }

        [XmlElement(ElementName = "Transaction")]
        public List<Transaction> Transaction { get; set; }
    }
}