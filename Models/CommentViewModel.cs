using System.ComponentModel.DataAnnotations;

namespace LetterBoxd.Models
{
    public class CommentViewModel
    {
        [Required]
        [StringLength(50)]
        public string Content { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
} 