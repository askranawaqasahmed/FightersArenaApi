using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
