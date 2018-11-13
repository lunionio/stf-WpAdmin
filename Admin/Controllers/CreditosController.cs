using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using Admin.Models.Financeiro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class CreditosController : Controller
    {
        // GET: Creditos
        public ActionResult Index()
        {
            var naturezas = GetNaturezas();
            var empresas = GetEmpresas();

            ViewBag.Naturezas = new SelectList(naturezas.Select(n => n.Nome));
            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            return View();
        }

        public IEnumerable<EmpresaViewModel> GetEmpresas()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/BuscarEmpresas/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var empresas = helper.Get<IEnumerable<EmpresaViewModel>>(url);

            return empresas;
        }

        public IEnumerable<Natureza> GetNaturezas()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/BuscarNaturezas/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var naturezas = helper.Get<IEnumerable<Natureza>>(url);

            return naturezas;
        }

        [HttpPost]
        public ActionResult Inserir(CreditoViewModel creditoViewModel)
        {
            try
            {
                var naturezas = GetNaturezas();
                var empresas = GetEmpresas();

                ViewBag.Naturezas = new SelectList(naturezas.Select(n => n.Nome));
                ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));

                if (!string.IsNullOrEmpty(creditoViewModel.Empresa) 
                    && !string.IsNullOrEmpty(creditoViewModel.Natureza) && creditoViewModel.Valor > 0)
                {
                    creditoViewModel.EmpresaId = empresas.FirstOrDefault(e => e.Nome.Equals(creditoViewModel.Empresa))?.Id;
                    creditoViewModel.NaturezaId = naturezas.FirstOrDefault(e => e.Nome.Equals(creditoViewModel.Natureza))?.ID;

                    FinanceiroHelper.InserirSaldo(creditoViewModel.Valor, "16", 
                        creditoViewModel.EmpresaId.ToString(), (int)creditoViewModel.NaturezaId, 1, creditoViewModel.Descricao, PixCoreValues.UsuarioLogado);

                    if(creditoViewModel.Taxa > 0)
                    {
                        var taxa = creditoViewModel.Taxa / 100;
                        var valor = creditoViewModel.Valor * taxa;

                        FinanceiroHelper.LancaTransacoes(valor * -1, creditoViewModel.EmpresaId.ToString(),
                            3, creditoViewModel.EmpresaId.ToString(), 3, 8, 1, "Pagamento de taxa.", PixCoreValues.UsuarioLogado);

                        FinanceiroHelper.LancaTransacoes(valor, creditoViewModel.EmpresaId.ToString(),
                            3, "3", 2, 8, 1, "Pagamento de taxa.", PixCoreValues.UsuarioLogado);
                    }

                    ModelState.Clear();
                    return View("Index");
                }

                ViewBag.ErrorMessage = "Preencha todos os campos.";
                return View("Index");
            }
            catch(Exception e)
            {
                ViewBag.ErrorMessage = "Não foi possível lançar os créditos.";
                return View("Index");
            }
        }
    }
}