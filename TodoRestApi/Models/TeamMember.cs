//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TodoRestApi.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public partial class TeamMember
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> JoinedDate { get; set; }
        public Nullable<System.DateTime> LeaveDate { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Team Team { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual User User { get; set; }
    }
}
