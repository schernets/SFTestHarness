using System.Runtime.Serialization;

namespace SFTestHarness
{
    [DataContract]
    public class StringMessage
    {
        [DataMember]
        public string Message { get; set; }
    }
}