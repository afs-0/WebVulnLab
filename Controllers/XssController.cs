using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebVulnLab.Controllers
{
    public class XssController : Controller
    {
        // Tüm XSS sekmelerinde ortak kullanılacak veritabanı simülasyonu
        private static List<string> _comments = new List<string> { "Siteniz çok güzel olmuş!" };

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
    }
}