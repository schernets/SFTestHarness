using System;
using System.Runtime.Serialization;

namespace SFTestHarness
{
    [DataContract]
    public class IngestionStartModel
    {
        [DataMember]
        public Guid AssetId { get; set; }
        [DataMember]
        public Guid ProcessId { get; set; }
        [DataMember]
        public string UploadUrl { get; set; }
    }
}