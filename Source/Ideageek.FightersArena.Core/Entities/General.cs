using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Entities.Setting
{
    public class General : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Group { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
