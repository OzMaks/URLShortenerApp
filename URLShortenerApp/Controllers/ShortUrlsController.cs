using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using URLShortenerApp.Data;
using URLShortenerApp.Models;
using URLShortenerApp.Services;
using URLShortenerApp.Settings;

namespace URLShortenerApp.Controllers
{
    [Authorize]
    public class ShortUrlsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly UrlShortenerService _urlShortener;

        public ShortUrlsController(ApplicationDbContext context, UserManager<User> userManager, UrlShortenerService shortener)
        {
            _context = context;
            _userManager = userManager;
            _urlShortener = shortener;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = PaginationSettings.DefaultPage, 
            int pageSize = PaginationSettings.DefaultPageSize, bool isPartial = false)
        {
            var query = _context.ShortUrls.Include(x => x.CreatedBy).OrderByDescending(x => x.Id);

            var totalItems = await query.CountAsync();
            var urls = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (isPartial)
                return PartialView("_ShortUrlTable", urls);

            return View(urls);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUrl(string originalUrl)
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _context.ShortUrls.AnyAsync(u => u.OriginalUrl == originalUrl))
                return BadRequest("This URL already exists.");

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = _urlShortener.GenerateShortCode(),
                CreatedById = user.Id
            };

            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();

            return PartialView("_ShortUrlRow", shortUrl);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var url = await _context.ShortUrls.FindAsync(id);

            if (url == null)
                return NotFound();

            if (User.IsInRole(Roles.Admin) || url.CreatedById == user.Id)
            {
                _context.ShortUrls.Remove(url);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return Forbid();
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var url = await _context.ShortUrls
                .Include(u => u.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (url == null)
                return NotFound();

            return View(url);
        }
    }
}
