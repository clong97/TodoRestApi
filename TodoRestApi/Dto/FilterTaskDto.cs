using System;
using System.ComponentModel.DataAnnotations;

namespace TodoRestApi.Dto
{
    public class FilterTaskDto
    {
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string AssigneeId { get; set; }
        public string TeamId { get; set; }
        public string CreatedDate { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }
    }
}