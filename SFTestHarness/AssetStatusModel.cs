using System;
using System.Runtime.Serialization;

namespace SFTestHarness
{
    [DataContract]
    public class AssetStatusModel
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string State { get; set; }
    }
}