using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ProfissionaisController : Controller
    {
        // GET: Profissionais
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProfissionais()
        {
            var ret = new List<ProfissionalViewModel>
            {
                new ProfissionalViewModel()
                {
                    Nome = "José Teste",
                    Especialidade = "Motorista",
                    Email = "umdia@colocarlousa.com.br",
                    Telefone = "11 96969 9696"
                }
            };

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detalhes(int id)
        {
            var ret = new ProfissionalViewModel()
            {
                Nome = "José Teste",
                Especialidade = "Motorista",
                Email = "umdia@colocarlousa.com.br",
                Telefone = "11 96969 9696"
            };
            
            return View(ret);
        }

        [HttpPost]
        public ActionResult Alterar(ProfissionalViewModel profissional)
        {
            return RedirectToAction("index");
        }

    }
}