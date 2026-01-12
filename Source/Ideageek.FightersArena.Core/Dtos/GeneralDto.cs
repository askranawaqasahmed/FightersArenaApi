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
        public string Name { get; set; }
        [Required]
        public string Group { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
    }
    public class EditGeneralDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Group { get; set; }
        public bool Status { get; set; }
    }
}