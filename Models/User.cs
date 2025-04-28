using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using finalProject.Models.Enums;

namespace finalProject.Models;
public class User
{
    [Key]
    public required  int UserId { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required  DateTime DateofBirth { get; set; }

    [Required]
    [EmailAddress]
    public required  string Email { get; set; }

    [Required]
    public required  string Username { get; set; }

    [Required]
    public required  string Password { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public Role Role { get; set; } // 👉 enum luôn nè
}