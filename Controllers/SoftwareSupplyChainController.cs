using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YourAppName.Controllers
{
    // SALDIRGANIN SUNUCUSU (C2 Server Simülasyonu)
    public static class AttackerC2Server
    {
        public static List<string> StolenDataLog = new List<string>();
    }

    // 3. TARAF KÜTÜPHANE SİMÜLASYONU (Görünüşte "SuperCsvParser" adında bir NuGet paketi)
    public static class MaliciousCsvParser
    {
        public static List<string[]> ParseCsv(string csvData)
        {
            // 1. GİZLİ KÖTÜ NİYETLİ DAVRANIŞ (Truva Atı / Backdoor)
            // Kütüphane işini yapmadan önce veya yaparken veriyi saldırganın sunucusuna sızdırıyor.
            if (!string.IsNullOrWhiteSpace(csvData))
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                AttackerC2Server.StolenDataLog.Add($"[{timestamp}] - VERİ SIZDIRILDI:\n{csvData}");
            }

            // 2. BEKLENEN NORMAL DAVRANIŞ
            // Kütüphane işini mükemmel yapıyor, sistemi bozmuyor. Bu yüzden kimse şüphelenmiyor!
            var result = new List<string[]>();
            if (string.IsNullOrWhiteSpace(csvData)) return result;

            var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                result.Add(line.Split(','));
            }
            return result;
        }
    }

    // Controller seviyesinde ana rotayı /A03 olarak belirliyoruz
    [Route("A03")]
    public class SoftwareSupplyChainController : Controller
    {
        // GET: /A03
        // Zafiyeti açıklayan ana sekme içeriği
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /A03/Simulation
        // İkiye bölünmüş ekranın olduğu simülasyon sayfası
        [HttpGet("Simulation")]
        public IActionResult Simulation()
        {
            // Sağ ekranda (Saldırgan terminali) gösterilmek üzere çalınan verileri sayfaya gönderiyoruz
            ViewBag.StolenLogs = AttackerC2Server.StolenDataLog.ToList();
            return View();
        }

        // POST: /A03/Simulation
        // Kullanıcının veriyi yüklediği form isteği
        [HttpPost("Simulation")]
        public IActionResult Simulation(string csvPayload)
        {
            if (string.IsNullOrWhiteSpace(csvPayload))
            {
                TempData["Error"] = "Lütfen işlenecek bir CSV verisi girin.";
                return RedirectToAction("Simulation");
            }

            // Geliştirici burada popüler ve güvenilir sandığı "MaliciousCsvParser" kütüphanesini çağırıyor.
            var parsedData = MaliciousCsvParser.ParseCsv(csvPayload);

            // Veriler başarıyla ayrıştırıldığı için kullanıcıya olumlu mesaj gösteriyoruz.
            // Bu "sahte bir güvenlik hissi" yaratır.
            TempData["Success"] = $"Başarılı! Kütüphane sayesinde {parsedData.Count} kayıt sisteme hatasız şekilde aktarıldı.";
            
            return RedirectToAction("Simulation");
        }
    }
}