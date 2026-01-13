using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ideageek.CAFASuite.Core.Dtos
{
    public class GeneralDto
    {
    }
    public class AddGeneralDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Group { get; set; } = string.Empty;
        [JsonIgnore]
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
    }
    public class EditGeneralDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Group { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
