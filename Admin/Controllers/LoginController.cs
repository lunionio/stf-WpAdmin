using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                    TempData["LoginMessage"] = string.Empty;
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    TempData["LoginMessage"] = "Usuário ou senha invalida";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch
            {
                TempData["LoginMessage"] = "Usuário ou senha invalida";
                return RedirectToAction("Index", "Login");
            }
        }
    }
}