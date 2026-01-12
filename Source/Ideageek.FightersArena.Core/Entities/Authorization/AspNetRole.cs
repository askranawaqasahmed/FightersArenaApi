namespace Ideageek.FightersArena.Core.Entities.Authorization
{
    public class AspNetRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }

}
