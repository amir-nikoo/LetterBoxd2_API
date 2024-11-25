using System.ComponentModel.DataAnnotations;

namespace LetterBoxd.Models
{
    public class RatingViewModel
    {
        [Required]
        public int Score { get;set; }

        [Required]
        public int MovieId { get;set; }
    }
} 