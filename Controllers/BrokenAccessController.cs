using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace YourAppName.Controllers
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }

    [Route("A01")]
    public class BrokenAccessControlController : Controller
    {
        // Veritabanı simülasyonu
        private static List<BlogPost> _posts = new List<BlogPost>
        {
            new BlogPost { Id = 1, Title = "Şirket Kuralları Güncellendi", Author = "admin" },
            new BlogPost { Id = 2, Title = "Haftalık Toplantı Notları", Author = "user" }
        };
        private static int _nextId = 3;

        // GET: /A01 (Giriş Sayfası)
        [HttpGet("")]
        public IActionResult Login()
        {
            // Mevcut çerezi temizle
            Response.Cookies.Delete("DemoRole");
            Response.Cookies.Delete("DemoUser");
            return View();
        }

        // POST: /A01/Login
        [HttpPost("Login")]
        public IActionResult DoLogin(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("Login");

            // Basit Oturum Yönetimi: "admin" girilirse Admin rolü, diğerleri "User" rolü alır.
            string role = username.ToLower() == "admin" ? "Admin" : "User";
            
            Response.Cookies.Append("DemoUser", username);
            Response.Cookies.Append("DemoRole", role);

            return RedirectToAction("Dashboard");
        }

        // GET: /A01/Dashboard
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            Request.Cookies.TryGetValue("DemoRole", out string role);
            Request.Cookies.TryGetValue("DemoUser", out string user);

            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login");

            ViewBag.Role = role;
            ViewBag.Username = user;
            return View(_posts);
        }

        // POST: /A01/Create
        [HttpPost("Create")]
        public IActionResult Create(string title)
        {
            Request.Cookies.TryGetValue("DemoUser", out string user);
            
            _posts.Add(new BlogPost { Id = _nextId++, Title = title, Author = user ?? "Anonim" });
            
            // Burp panelinde düzgün görünmesi için JSON/Metin dönüyoruz
            return Ok($"Başarılı: '{title}' eklendi.");
        }

        // POST: /A01/Delete/5
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            // ZAFİYET BURADA: İsteği atan kişinin "Admin" olup olmadığını kontrol ETMİYORUZ!
            // Sadece sisteme giriş yapmış olması yeterli oluyor.
            
            /* GÜVENLİ KOD ŞÖYLE OLMALIYDI:
               Request.Cookies.TryGetValue("DemoRole", out string role);
               if (role != "Admin") return Unauthorized("Bu işlem için yetkiniz yok!");
            */

            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                _posts.Remove(post);
                return Ok($"Başarılı: {id} ID'li post silindi!");
            }
            
            return NotFound("Post bulunamadı.");
        }
    }
}