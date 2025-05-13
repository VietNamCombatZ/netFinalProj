using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using finalProject.Models.Enums;
namespace finalProject.Models;

public class Article
{
    public Article()
    {
        SubmissionDate = DateTime.Now;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public   int ArticleId { get; set; }
    
    public DateTime? ReviewedAt { get; set; } // Nullable vì chưa duyệt thì sẽ là null
    
    [Required]
    [StringLength(255)]
    public required  string Title { get; set; }
    
    
    
    
    [Required]
    public required  DateTime SubmissionDate { get; set; }
    
    [Required]
    public required  string Summary { get; set; }
    
    [Required]
    public required  string Content { get; set; }
    
    // [Required]
    // public required  string Topic { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public required ArticleStatus Status { get; set; } = ArticleStatus.Pending;// "Pending", "Approved", "Rejected"
    
    [ForeignKey("AuthorId")]
    public int AuthorId { get; set; } //nếu viết int? nó sẽ cho phép trường này NULL
    
    
    public virtual User Author { get; set; }
    
    [ForeignKey("Topic")]
    public int TopicId { get; set; }
    public virtual Topic Topic { get; set; }
}