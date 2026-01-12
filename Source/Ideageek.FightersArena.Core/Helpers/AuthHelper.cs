using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ideageek.FightersArena.Core.Helpers
{
    public interface IAuthHelper
    {
        Guid? GetUserId();
    }
    public class AuthHelper : IAuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId =  user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                return Guid.Parse(userId);
            return null;
        }
    }
}
