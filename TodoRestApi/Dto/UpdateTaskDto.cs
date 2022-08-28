using System;
using System.ComponentModel.DataAnnotations;

namespace TodoRestApi.Dto
{
    public class UpdateTaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Only Low, Medium, High are allowed.")]
        public string Priority { get; set; }
        [RegularExpression("^(New|In-Progress|On-Hold|Completed)$", ErrorMessage = "Only New, In-Progress, On-Hold, Completed are allowed.")]
        public string Status { get; set; }
        [Range(0,100, ErrorMessage = "Only 0 to 100 allowed.")]
        public Nullable<int> Progress { get; set; }
        public Nullable<int> AssigneeId { get; set; }
        [RegularExpression("^(0?[1-9]|[12][0-9]|3[01])/(0?[1-9]|1[0-2])/\\d{4} (00|[0-9]|1[0-9]|2[0-3]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])$", ErrorMessage = "Wrong format. DD/MM/YYYY HH:MI:SS")]
        public string StartDate { get; set; }
        [RegularExpression("^(0?[1-9]|[12][0-9]|3[01])/(0?[1-9]|1[0-2])/\\d{4} (00|[0-9]|1[0-9]|2[0-3]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])$", ErrorMessage = "Wrong format. DD/MM/YYYY HH:MI:SS")]
        public string DueDate { get; set; }
    }
}