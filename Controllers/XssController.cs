using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebVulnLab.Controllers
{
    public class XssController : Controller
    {
        // Stored XSS senaryosu için geçici (In-Memory) veritabanı simülasyonu
        private static List<string> _comments = new List<string> { "Harika bir site olmuş!" };

        // 1. XSS Senaryoları Seçim Ekranı
        public IActionResult Index()
        {
            return View();
        }

        // 2. Reflected XSS Ekranı
        [HttpGet]
        public IActionResult Reflected(string searchKeyword)
        {
            // Kullanıcıdan alınan veriyi doğrudan View'a gönderiyoruz
            ViewBag.Keyword = searchKeyword;
            return View();
        }

        // 3. Stored XSS Ekranı (Hem Gösterme Hem Ekleme)
        [HttpGet]
        public IActionResult Stored()
        {
            return View(_comments);
        }

        [HttpPost]
        public IActionResult Stored(string newComment)
        {
            if (!string.IsNullOrEmpty(newComment))
            {
                _comments.Add(newComment); // Kötü niyetli kodu listeye kaydediyoruz
            }
            return RedirectToAction("Stored");
        }
    }
}