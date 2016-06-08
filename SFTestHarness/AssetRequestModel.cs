using System.Runtime.Serialization;

namespace SFTestHarness
{
    [DataContract]
    public class AssetRequestModel
    {
        [DataMember]
        public string Filename { get; set; }
    }
}