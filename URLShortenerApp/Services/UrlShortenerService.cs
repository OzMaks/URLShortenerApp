using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortenerApp.Data;
using URLShortenerApp.Models;

namespace URLShortenerApp.Services
{
    public class UrlShortenerService
    {
        public string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
