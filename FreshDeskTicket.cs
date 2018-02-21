using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace YourNameSpace.Models
{
    [DataContract]
    public class FreshDeskTicket
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "description_text")]
        public string DescriptionText { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "priority")]
        public int Priority { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "due_by")]
        public DateTime DueBy { get; set; }

        [DataMember(Name = "responder_id")]
        public long? ResponderId { get; set; }

        [DataMember(Name = "group_id")]
        public long? GroupId { get; set; }

        [DataMember(Name = "company_id")]
        public long? CompanyId { get; set; }

        [DataMember(Name = "requester_id")]
        public long? RequesterId { get; set; }
    }
}