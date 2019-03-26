using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarProfissionais/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var helper = new ServiceHelper();
            var profissionais = helper.Get<IEnumerable<ProfissionalServico>>(url);

            var usuarios = GetUsuarios(profissionais.Select(x => x.Profissional.IdUsuario));

            IList<ProfissionalViewModel> ret = new List<ProfissionalViewModel>();

            foreach (var p in profissionais)
            {

                ret.Add(new ProfissionalViewModel(p.Profissional.ID, p.Profissional.Nome, p.Servico.Nome, p.Profissional.Telefone.Numero,
                    p.Profissional.Telefone.ID, p.Profissional.DataNascimento.ToString(), p.Profissional.Email, p.UsuarioId, p.Profissional.Endereco));
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detalhes(int id)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarPorId/" + usuario.idCliente + "/" + usuario.IdUsuario;

            object envio = new
            {
                idProfissional = id,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<ProfissionalServico>(url, envio);
            var user = GetUsuario(response.Profissional.IdUsuario);
            var jobs = GetJobQuantidade(response.Profissional.ID);

            var ret = new ProfissionalViewModel(response.Profissional.ID, user.Nome, response.Nome, response.Profissional.Telefone.Numero, response.Profissional.Telefone.ID, 
                response.Profissional.DataNascimento.ToShortDateString(), response.Profissional.Email,
                response.Profissional.IdUsuario, response.Profissional.Endereco){ DataCriacao = response.Profissional.DataCriacao, JobQuantidade = jobs, UsuarioId = user.ID };

            var docs = GetDocumentos(ret.Id);

            IList<DocumentoViewModel> models = new List<DocumentoViewModel>();

            foreach (var item in docs)
            {
                models.Add(new DocumentoViewModel(item.ID, item.DocumentoTipo.Nome, item.Tipo,
                    item.DocumentoStatusID, item.DocumentoStatus.Nome, item.DataCriacao.ToString(), item.Arquivo, item.Numero)
                { Observacoes = item.StatusObservacoes?.Observacoes });
            }

            ret.Documentos = models;
            ret.Avatar = user.Avatar;

            var statuses = GetAllDocumentoStatus();
            ViewBag.Statuses = statuses;

            var dados = GetDadosBancarios(response.Profissional.ID);
            ret.DadosBancarios = dados == null ? new DadosBancarios() : dados;

            return View(ret);
        }

        private IEnumerable<Documento> GetDocumentos(int profissionalId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpDocumento/BuscarPorCodigo/" + usuario.idCliente + "/" + usuario.IdUsuario;

            object envio = new
            {
                codigoExterno = profissionalId,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<IEnumerable<Documento>>(url, envio);

            return response;
        }

        private Usuario GetUsuario(int usuarioId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/BuscarUsuarioPorId/" + usuario.idCliente + "/" + usuario.IdUsuario;

            object envio = new
            {
                idUsuario = usuarioId,
            };

            var helper = new ServiceHelper();
            var response = helper.Post<Usuario>(url, envio);

            return response;
        }

        private IEnumerable<Usuario> GetUsuarios(IEnumerable<int> ids)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/BuscarUsuarios/" + PixCoreValues.UsuarioLogado.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                PixCoreValues.UsuarioLogado.idCliente,
                ids,
            };

            var helper = new ServiceHelper();
            var usuarios = helper.Post<IEnumerable<Usuario>>(url, envio);

            return usuarios;
        }

        [HttpPost]
        public string Alterar(ProfissionalViewModel profissional)
        {
            var result = string.Empty;
            if(profissional.Id != 0) //Necessário ID
            {
                var usuario = GetUsuario(profissional.UsuarioId);
                usuario.Status = (int)UserStatus.Ativo;

                var statuses = GetAllDocumentoStatus();
                var documentos = GetDocumentos(profissional.Id);

                foreach (var item in documentos)
                {
                    var doc = profissional.Documentos.FirstOrDefault(d => d.Id.Equals(item.ID));

                    if (doc != null)
                    {
                        var pDocStatus = doc.Status;
                        item.DocumentoStatusID = statuses.FirstOrDefault(s => s.Nome.Equals(pDocStatus)).ID;

                        item.StatusObservacoes = new DocStatusObservacoes(item.ID, doc.Observacoes);

                        if (item.DocumentoStatusID == 3) //Reprovado
                        {
                            usuario.Status = (int)UserStatus.Inativo;
                        }

                        if (item.DocumentoStatusID == 1) //Pendente
                        {
                            usuario.Status = (int)UserStatus.Inativo;
                        }
                    }
                }                
                
                result += PostUsuario(usuario);
                result += PostDocumentos(documentos);
            }

            return result;
        }

        private string PostDocumentos(IEnumerable<Documento> documentos)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpDocumento/AtualizarDocumentos/" + usuario.idCliente + "/" + usuario.IdUsuario;

            object envio = new
            {
                documentos,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            return (result as string);
        }

        private string PostUsuario(Usuario usuario)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/salvarUsuario/" + PixCoreValues.UsuarioLogado.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                usuario,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<Usuario>(url, envio);

            if(usuario.ID > 0)
            {
                return "Usuário salvo com sucesso. ";
            }

            return "Não foi possível atualizar o status do usuário. ";
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
            var result = helper.Post<object>(url, envio);

            return Convert.ToString(result);            
        }

        public IEnumerable<DocumentoStatus> GetAllDocumentoStatus()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpDocumento/BuscaTodosStatus/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var helper = new ServiceHelper();
            var statuses = helper.Get<IEnumerable<DocumentoStatus>>(url);

            return statuses;
        }

        public IEnumerable<Servico> GetServico(int profissionalId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarServico/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                idProfissional = profissionalId,
            };

            var helper = new ServiceHelper();
            var servico = helper.Post<IEnumerable<Servico>>(url, envio);

            return servico;
        }

        private int GetJobQuantidade(int iD)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpCheckIn/GetQuantidadeJobs/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                usuario.idCliente,
                profissionalId = iD,
            };

            var helper = new ServiceHelper();
            var r = helper.Post<object>(url, envio);

            return Convert.ToInt32(r);
        }

        public DadosBancarios GetDadosBancarios(int idProfissional)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/BuscarDadosBancariosPorUsuario/" + usuario.idCliente + "/" + usuario.IdUsuario;
                
            var envio = new
            {
                usuario.idCliente,
                codigoExterno = idProfissional,
            };

            var helper = new ServiceHelper();
            var dados = helper.Post<DadosBancarios>(url, envio);

            return dados;
        }

        [HttpGet]
        public ActionResult BuscarServicoTipo()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarServicoTipo/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var helper = new ServiceHelper();
            var result = helper.Get<object>(url);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BuscarServico()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarServico/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var helper = new ServiceHelper();
            var result = helper.Get<object>(url);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}