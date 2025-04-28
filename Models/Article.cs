using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using finalProject.Models.Enums;
namespace finalProject.Models;

public class Article
{
    [Key]
    public required  int ArticleId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required  string Title { get; set; }
    
    
    
    
    [Required]
    public required  DateTime SubmissionDate { get; set; }
    
    [Required]
    public required  string Summary { get; set; }
    
    [Required]
    public required  string Content { get; set; }
    
    [Required]
    public required  string Topic { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(50)")]
    public required  ArticleStatus Status { get; set; }// "Pending", "Approved", "Rejected"
    
    public int AuthorId { get; set; } //nếu viết int? nó sẽ cho phép trường này NULL
    [ForeignKey("AuthorId")]
    
    public required User Author { get; set; }
}