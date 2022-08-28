using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoRestApi.Utils
{
    public class Constant
    {
        public const string ENCRYPTION_KEY = "MAKVKKKBNI99212";

        public static readonly string[] STATUS = { "Active", "Deactivated" };
        public const string STATUS_ACTIVE = "Active";
        public const string STATUS_DEACTIVATED = "Deactivated";
        public const string STATUS_INVITED = "Invited";
        public const string STATUS_REJECTED = "Rejected";
        public const string STATUS_NEW = "New";
        public const string STATUS_IN_PROGRESS = "In-Progress";
        public const string STATUS_ON_HOLD = "On-Hold";
        public const string STATUS_CANCELLED = "Cancelled";
        public const string STATUS_COMPLETED = "Completed";

        public const string ROLE_OWNER = "Owner";
        public const string ROLE_MEMBER = "Member";

        public const string PRIORITY_LOW = "Low";
        public const string PRIORITY_MEDIUM = "Medium";
        public const string PRIORITY_HIGH = "High";
    }
}