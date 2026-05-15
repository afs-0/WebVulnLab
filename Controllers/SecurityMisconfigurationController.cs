using Microsoft.AspNetCore.Mvc;

namespace YourAppName.Controllers
{
    // Controller seviyesinde ana rotayı /A02 olarak belirliyoruz
    [Route("A02")]
    public class SecurityMisconfigurationController : Controller
    {
        // GET: /A02
        // Zafiyeti açıklayan ana sekme içeriği
        [Route("")] 
        public IActionResult Index()
        {
            return View();
        }

        // GET: /A02/AdminPortalLogin
        // Yapılandırılmamış 3. taraf bir aracın giriş sayfasını simüle eder
        [HttpGet("AdminPortalLogin")]
        public IActionResult AdminPortalLogin()
        {
            return View();
        }

        // POST: /A02/AdminPortalLogin
        [HttpPost("AdminPortalLogin")]
        public IActionResult AdminPortalLogin(string username, string password)
        {
            // ZAFİYET: Dağıtılmış bir serviste varsayılan kimlik bilgilerini aktif bırakmak
            if (username == "admin" && password == "admin")
            {
                TempData["Message"] = "Zafiyet Sömürüldü! Varsayılan 'admin/admin' kimlik bilgileriyle giriş yapıldı.";
                return RedirectToAction("AdminDashboard");
            }

            // Başarısız giriş
            ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        // GET: /A02/AdminDashboard
        // Giriş atlatıldıktan sonra açığa çıkan sayfa
        [HttpGet("AdminDashboard")]
        public IActionResult AdminDashboard()
        {
            // Gerçek bir uygulamada burada bir yetkilendirme çerezi (cookie) kontrol ederdiniz.
            // Gösterim amacıyla sadece sayfayı render ediyoruz.
            return View();
        }
    }
}