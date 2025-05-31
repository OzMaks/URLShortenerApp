using Microsoft.AspNetCore.Mvc;
using URLShortenerApp.Data;
using Microsoft.EntityFrameworkCore;

namespace URLShortenerApp.Controllers
{
    public class RedirectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RedirectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Go(string code)
        {
            var url = await _context.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == code);
            if (url == null)
                return NotFound();

            return Redirect(url.OriginalUrl);
        }
    }
}
