using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class AtendimentoController : Controller
    {
        // GET: Atendimento
        public ActionResult Index()
        {
            //var teste = new AtendimentoViewModel()
            //{
            //    Ativo = true,
            //    Descricao = "Teste",
            //    IdCliente = PixCoreValues.UsuarioLogado.idCliente,
            //    IdUserAdmin = PixCoreValues.UsuarioLogado.IdUsuario,
            //    Nome = "Teste",
            //    Resposta = "Resposta para o ticket X",
            //    Status = 1,
            //    TicketID = 29,
            //};

            //return View();
            //return ResponderTicket(teste);
            return GetAtendimentos();
        }

        public ActionResult ResponderTicket(AtendimentoViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.Nome) && !string.IsNullOrEmpty(viewModel.Resposta))
                {
                    viewModel.IdCliente = PixCoreValues.UsuarioLogado.idCliente;

                    if (viewModel.ID == 0)
                        viewModel.UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario;

                    viewModel.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                    viewModel.Ativo = true;
                    if (EnviaAtendimento(viewModel))
                    {
                        ViewData["ResultadoAtendimento"] = new ResultadoViewModel("Resposta enviada com sucesso!", true);
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewData["ResultadoAtendimento"] = new ResultadoViewModel("Informe todos os dados necessários.", false);
                return View("Index", viewModel);
            }
            catch (Exception e)
            {
                ViewData["ResultadoAtendimento"] = new ResultadoViewModel("Não foi possível enviar a resposta.", false);
                return View("Index", viewModel);
            }
        }

        public ActionResult GetAtendimentos()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpAtendimento/BuscarTickets/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

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
            var tickets = jss.Deserialize<IEnumerable<object>>(result);
            return Json(tickets, JsonRequestBehavior.AllowGet);
        }

        private bool EnviaAtendimento(AtendimentoViewModel model)
        {
            try
            {
                var jss = new JavaScriptSerializer();
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/wpAtendimento/TicketResposta/" + model.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

                object envio = new
                {
                   atendimento = model
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