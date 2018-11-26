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
            return View(new EmpresaViewModel());
        }

        public ActionResult Salvar(EmpresaViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.RazaoSocial) && !string.IsNullOrEmpty(viewModel.Cnpj))
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
                            DataCriacao = DateTime.UtcNow,
                            Id = model.EnderecoId,
                            model.Cep,
                            model.Cidade,
                            model.Bairro,
                            NumeroLocal = model.Numero,
                            model.Complemento,
                            PixCoreValues.UsuarioLogado.IdUsuario,
                            model.UsuarioCriacao,
                            model.UsuarioEdicao,
                            model.IdCliente,
                            Local = model.Rua,
                            model.status,
                            EmpresaId = model.Id,
                        },
                        model.Nome,
                        model.UsuarioCriacao,
                        model.UsuarioEdicao,
                        model.Ativo,
                        model.status,
                        model.IdCliente, 
                        model.Email,
                        Telefone = new
                        {
                            DataCriacao = DateTime.UtcNow,
                            Id = model.TelefoneId,
                            Numero = model.Telefone,
                            model.IdCliente,
                            model.status,
                            UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario,
                            UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario,
                            EmpresaId = model.Id,
                        },
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
            var empresa = GetEmpresa(id);

            return View("Cadastrar", empresa);
        }

        public ActionResult Excluir(int id)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/DeletarEmpresa/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
            object envio = new
            {
                empresa = new
                {
                    id,
                    cnpj = "0",
                }
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            return RedirectToAction("Listar");
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

        private EmpresaViewModel GetEmpresa(int empresaId)
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
            var result = helper.Post<Empresa>(url, envio);

            var empresa = new EmpresaViewModel()
            {
                Ativo = result.Ativo,
                Bairro = result.Endereco.Bairro,
                Cep = result.Endereco.CEP,
                Cidade = result.Endereco.Cidade,
                Cnae = result.CNAE_S,
                Cnpj = result.CNPJ,
                Complemento = result.Endereco.Complemento,
                Email = result.Email,
                EnderecoId = result.Endereco.ID,
                Id = result.ID,
                IdCliente = result.IdCliente,
                Nome = result.Nome,
                Numero = result.Endereco.NumeroLocal,
                RazaoSocial = result.RazaoSocial,
                Rua = result.Endereco.Local,
                status = result.Status,
                Telefone = result.Telefone.Numero,
                TelefoneId = result.Telefone.ID,
                Uf = result.Endereco.Uf,
                UsuarioCriacao = result.UsuarioCriacao,
                UsuarioEdicao = result.UsuarioEdicao,
            };

            return empresa;
        }
    }
}