using System.ComponentModel.DataAnnotations;

namespace JobManager.Service
{
    public class SJobItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        public int SJobId { get; set; }
    }
}