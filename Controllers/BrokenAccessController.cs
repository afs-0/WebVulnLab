using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace YourAppName.Controllers
{
    // Model Sınıfı (Normalde Models klasöründe olur)
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    // Controller seviyesinde ana rotayı /A01 olarak belirliyoruz
    [Route("A01")]
    public class BrokenAccessControlController : Controller
    {
        // Veritabanını simüle etmek için statik bir liste kullanıyoruz
        private static List<BlogPost> _posts = new List<BlogPost>
        {
            new BlogPost { Id = 1, Title = "Şirket İçi Kurallar", Content = "Ofise giriş saatleri 09:00 olarak güncellenmiştir." },
            new BlogPost { Id = 2, Title = "Q3 Finansal Raporu", Content = "Gizli şirket finansal verileri..." },
            new BlogPost { Id = 5, Title = "Kritik Sistem Şifreleri", Content = "Sistem yöneticileri için acil durum şifreleri." }
        };

        // GET: /A01
        // Zafiyeti açıklayan ana sekme içeriği
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /A01/Posts
        // Standart kullanıcının gördüğü gönderi listesi
        [HttpGet("Posts")]
        public IActionResult Posts()
        {
            // Simülasyon: Sisteme "Standart Kullanıcı" olarak giriş yapıldığını varsayıyoruz.
            ViewBag.CurrentUserRole = "StandardUser"; 
            
            return View(_posts);
        }

        // POST: /A01/Posts/Delete/5
        // Zafiyetli Silme İşlemi
        [HttpPost("Posts/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            // ZAFİYET BURADA!
            // Sunucu, bu isteği yapan kişinin "Admin" olup olmadığını kontrol ETMIYOR.
            // Sadece ön uçta (UI) butonu gizlemenin güvenli olduğunu varsayıyor.
            
            /* GÜVENLİ KOD ŞÖYLE OLMALIYDI:
               if (currentUser.Role != "Admin") { return Unauthorized(); }
            */

            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                _posts.Remove(post);
                TempData["Message"] = $"ZAFİYET SÖMÜRÜLDÜ! Yönetici (Admin) yetkiniz olmamasına rağmen sunucu isteği kabul etti ve {id} ID'li gönderi silindi.";
            }

            return RedirectToAction("Posts");
        }
    }
}