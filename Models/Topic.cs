using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using finalProject.Models.Enums;

namespace finalProject.Models;


    public class Topic
    {
        [Key]
        public int TopicId { get; set; }

        [Required, Display(Name = "Topic Name")]
        [StringLength(100)]
        public string TopicName { get; set; }

        // Navigation property
        public virtual ICollection<Article> Articles { get; set; }
    }
