﻿using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using Admin.Models.Financeiro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

            var envio = new
            {
                usuario.idCliente,
            };

            var helper = new ServiceHelper();
            var empresas = helper.Post<IEnumerable<Empresa>>(url, envio).Where(e => e.ID != 52 && e.ID != 53);

            IList<EmpresaViewModel> models = new List<EmpresaViewModel>();

            foreach (var item in empresas)
            {
                var empresa = new EmpresaViewModel()
                {
                    Ativo = item.Ativo,
                    Bairro = item.Endereco.Bairro,
                    Cep = item.Endereco.CEP,
                    Cidade = item.Endereco.Cidade,
                    Cnae = item.CNAE_S,
                    Cnpj = item.CNPJ,
                    Complemento = item.Endereco.Complemento,
                    Email = item.Email,
                    EnderecoId = item.Endereco.ID,
                    Id = item.ID,
                    IdCliente = item.IdCliente,
                    Nome = item.Nome,
                    Numero = Convert.ToInt32(item.Endereco.NumeroLocal),
                    RazaoSocial = item.RazaoSocial,
                    Rua = item.Endereco.Local,
                    status = item.Status,
                    Telefone = item.Telefone.Numero,
                    TelefoneId = item.Telefone.ID,
                    Uf = item.Endereco.Uf,
                    UsuarioCriacao = item.UsuarioCriacao,
                    UsuarioEdicao = item.UsuarioEdicao,
                };

                models.Add(empresa);
            }

            return models;
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
                creditoViewModel.Taxa = decimal.Parse(creditoViewModel.TaxaString, CultureInfo.GetCultureInfo("pt-BR"));

                var naturezas = GetNaturezas();
                var empresas = GetEmpresas();

                ViewBag.Naturezas = new SelectList(naturezas.Select(n => n.Nome));
                ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));

                if (!string.IsNullOrEmpty(creditoViewModel.Empresa) 
                    && !string.IsNullOrEmpty(creditoViewModel.Natureza))
                {
                    var empresa = empresas.FirstOrDefault(e => e.Nome.Equals(creditoViewModel.Empresa));

                    creditoViewModel.EmpresaId = empresa?.Id;
                    creditoViewModel.NaturezaId = naturezas.FirstOrDefault(e => e.Nome.Equals(creditoViewModel.Natureza))?.ID;

                    FinanceiroHelper.InserirSaldo(decimal.Parse(creditoViewModel.Valor, CultureInfo.GetCultureInfo("pt-BR")), "52", 
                        creditoViewModel.EmpresaId.ToString(), (int)creditoViewModel.NaturezaId, 1, 
                        creditoViewModel.Descricao, PixCoreValues.UsuarioLogado, empresa?.Email);

                    if(creditoViewModel.Taxa > 0)
                    {
                        var taxa = creditoViewModel.Taxa / 100;
                        var valor = decimal.Parse(creditoViewModel.Valor, CultureInfo.GetCultureInfo("pt-BR")) * taxa;

                        FinanceiroHelper.LancaTransacoes(valor * -1, creditoViewModel.EmpresaId.ToString(),
                            3, creditoViewModel.EmpresaId.ToString(), 3, 8, 1, "Pagamento de taxa.", PixCoreValues.UsuarioLogado);

                        FinanceiroHelper.LancaTransacoes(valor, creditoViewModel.EmpresaId.ToString(),
                            3, "53", 2, 8, 1, "Pagamento de taxa.", PixCoreValues.UsuarioLogado);
                    }

                    ModelState.Clear();
                    TempData["Message"] = "Créditos inseridos com sucesso";
                    return View("Index");
                }

                TempData["Message"] = "Preencha todos os campos.";
                return View("Index");
            }
            catch(Exception e)
            {
                TempData["Message"] = "Não foi possível lançar os créditos.";
                return View("Index");
            }
        }
    }
}