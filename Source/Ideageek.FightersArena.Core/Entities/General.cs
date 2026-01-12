using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Entities.Setting
{
    public class General : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Group { get; set; }
        public bool Status { get; set; }
    }
}
