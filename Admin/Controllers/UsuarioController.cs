using Admin.Controllers.Attributes;
using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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
            var result = GetPerfis().Select(p => p.Nome);
            var empresas = GetEmpresas();

            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(result);
            return View();
        }

        private IEnumerable<Perfil> GetPerfis()
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Perfil/GetAllPerfil/" + _idCliente;

                var helper = new ServiceHelper();
                var result = helper.Get<IEnumerable<Perfil>>(url);

                return result;
            }
            catch (Exception e)
            {
                return new List<Perfil>();
            }
        }

        public IEnumerable<EmpresaViewModel> GetEmpresas()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpEmpresas/BuscarEmpresas/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var empresas = helper.Get<IEnumerable<Empresa>>(url).Where(e => e.ID != 52 && e.ID != 53); ;

            IList<EmpresaViewModel> models = new List<EmpresaViewModel>();

            foreach (var result in empresas)
            {
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
                    Numero = Convert.ToInt32(result.Endereco.NumeroLocal),
                    RazaoSocial = result.RazaoSocial,
                    Rua = result.Endereco.Local,
                    status = result.Status,
                    Telefone = result.Telefone.Numero,
                    TelefoneId = result.Telefone.ID,
                    Uf = result.Endereco.Uf,
                    UsuarioCriacao = result.UsuarioCriacao,
                    UsuarioEdicao = result.UsuarioEdicao,
                };

                models.Add(empresa);
            }

            return models;
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
                var result = GetPerfis();
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
                    viewModel.IdEmpresa = empresas.Where(r => r.Nome.Equals(viewModel.Empresa)).FirstOrDefault().Id;
                    viewModel.VAdmin = "true";
                    viewModel.Status = 1;

                    var user = SaveUsuario(viewModel);
                    var perfil = result.SingleOrDefault(p => p.Nome.Equals(viewModel.Perfil));

                    if (user.ID > 0)
                    {
                        ModelState.Clear();

                        var usuarioXPerfil = VincularPerfil(user.ID, perfil.ID, viewModel.UsuarioXPerfil.Id);

                        if (usuarioXPerfil.Id > 0)
                        {
                            return RedirectToAction("Listagem");
                        }
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
            var result = GetPerfis();
            var empresas = GetEmpresas();

            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(result.Select(p => p.Nome));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var usuarioFiltrado = GetUsuario((int)id, _idCliente);
            usuarioFiltrado.Empresa = empresas.FirstOrDefault(e => e.Id.Equals(usuarioFiltrado.IdEmpresa))?.Nome;
            usuarioFiltrado.Perfil = result.FirstOrDefault(p => p.ID.Equals(usuarioFiltrado.UsuarioXPerfil.IdPerfil))?.Nome;

            return View("Cadastro", usuarioFiltrado);
        }

        public ActionResult Excluir(int? id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var usuarios = GetUsuarios(_idCliente);

            try
            {
                var usuario = usuarios.FirstOrDefault(u => u.ID.Equals(id));
                if (DeleteUsuario(usuario))
                {
                    return View("Listagem", GetUsuarios(_idCliente));
                }

                return View("Listagem", usuarios);
            }
            catch(Exception e)
            {
                return View("Listagem", usuarios);
            }
        }

        private UsuarioViewModel SaveUsuario(UsuarioViewModel usuario)
        {
            try
            {
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Principal/salvarUsuario/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

                object envio = new 
                {
                    usuario,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<UsuarioViewModel>(url, envio);

                return result;
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
                var perfis = GetPerfis();
                var empresas = GetEmpresas();

                var url = ConfigurationManager.AppSettings["UrlAPI"];
                var serverUrl = $"{ url }/Seguranca/Principal/buscarUsuario/{ _idCliente }/{ PixCoreValues.UsuarioLogado.IdUsuario }";

                var helper = new ServiceHelper();
                var result = helper.Get<IEnumerable<UsuarioViewModel>>(serverUrl);
                var usuariosXPerfis = GetPerfisUsuarios(result.Select(u => u.ID));

                foreach (var item in result)
                {
                    item.Empresa = empresas.FirstOrDefault(e => e.Id.Equals(item.IdEmpresa))?.Nome;
                    item.UsuarioXPerfil = usuariosXPerfis.FirstOrDefault(x => x.IdUsuario.Equals(item.ID));
                    if (item.UsuarioXPerfil != null)
                    {
                        item.Perfil = perfis.FirstOrDefault(p => p.ID.Equals(item.UsuarioXPerfil.IdPerfil))?.Nome;
                    }
                }

                return result.Where(u => u.Ativo && u.Status != 9);   
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
                if (PixCoreValues.UsuarioLogado.IdUsuario.Equals(usuario.ID))
                {
                    return false;
                }

                usuario.Ativo = false;
                usuario.Status = 9;

                var result = SaveUsuario(usuario);

                if(result.Status == 9 && !result.Ativo)
                {
                    return true;
                }

                return false;
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

            result.UsuarioXPerfil = GetPerfilUsuario(result.ID);
            result.Perfil = GetPerfil(result.UsuarioXPerfil.IdPerfil).Nome;

            var empresas = GetEmpresas();

            result.Empresa = empresas.SingleOrDefault(e => e.Id.Equals(result.IdEmpresa))?.Nome;

            ViewBag.Empresas = new SelectList(empresas.Select(e => e.Nome));
            ViewBag.Perfis = new SelectList(GetPerfis().Select(p => p.Nome));

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
                ViewBag.Perfis = new SelectList(GetPerfis().Select(p => p.Nome));

                return RedirectToAction("EditarUsuario");
            }
            catch(Exception e)
            {
                throw new Exception("Não foi possível editar o usuário.", e);
            }
        }

        private UsuarioXPerfil VincularPerfil(int usuarioId, int perfilId, int vinculoId = 0)
        {
            try
            {
                var usuarioXPerfil = new UsuarioXPerfil()
                {
                    Id = vinculoId,
                    DataCriacao = DateTime.UtcNow,
                    DataEdicao = DateTime.UtcNow,
                    IdPerfil = perfilId,
                    IdUsuario = usuarioId,
                    UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario,
                    UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario,
                };

                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = $"{ keyUrl }/Perfil/SaveUsuarioXPerfil/";

                var helper = new ServiceHelper();
                var result = helper.Post<UsuarioXPerfil>(url, usuarioXPerfil);

                return result;
            }
            catch (Exception e)
            {
                return new UsuarioXPerfil();
            }
        }

        private IEnumerable<UsuarioXPerfil> GetPerfisUsuarios(IEnumerable<int> ids)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = $"{ keyUrl }/Perfil/GetUsuariosXPerfis/";

            var helper = new ServiceHelper();
            var usuariosXPerfis = helper.Post<IEnumerable<UsuarioXPerfil>>(url, ids);

            return usuariosXPerfis;
        }

        private void DesvincularPerfil(int id)
        {
            var usuarioXPerfil = GetPerfilUsuario(id);

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = $"{ keyUrl }/Perfil/DesvincularPerfil/";

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, usuarioXPerfil);
        }

        private UsuarioXPerfil GetPerfilUsuario(int usuarioId)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = $"{ keyUrl }/Perfil/GetPerfilByUsuario/{ usuarioId }";

            var helper = new ServiceHelper();
            var usuarioXPerfil = helper.Get<UsuarioXPerfil>(url);

            return usuarioXPerfil;
        }

        private Perfil GetPerfil(int idPerfil)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = $"{ keyUrl }/Perfil/GetPerfilByID/{ idPerfil }";

            var helper = new ServiceHelper();
            var result = helper.Get<Perfil>(url);

            return result;
        }

        public UsuarioViewModel GetUsuario(int id, int idCliente)
        {
            try
            {
                var url = ConfigurationManager.AppSettings["UrlAPI"];
                var serverUrl = $"{ url }/Seguranca/Principal/BuscarUsuarioPorId/{ _idCliente }/{ PixCoreValues.UsuarioLogado.IdUsuario }";

                var envio = new
                {
                    idCliente,
                    idUsuario = id,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<UsuarioViewModel>(serverUrl, envio);

                result.UsuarioXPerfil = GetPerfilUsuario(result.ID);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível listar os usuários.", e);
            }
        }
    }
}