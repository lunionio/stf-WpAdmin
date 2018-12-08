using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using Admin.Models.FaleConosco;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class AtendimentoController : Controller
    {
        // GET: Atendimento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Editar(int id)
        {
            ViewBag.Id = id;

            var result = GetTicket(id);
            var ticket = JsonConvert.DeserializeObject<Ticket>(result);

            var model = new AtendimentoViewModel(ticket.Email, ticket.Origem, ticket.Tipo.Nome, ticket.Numero, ticket.Descricao, ticket.ID, ticket.TicketStatusID);
            return View(model);
        }

        public ActionResult Excluir(int id)
        {
            var usuario = PixCoreValues.UsuarioLogado;

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpAtendimento/DesativarTicket/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;
            object envio = new
            {
                ticket = new
                {
                    id,
                }
            };

            var helper = new ServiceHelper();
            var result = helper.Post<string>(url, envio);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string ResponderTicket(AtendimentoViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.Resposta))
                {
                    viewModel.IdCliente = PixCoreValues.UsuarioLogado.idCliente;

                    if (viewModel.ID == 0)
                        viewModel.UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario;

                    viewModel.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                    viewModel.Ativo = true;

                    if (EnviaAtendimento(viewModel))
                    {
                        ModelState.Clear();
                        return "ok";
                    }
                }

                return "Não foi possível realizar o atendimento.";
            }
            catch (Exception e)
            {
                return "Não foi possível realizar o atendimento.";
            }
        }

        public ActionResult GetAtendimentos()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpAtendimento/BuscarTickets/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var tickets = helper.Get<IEnumerable<object>>(url);

            return Json(tickets, JsonRequestBehavior.AllowGet);
        }

        private bool EnviaAtendimento(AtendimentoViewModel model)
        {
            try
            {
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/wpAtendimento/TicketResposta/" + model.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

                var atendimento = new Atendimento(model.IdCliente, model.Resposta, model.TicketId)
                {
                    IdCliente = model.IdCliente,
                    Nome = model.Numero,
                    UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario,
                    UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario,
                    Status = 1,
                    Descricao = model.Descricao,
                };

                object envio = new
                {
                   atendimento,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<string>(url, envio);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível responder o ticket.", e);
            }
        }

        private string GetTicket(int ticketId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpAtendimento/BuscarTicketPorID/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                id = ticketId,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            return JsonConvert.SerializeObject(result);
        }
    }
}