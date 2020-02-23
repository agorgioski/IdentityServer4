using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RedirectToAdmin() 
        {
            return RedirectToAction("Index", "Admin", new { area = "Admin"});
        }

        public IActionResult RedirectToCase() 
        {
            return RedirectToAction("Index", "Case", new { area = "Case"});
        }
        
        [ActionName("LogOff")]
        public async Task LogOff()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }
    }
}