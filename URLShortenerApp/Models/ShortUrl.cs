using System.ComponentModel.DataAnnotations;

namespace URLShortenerApp.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        [Required]
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; }
        public User? CreatedBy { get; set; }
    }
}
