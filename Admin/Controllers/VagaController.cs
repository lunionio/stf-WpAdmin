using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using Newtonsoft.Json;
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
    public class VagaController : Controller
    {
        // GET: Vaga
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PublicarAgora(VagaViewModel vaga)
        {
            vaga.status = 1;
            vaga.DataEvento = Convert.ToDateTime(vaga.Date);
            if (SaveVaga(vaga))
                return Json("ok");
            else
                return Json("Desculpe, o sistema encontrou um erro ao efetuar sua solicitação." +
                    " Entre em contato com nosso suporte técnico.");
        }

        [HttpPost]
        public ActionResult PublicarMaisTarde(VagaViewModel vaga)
        {
            vaga.status = 2;
            if (SaveVaga(vaga))
                return RedirectToAction("Gerenciar");
            else
                return Json("Desculpe, o sistema encontrou um erro ao efetuar sua solicitação. Entre em contato" +
                    " com nosso suporte técnico.");
        }
        
        public PartialViewResult ModalConfirmarVaga(VagaViewModel model)
        {
            return PartialView(model);
        }

        public PartialViewResult _listarOportunidades()
        {
            IList<VagaViewModel> vagas = GetOportunidades();

            return PartialView(vagas);
        }

        private static IList<VagaViewModel> GetOportunidades()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpOportunidades/BuscarOportunidadePorEmpresa/" + usuario.idCliente + "/" +
                PixCoreValues.UsuarioLogado.IdUsuario;

            var envio = new
            {
                usuario.idEmpresa,
            };

            var helper = new ServiceHelper();
            var oportunidades = helper.Post<IEnumerable<OportunidadeViewModel>>(url, envio);

            var vagas = new List<VagaViewModel>();

            foreach (var o in oportunidades)
                vagas.Add(new VagaViewModel() { Id = o.ID, Nome = o.Nome, ProfissionalNome = o.DescProfissional });
            return vagas;
        }

        public PartialViewResult _listarProfissionais()
        {
            int optId = 51;
            var userXOportunidades = GetProfissionaisByOpt(optId);
            var profissionais = GetProfissionais(userXOportunidades.Select(x => x.UserId));
            var users = GetUsers(profissionais.Select(x => x.UsuarioId));


            return PartialView();
        }

        private IEnumerable<ProfissionalServico> GetProfissionais(IEnumerable<int> ids)
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/wpProfissionais/BuscarPorIds/" + usuario.idCliente + "/" +
                    PixCoreValues.UsuarioLogado.IdUsuario;

                var envio = new
                {
                    ids,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<IEnumerable<ProfissionalServico>>(url, envio);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível completar a operação.", e);
            }
        }

        private IEnumerable<Usuario> GetUsers(IEnumerable<int> ids)
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/Pincipal/BuscarUsuarios/" + usuario.idCliente + "/" +
                    PixCoreValues.UsuarioLogado.IdUsuario;

                var envio = new
                {
                    ids,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<IEnumerable<Usuario>>(url, envio);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível completar a operação.", e);
            }
        }

        private IEnumerable<UserXOportunidade> GetProfissionaisByOpt(int idOpt)
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/WpOportunidades/BuscarUsuariosPorOportunidade/" + usuario.idCliente + "/" +
                    PixCoreValues.UsuarioLogado.IdUsuario;

                var envio = new
                {
                    idOpt,
                };

                var helper = new ServiceHelper();
                var result = helper.Post<IEnumerable<UserXOportunidade>>(url, envio);

                return result;
            }
            catch(Exception e)
            {
                throw new Exception("Não foi possível completar a operação.", e);
            }
        }

        public PartialViewResult _vincularPorifissionais()
        {
            var t = new UserXOportunidade()
            {
                OportunidadeId = 38,
                UserId = 3,
                StatusID = 2
            };

            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpOportunidades/CanditarOportunidade/" + usuario.idCliente + "/" +
                PixCoreValues.UsuarioLogado.IdUsuario;

            var envio = new
            {
                userXOportunidade = t,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<IEnumerable<UserXOportunidade>>(url, envio);

            return PartialView();
        }

        private static bool SaveVaga(VagaViewModel vaga)
        {
            try
            {
                var usuario = PixCoreValues.UsuarioLogado;
                var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                var url = keyUrl + "/Seguranca/WpOportunidades/SalvarOportunidade/" + usuario.idCliente + "/" +
                    PixCoreValues.UsuarioLogado.IdUsuario;

                vaga.status = 1;

                var op = Oportundiade.Convert(vaga);

                var envio = new
                {
                    oportunidade = op,
                };

                var helper = new ServiceHelper();
                var resut = helper.Post<object>(url, envio);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível salvar o usuário.", e);
            }
        }  
    }
}