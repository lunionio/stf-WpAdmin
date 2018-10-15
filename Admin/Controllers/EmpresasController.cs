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
            //var empresas = GetEmpresas();
            return View();
        }

        public ActionResult Salvar(EmpresaViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.Nome) && !string.IsNullOrEmpty(viewModel.Cnae))
                {
                    viewModel.IdCliente = _idCliente;

                    if (viewModel.Id == 0)
                        viewModel.UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario;

                    viewModel.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                    viewModel.Ativo = true;
                    if (SaveEmpresa(viewModel))
                    {
                        ViewData["ResultadoEmpresa"] = new ResultadoViewModel("Empresa cadastrada com sucesso!", true);
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewData["ResultadoEmpresa"] = new ResultadoViewModel("Informe todos os dados necessários.", false);
                return View("Cadastrar", viewModel);
            }
            catch (Exception e)
            {
                ViewData["ResultadoEmpresa"] = new ResultadoViewModel("Não foi possível salvar o usuário.", false);
                return View("Cadastrar", viewModel);
            }
        }

        private bool SaveEmpresa(EmpresaViewModel model)
        {
            try
            {
                var jss = new JavaScriptSerializer();
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
                            model.UsuarioEdicao
                        },
                        model.Nome,
                        model.UsuarioCriacao,
                        model.UsuarioEdicao,
                        model.Ativo,
                        model.status,
                        model.IdCliente
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

        public ActionResult Listar()
        {
            return View();
        }

        public ActionResult Editar(int id)
        {
            //preciso de um get por id aqui
            return View();
        }
        public ActionResult Excluir(int id)
        {
            //preciso da funcionalidade de excluir aqui
            return View();
        }


        public ActionResult GetEmpresas()
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
            var empresas = jss.Deserialize<IEnumerable<object>>(result);
            return Json(empresas, JsonRequestBehavior.AllowGet);
        }
    }
}