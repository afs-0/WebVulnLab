using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace YourAppName.Controllers
{
    // Aktarım Katmanı Seçenekleri
    public enum TransportMode
    {
        Plaintext,
        Base64,
        Aes
    }

    // Sohbet Mesajı Modeli
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string OriginalText { get; set; }
        public string SniffedText { get; set; }
        public TransportMode Mode { get; set; }
    }

    // Basit Şifreleme Yardımcısı
    public static class CryptoHelper
    {
        // ZAFİYET: Kaynak koda gömülü (hardcoded) şifreleme anahtarı!
        public static readonly string HardcodedKey = "SuperGizliAnahtar123!!4567890123"; 

        public static string EncryptAes(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(HardcodedKey.Substring(0, 32));
            byte[] iv = new byte[16]; // Demo için statik IV (Gerçekte her seferinde rastgele olmalıdır)

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    [Route("A04")]
    public class CryptographicFailuresController : Controller
    {
        // Sohbet geçmişini ve sızdırılan anahtar durumunu tutmak için statik değişkenler
        private static List<ChatMessage> _chatHistory = new List<ChatMessage>();
        private static bool _isKeyLeaked = false;

        // GET: /A04
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /A04/Chat
        [HttpGet("Chat")]
        public IActionResult ChatSimulation()
        {
            ViewBag.History = _chatHistory;
            ViewBag.IsKeyLeaked = _isKeyLeaked;
            ViewBag.HardcodedKey = CryptoHelper.HardcodedKey;
            return View();
        }

        // POST: /A04/Chat
        [HttpPost("Chat")]
        public IActionResult SendMessage(string sender, string messageText, TransportMode transportMode)
        {
            if (string.IsNullOrWhiteSpace(messageText)) return RedirectToAction("ChatSimulation");

            var msg = new ChatMessage
            {
                Sender = sender,
                OriginalText = messageText,
                Mode = transportMode
            };

            // Taşıma katmanına göre paket (sniffer) görünümünü oluştur
            switch (transportMode)
            {
                case TransportMode.Plaintext:
                    msg.SniffedText = messageText;
                    break;
                case TransportMode.Base64:
                    msg.SniffedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageText));
                    break;
                case TransportMode.Aes:
                    msg.SniffedText = CryptoHelper.EncryptAes(messageText);
                    break;
            }

            _chatHistory.Add(msg);
            return RedirectToAction("ChatSimulation");
        }

        // POST: /A04/LeakKey
        [HttpPost("LeakKey")]
        public IActionResult LeakKey()
        {
            _isKeyLeaked = true;
            return RedirectToAction("ChatSimulation");
        }

        // POST: /A04/ClearChat
        [HttpPost("ClearChat")]
        public IActionResult ClearChat()
        {
            _chatHistory.Clear();
            _isKeyLeaked = false;
            return RedirectToAction("ChatSimulation");
        }
    }
}