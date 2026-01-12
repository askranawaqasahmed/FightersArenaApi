using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Entities.Administration
{
    public class Bank : BaseEntity
    {
        [Required]
        public Guid GeneralBankId { get; set; }
        public string GeneralBank { get; set; }
        [Required]
        [FilterableAttribute]
        public string AccountTitle { get; set; }
        [Required]
        [FilterableAttribute]
        public string AccountNumber { get; set; }
        [Required]
        public Guid ChartOfAccountId { get; set; }
        [Required]
        public Guid AccountTypeId { get; set; }
        public string AccountType { get; set; }
        public string IBANNumber { get; set; }
        public string SwiftCode { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public double OpeningBalance { get; set; }
        public DateTime? DateOfOpeningBalance { get; set; }
        [FilterableAttribute]
        public bool BankStatus { get; set; }
    }
}
