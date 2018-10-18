using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ProfissionaisController : Controller
    {
        // GET: Profissionais
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProfissionais()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarProfissionais/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var profissionais = helper.Get<IEnumerable<Profissional>>(url);

            IList<ProfissionalViewModel> ret = new List<ProfissionalViewModel>();

            foreach (var p in profissionais)
            {
                ret.Add(new ProfissionalViewModel(p.ID, p.Nome, string.Empty, p.Telefone.Numero, p.Telefone.ID, null, p.Email, p.Endereco));
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detalhes(int id)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarPorId/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                idProfissional = id,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<Profissional>(url, envio);

            var ret = new ProfissionalViewModel(response.ID, response.Nome, string.Empty, response.Telefone.Numero, response.Telefone.ID, 
                null, response.Email, response.Endereco);

            ret.Documentos = GetDocumentos(ret.Id);

            var user = GetUsuario(response.IdUsuario);

            ret.Avatar = user.Avatar;

            ViewBag.Statuses = GetAllDocumentoStatus();

            return View(ret);
        }

        private IList<DocumentoViewModel> GetDocumentos(int profissionalId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpDocumento/BuscarPorCodigo/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                codigoExterno = profissionalId,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<IEnumerable<Documento>>(url, envio);

            IList<DocumentoViewModel> models = new List<DocumentoViewModel>();

            foreach (var item in response)
            {
                models.Add(new DocumentoViewModel(item.ID, item.DocumentoTipo.Nome, item.Tipo, 
                    item.DocumentoStatusID, item.DocumentoStatus.Nome, item.DataCriacao.ToString(), item.Arquivo));
            }

            return models;
        }

        private Usuario GetUsuario(int usuarioId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/BuscarUsuarioPorId/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                idUsuario = usuarioId,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<Usuario>(url, envio);

            return response;
        }

        [HttpPost]
        public ActionResult Alterar(ProfissionalViewModel profissional)
        {
            if(profissional.Id != 0) //Necessário ID
            {
                var p = new Profissional(profissional.Id, profissional.Nome, profissional.Telefone, profissional.Email, profissional.Endereco) 
                {
                    DataEdicao = DateTime.UtcNow,
                    UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario,
                    IdCliente = PixCoreValues.UsuarioLogado.idCliente,
                };

                p.Telefone.DataEdicao = DateTime.UtcNow;
                p.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                p.IdCliente = PixCoreValues.UsuarioLogado.idCliente;
                p.Endereco.UsuarioEdicao = PixCoreValues.UsuarioLogado.IdUsuario;
                p.Endereco.IdCliente = PixCoreValues.UsuarioLogado.idCliente;
                p.Endereco.DataEdicao = DateTime.UtcNow;
                p.Endereco.ProfissionalId = profissional.Id;
                p.Endereco.Nome = p.Nome;
                p.Telefone.ID = profissional.TelefoneId;

                foreach (var documento in profissional.Documentos)
                {
                    if(documento.IdStatus == 3) //Reprovado
                    {
                        //Muda o status do usuário, Criar Enumerador para Status da base
                    }
                }

                p.DataCriacao = profissional.DataCriacao;
                var result = PostProfissional(p);

                //Atualiza o documento
            }

            return RedirectToAction("index");
        }

        public string PostProfissional(Profissional profissional)
        {            
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/SalvarProfissional/" + profissional.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                profissional,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<string>(url, envio);

            return result;            
        }

        public IEnumerable<DocumentoStatus> GetAllDocumentoStatus()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpDocumento/BuscaTodosStatus/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var statuses = helper.Get<IEnumerable<DocumentoStatus>>(url);

            return statuses;
        }
    }
}