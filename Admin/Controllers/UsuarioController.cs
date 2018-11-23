using Admin.Controllers.Attributes;
using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    [NoDirectAccess]
    public class UsuarioController : Controller
    {
        private int _idCliente;

        public UsuarioController()
        {
            _idCliente = PixCoreValues.IDCliente;
        }

        public ActionResult Cadastro()
        {
            var result = GetPermissoes();
            var empresas = GetEmpresas();

            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(result.Select(p => p.Nome));
            return View();
        }

        private static IEnumerable<PermissaoViewModel> GetPermissoes()
        {
            try
            {
                var url = ConfigurationManager.AppSettings["UrlAPI"];
                //var serverUrl = $"{ url }/permissao/getallpermissao/{ _idCliente }"; //TODO: Necessário cadastrar perfil com id do usuário
                var serverUrl = $"{ url }/permissao/getallpermissao/{ 1 }";

                var helper = new ServiceHelper();
                var result = helper.Get<IEnumerable<PermissaoViewModel>>(serverUrl);

                return result;
            }
            catch(Exception e)
            {
                return new List<PermissaoViewModel>();
            }
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

        public ActionResult Listagem()
        {
            var usuarios = GetUsuarios(_idCliente);
            return View(usuarios);
        }

        [HttpPost]
        public ActionResult Cadastro(UsuarioViewModel viewModel)
        {
            try
            {
                var result = GetPermissoes();
                var empresas = GetEmpresas();

                ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
                ViewBag.Perfis = new SelectList(result.Select(p => p.Nome));

                if (! string.IsNullOrEmpty(viewModel.Nome) && !string.IsNullOrEmpty(viewModel.Login) 
                    && !string.IsNullOrEmpty(viewModel.Senha) && !string.IsNullOrEmpty(viewModel.Perfil) && !string.IsNullOrEmpty(viewModel.Empresa))
                {
                    viewModel.idCliente = _idCliente;

                    if(viewModel.ID == 0)
                        viewModel.UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario;

                    viewModel.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                    viewModel.Ativo = true;
                    viewModel.PerfilUsuario = result.Where(r => r.Nome.Equals(viewModel.Perfil)).FirstOrDefault().ID;
                    viewModel.IdEmpresa = empresas.Where(r => r.Nome.Equals(viewModel.Empresa)).FirstOrDefault().Id;

                    if (SaveUsuario(viewModel))
                    {
                        ModelState.Clear();
                        return RedirectToAction("Listagem");
                    }
                }

                return View("Cadastro", viewModel);
            }
            catch (Exception e)
            {             
                return View("Cadastro", viewModel);
            }
        }

        public ActionResult Editar(int? id)
        {
            var result = GetPermissoes();
            var empresas = GetEmpresas();

            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(result.Select(p => p.Nome));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var usuarios = GetUsuarios(_idCliente);

            var usuarioFiltrado = usuarios.FirstOrDefault(x => x.ID.Equals(id));

            return View("Cadastro", usuarioFiltrado);
        }

        public ActionResult Excluir(int? id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var users = GetUsuarios(_idCliente);
            try
            {
                var usuario = users.FirstOrDefault(u => u.ID.Equals(id));

                if (DeleteUsuario(usuario))
                {
                    var usuarios = GetUsuarios(_idCliente);
                    return View("Listagem", usuarios);
                }

                return View("Listagem", users);
            }
            catch(Exception e)
            {
                return View("Listagem", users);
            }
        }

        private bool SaveUsuario(UsuarioViewModel usuario)
        {
            try
            {
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Principal/salvarUsuario/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

                object envio = new 
                {
                    usuario = new
                    {
                        usuario.ID,
                        usuario.idCliente,
                        usuario.Nome,
                        usuario.Login,
                        usuario.Senha,
                        usuario.UsuarioCriacao,
                        usuario.UsuarioEdicao,
                        usuario.Ativo,
                        usuario.IdEmpresa,
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

        private IEnumerable<UsuarioViewModel> GetUsuarios(int idCliente)
        {
            try
            {
                var perfis = GetPermissoes();
                var empresas = GetEmpresas();

                var url = ConfigurationManager.AppSettings["UrlAPI"];
                var serverUrl = $"{ url }/Seguranca/Principal/buscarUsuario/{ _idCliente }/{ PixCoreValues.UsuarioLogado.IdUsuario }";

                var helper = new ServiceHelper();
                var result = helper.Get<IEnumerable<UsuarioViewModel>>(serverUrl);

                foreach (var item in result)
                {
                    item.Empresa = empresas.FirstOrDefault(e => e.Id.Equals(item.IdEmpresa))?.Nome;
                    item.Perfil = perfis.FirstOrDefault(p => p.ID.Equals(item.PerfilUsuario))?.Nome;
                }

                return result;
   
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível listar os usuários.", e);
            }
        }

        private bool DeleteUsuario(UsuarioViewModel usuario)
        {
            try
            {
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Principal/DeletarUsuario/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
                object envio = new
                {
                    usuario = new
                    {
                        idUsuario = usuario.ID
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

        public ActionResult EditarUsuario()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/BuscarUsuarioPorId/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                usuario.idCliente,
                idUsuario = usuario.IdUsuario,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<UsuarioViewModel>(url, envio);

            ViewBag.Empresas = new SelectList(GetEmpresas().Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(GetPermissoes().Select(p => p.Nome));

            return View("Editar", result);
        }

        public ActionResult AltararUsuario(UsuarioViewModel model)
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Principal/salvarUsuario/" + usuario.idCliente + "/" + usuario.IdUsuario;

                model.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                model.Ativo = true;
                model.idCliente = _idCliente;
                model.Status = 1;

                var envio = new
                {
                    usuario = model,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<UsuarioViewModel>(url, envio);

                PixCoreValues.AtualizarUsuarioLogado(result);

                ViewBag.Empresas = new SelectList(GetEmpresas().Select(e => e.Nome));
                ViewBag.Perfis = new SelectList(GetPermissoes().Select(p => p.Nome));

                return RedirectToAction("Editar", result);
            }
            catch(Exception e)
            {
                throw new Exception("Não foi possível editar o usuário.", e);
            }
        }
    }
}