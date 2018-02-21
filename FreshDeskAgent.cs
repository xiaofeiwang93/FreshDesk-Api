using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YourNameSpace.Models
{
    [DataContract]
    public class FreshDeskAgents
    {
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        [DataMember(Name = "contact")]
        public FreshDeskContact Contact { get; set; }

        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
