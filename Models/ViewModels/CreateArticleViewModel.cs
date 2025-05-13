
using System.ComponentModel.DataAnnotations;

namespace finalProject.Models.ViewModels
{
    public class CreateArticleViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Summary { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Chủ đề")]
        public int TopicId { get; set; }

        public List<Topic>? Topics { get; set; }
    }
}