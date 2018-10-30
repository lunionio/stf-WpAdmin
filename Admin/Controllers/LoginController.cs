using Admin.Helppser;
using Admin.Models;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string senha)
        {
            var collection = new LoginViewModel
            {
                Login = login,
                Senha = senha
            };

            try
            {
                if (PixCoreValues.Login(collection))
                {
                    // Response.Redirect(PixCoreValues.defaultSiteUrl);

                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    ViewData["TemporariaMensagem"] = "Usuario ou senha invalida";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch
            {
                ViewData["TemporariaMensagem"] = "Usuario ou senha invalida";
                return RedirectToAction("Index", "Login");
            }

            
        }
    }
}