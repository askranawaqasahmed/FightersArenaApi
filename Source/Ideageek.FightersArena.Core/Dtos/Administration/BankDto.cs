using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ideageek.FightersArena.Core.Dtos.Administration
{
    public class BankDto
    {
    }
    public class AddBankDto
    {
        [Required]
        public Guid GeneralBankId { get; set; }
        [Required]
        public string AccountTitle { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public Guid AccountTypeId { get; set; }
        public string IBANNumber { get; set; }
        public string SwiftCode { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public double OpeningBalance { get; set; }
        [AllowNull]
        public DateTime? DateOfOpeningBalance { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
    }
    public class EditBankDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid GeneralBankId { get; set; }
        [Required]
        public string AccountTitle { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public Guid AccountTypeId { get; set; }
        public string IBANNumber { get; set; }
        public string SwiftCode { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public double OpeningBalance { get; set; }
        [AllowNull]
        public DateTime? DateOfOpeningBalance { get; set; }
        public bool BankStatus { get; set; }
    }
}
