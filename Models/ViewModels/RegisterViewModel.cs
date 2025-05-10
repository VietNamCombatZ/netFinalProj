// Models/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace finalProject.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime DateofBirth { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Username { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; }
}