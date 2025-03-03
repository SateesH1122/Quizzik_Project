using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression("^(Admin|Student)$", ErrorMessage = "Role must be either 'Admin' or 'Student'.")]
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
