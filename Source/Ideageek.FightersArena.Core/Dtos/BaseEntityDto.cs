namespace Ideageek.FightersArena.Core.Dtos
{
    public class BaseEntityDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
