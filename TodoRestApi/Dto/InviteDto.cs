using System.ComponentModel.DataAnnotations;

namespace TodoRestApi.Dto
{
    public class InviteDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public int TeamId { get; set; }
    }
}