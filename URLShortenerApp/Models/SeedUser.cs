﻿namespace URLShortenerApp.Models
{
    public class SeedUser
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public List<string>? Roles { get; set; }
    }
}
