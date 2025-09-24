using System.ComponentModel.DataAnnotations;

namespace ProjectManagerMvc.ViewModels
{
    public class CreateUserViewModel
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(11)]
        public string CPF { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Position { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Admin|Manager|Collaborator", ErrorMessage = "Role inválida. Use Admin, Manager ou Collaborator.")]
        public string Role { get; set; } = "Collaborator";
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(11)]
        public string CPF { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Position { get; set; }

        [Required]
        [RegularExpression("Admin|Manager|Collaborator", ErrorMessage = "Role inválida. Use Admin, Manager ou Collaborator.")]
        public string Role { get; set; } = "Collaborator";
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
