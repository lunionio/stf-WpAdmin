using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class EmpresasController : Controller
    {
        private int _idCliente;

        public EmpresasController()
        {
            _idCliente = PixCoreValues.UsuarioLogado.idCliente;
        }

        // GET: Empresas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        public ActionResult Salvar(EmpresaViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.Nome) && !string.IsNullOrEmpty(viewModel.Cnae))
                {
                    viewModel.status = 1;
                    viewModel.IdCliente = _idCliente;

                    if (viewModel.Id == 0)
                        viewModel.UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario;

                    viewModel.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                    viewModel.Ativo = true;
                    if (SaveEmpresa(viewModel))
                    {
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                }

                return View("Cadastrar", viewModel);
            }
            catch (Exception e)
            {
                return View("Cadastrar", viewModel);
            }
        }

        private bool SaveEmpresa(EmpresaViewModel model)
        {
            try
            {
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/wpEmpresas/SalvarEmpresas/" + model.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

                object envio = new
                {
                    empresa = new
                    {
                        model.Id,
                        CNAE_S = model.Cnae,
                        model.RazaoSocial,
                        model.Cnpj,
                        endereco = new
                        {
                            model.Cep,
                            model.Cidade,
                            model.Bairro,
                            NumeroLocal = model.Numero,
                            model.Complemento,
                            PixCoreValues.UsuarioLogado.IdUsuario,
                            model.UsuarioCriacao,
                            model.UsuarioEdicao,
                            model.IdCliente,
                            local = model.Rua
                        },
                        model.Nome,
                        model.UsuarioCriacao,
                        model.UsuarioEdicao,
                        model.Ativo,
                        model.status,
                        model.IdCliente, 
                    }
                };

                var helper = new ServiceHelper();
                var result = helper.Post<object>(url, envio);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível salvar o usuário.", e);
            }
        }

        public ActionResult Listar()
        {
            return View();
        }

        public ActionResult Editar(int id)
        {
            var empresaJson = GetEmpresa(id);

            return View();
        }

        public ActionResult Excluir(int id, string cnpj) //Necessário passar o CNPJ
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/DeletarEmpresa/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
            object envio = new
            {
                empresa = new
                {
                    id,
                    cnpj,
                }
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmpresas()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/BuscarEmpresas/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var empresas = helper.Get<IEnumerable<object>>(url);

            return Json(empresas, JsonRequestBehavior.AllowGet);
        }

        private ActionResult GetEmpresa(int empresaId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/BuscarPorId/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
            object envio = new
            {
                usuario.idCliente,
                id = empresaId,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}