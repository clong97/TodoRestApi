using System.ComponentModel.DataAnnotations;

namespace TodoRestApi.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        public string Username { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}