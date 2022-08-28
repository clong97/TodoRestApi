using System.ComponentModel.DataAnnotations;

namespace TodoRestApi.Dto
{
    public class TeamDto
    {
        [Required]
        public string Name { get; set; }
    }
}