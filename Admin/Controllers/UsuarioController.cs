using Admin.Controllers.Attributes;
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
                IEnumerable<PermissaoViewModel> result;
                using (var client = new WebClient())
                {
                    var jss = new JavaScriptSerializer();
                    var url = ConfigurationManager.AppSettings["UrlAPI"];
                    //var serverUrl = $"{ url }/permissao/getallpermissao/{ _idCliente }"; //TODO: Necessário cadastrar perfil com id do usuário
                    var serverUrl = $"{ url }/permissao/getallpermissao/{ 1 }";
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var response = client.DownloadString(new Uri(serverUrl));
                    result = jss.Deserialize<IEnumerable<PermissaoViewModel>>(response);
                }

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

            var result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            var jss = new JavaScriptSerializer();
            var empresas = jss.Deserialize<IEnumerable<EmpresaViewModel>>(result);

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
                        ViewData["Resultado"] = new ResultadoViewModel("Usuário cadastrado com sucesso!", true);
                        ModelState.Clear();

                        return RedirectToAction("Listagem");
                    }
                }

                ViewData["Resultado"] = new ResultadoViewModel("Informe todos os dados necessários.", false);
                return View("Cadastro", viewModel);
            }
            catch (Exception e)
            {
                ViewData["Resultado"] = new ResultadoViewModel("Não foi possível salvar o usuário.", false);                
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

                ViewData["ResultadoDelete"] = new ResultadoViewModel("Não foi possível deletar o usuário.", false);
                return View("Listagem", users);
            }
            catch(Exception e)
            {
                ViewData["ResultadoDelete"] = new ResultadoViewModel("Não foi possível deletar o usuário.", false);
                return View("Listagem", users);
            }
        }

        private bool SaveUsuario(UsuarioViewModel usuario)
        {
            try
            {
                var jss = new JavaScriptSerializer();
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
                var data = jss.Serialize(envio);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) 
                        || "null".Equals(result.ToLower()))
                    {
                        throw new Exception("Ouve um erro durante o processo.");
                    }
                }

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

                using (var client = new WebClient())
                {
                    var jss = new JavaScriptSerializer();
                    var url = ConfigurationManager.AppSettings["UrlAPI"];
                    var serverUrl = $"{ url }/Seguranca/Principal/buscarUsuario/{ _idCliente }/{ PixCoreValues.UsuarioLogado.IdUsuario }";
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var response = client.DownloadString(new Uri(serverUrl));
                    var result = jss.Deserialize<IEnumerable<UsuarioViewModel>>(response);

                    foreach (var item in result)
                    {
                        item.Empresa = empresas.FirstOrDefault(e => e.Id.Equals(item.IdEmpresa))?.Nome;
                        item.Perfil = perfis.FirstOrDefault(p => p.ID.Equals(item.PerfilUsuario))?.Nome;
                    }

                    return result;
                }
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
                var jss = new JavaScriptSerializer();
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Principal/DeletarUsuario/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
                object envio = new
                {
                    usuario = new
                    {
                        idUsuario = usuario.ID
                    }
                };
                var data = jss.Serialize(envio);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result)
                        || "null".Equals(result.ToLower()))
                    {
                        throw new Exception("Ouve um erro durante o processo.");
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível salvar o usuário.", e);
            }
        }
    }
}