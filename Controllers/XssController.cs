using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebVulnLab.Controllers
{
    public class XssController : Controller
    {
        // Stored XSS sekmelerinde kullanılacak veritabanı simülasyonu listeleri
        private static List<string> _comments = new List<string> { "Siteniz çok güzel olmuş!" };
        private static List<string> _userLinks = new List<string> { "https://github.com" };
        
        // XSS ANA MENÜSÜ
        public IActionResult Index()
        {
            return View();
        }

        // --- 1. REFLECTED XSS SENARYOLARI ---

        public IActionResult ReflectedVulnerable(string searchKeyword)
        {
            ViewBag.Keyword = searchKeyword;
            return View();
        }

        public IActionResult ReflectedSecure(string searchKeyword)
        {
            ViewBag.Keyword = searchKeyword;
            return View();
        }


        // --- 2. STORED XSS SENARYOLARI ---

        [HttpGet]
        public IActionResult StoredVulnerable()
        {
            return View(_comments);
        }

        [HttpPost]
        public IActionResult StoredVulnerable(string newComment)
        {
            if (!string.IsNullOrEmpty(newComment))
                _comments.Add(newComment);
            
            return RedirectToAction("StoredVulnerable");
        }

        [HttpGet]
        public IActionResult StoredSecure()
        {
            return View(_comments);
        }

        [HttpPost]
        public IActionResult StoredSecure(string newComment)
        {
            if (!string.IsNullOrEmpty(newComment))
                _comments.Add(newComment);
            
            return RedirectToAction("StoredSecure");
        }

        // --- 3. DOM-BASED REFLECTED XSS (JS innerHTML hatası) ---
        public IActionResult DomReflected(string searchKeyword)
        {
            ViewBag.Keyword = searchKeyword;
            return View();
        }

        // --- 4. ATTRIBUTE-BASED STORED XSS (URL Doğrulama Unutulması) ---
        [HttpGet]
        public IActionResult StoredAttribute()
        {
            return View(_userLinks);
        }

        [HttpPost]
        public IActionResult StoredAttribute(string newLink)
        {
            if (!string.IsNullOrEmpty(newLink))
                _userLinks.Add(newLink); // Link geçerli mi diye kontrol etmeyi UNUTTUK!
    
            return RedirectToAction("StoredAttribute");
        }
    }
}