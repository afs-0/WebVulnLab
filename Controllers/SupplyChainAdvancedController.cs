using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace YourAppName.Controllers
{
    public static class AppEnvironment
    {
        public static string FastPaySdkVersion = "1.0.0"; 
        public static List<string> AttackerC2Logs = new List<string>();
        public static List<string> SystemLogs = new List<string>();
    }

    public static class FastPaySDK
    {
        public static bool ProcessPayment(string cardHolder, string cardNumber, string cvv)
        {
            // Zafiyetli Kütüphane Mantığı
            if (AppEnvironment.FastPaySdkVersion == "1.1.0")
            {
                string stolenData = $"[Sıfırıncı Gün] Kart Çalındı: İsim:{cardHolder}, Kart:{cardNumber}, CVV:{cvv}";
                AppEnvironment.AttackerC2Logs.Add(stolenData);
            }

            AppEnvironment.SystemLogs.Add($"[BAŞARILI] {cardHolder} kişisinden tahsilat yapıldı. (SDK: v{AppEnvironment.FastPaySdkVersion})");
            return true;
        }
    }

    [Route("A03")]
    public class SupplyChainAdvancedController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.CurrentVersion = AppEnvironment.FastPaySdkVersion;
            ViewBag.SystemLogs = AppEnvironment.SystemLogs;
            ViewBag.AttackerLogs = AppEnvironment.AttackerC2Logs;
            return View();
        }

        [HttpPost("Checkout")]
        public IActionResult Checkout(string cardHolder, string cardNumber, string cvv)
        {
            bool success = FastPaySDK.ProcessPayment(cardHolder, cardNumber, cvv);
            if (success)
            {
                TempData["PaymentMsg"] = "Ödemeniz başarıyla alındı! Teşekkür ederiz.";
            }
            
            // İşlem bitince "Müşteri" sekmesinde kal
            TempData["ActiveTab"] = "customer"; 
            return RedirectToAction("Index");
        }

        [HttpPost("UpdateDependency")]
        public IActionResult UpdateDependency()
        {
            AppEnvironment.FastPaySdkVersion = "1.1.0";
            AppEnvironment.SystemLogs.Add("[SİSTEM] FastPay.SDK başarıyla v1.1.0 sürümüne güncellendi.");
            
            // İşlem bitince "IT Ekibi" sekmesinde kal
            TempData["ActiveTab"] = "it-team";
            return RedirectToAction("Index");
        }

        [HttpPost("Reset")]
        public IActionResult Reset()
        {
            AppEnvironment.FastPaySdkVersion = "1.0.0";
            AppEnvironment.SystemLogs.Clear();
            AppEnvironment.AttackerC2Logs.Clear();
            
            TempData["ActiveTab"] = "customer";
            return RedirectToAction("Index");
        }
    }
}