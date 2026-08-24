using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models
{
    public class User
    {
        // Ensure that Id is always a positive number (if 0 is not a valid Id in your system)
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid user ID.")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "First name is required. ")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 chaacters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The IsActive field is required.")]        
        public bool IsActive { get; set; } = true;
    }
}