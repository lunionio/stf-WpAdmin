﻿using Admin.Helppers;
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

        public ActionResult GetProfissionais() //OK
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarProfissionais/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var profissionais = helper.Get<IEnumerable<Profissional>>(url);

            var usuarios = GetUsuarios(usuario.idCliente);

            IList<ProfissionalViewModel> ret = new List<ProfissionalViewModel>();

            foreach (var p in profissionais)
            {
                var user = usuarios.FirstOrDefault(u => u.ID.Equals(p.IdUsuario));

                //var servicos = GetServico(p.ID);

                ret.Add(new ProfissionalViewModel(p.ID, user.Nome, p.Nome, p.Telefone.Numero,
                    p.Telefone.ID, p.DataNascimento.ToString(), p.Email, p.IdUsuario, p.Endereco));
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
            var user = GetUsuario(response.IdUsuario);

            //var servicos = GetServico(id);

            var ret = new ProfissionalViewModel(response.ID, user.Nome, response.Nome, response.Telefone.Numero, response.Telefone.ID, 
                response.DataNascimento.ToString(), response.Email, response.IdUsuario, response.Endereco){ DataCriacao = response.DataCriacao };

            var docs = GetDocumentos(ret.Id);


            IList<DocumentoViewModel> models = new List<DocumentoViewModel>();

            foreach (var item in docs)
            {
                models.Add(new DocumentoViewModel(item.ID, item.DocumentoTipo.Nome, item.Tipo,
                    item.DocumentoStatusID, item.DocumentoStatus.Nome, item.DataCriacao.ToString(), item.Arquivo));
            }

            ret.Documentos = models;
            ret.Avatar = user.Avatar;

            var statuses = GetAllDocumentoStatus();
            ViewBag.Statuses = statuses;

            return View(ret);
        }

        private IEnumerable<Documento> GetDocumentos(int profissionalId)
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

            return response;
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

        private IEnumerable<Usuario> GetUsuarios(int idCliente)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/buscarUsuario/" + idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var usuarios = helper.Get<IEnumerable<Usuario>>(url);

            return usuarios;
        }

        [HttpPost]
        public string Alterar(ProfissionalViewModel profissional)
        {
            var result = string.Empty;
            if(profissional.Id != 0) //Necessário ID
            {
                var usuario = GetUsuario(profissional.UsuarioId);
                usuario.Status = (int)Status.Ativo;

                var statuses = GetAllDocumentoStatus();
                var documentos = GetDocumentos(profissional.Id);

                foreach (var item in documentos)
                {
                    var pDocStatus = profissional.Documentos.FirstOrDefault(d => d.Id.Equals(item.ID)).Status;
                    item.DocumentoStatusID = statuses.FirstOrDefault(s => s.Nome.Equals(pDocStatus)).ID;

                    if (item.DocumentoStatusID == 3) //Reprovado
                    {
                        usuario.Status = (int)Status.Inativo;
                    }

                    if (item.DocumentoStatusID == 1) //Pendente
                    {
                        usuario.Status = (int)Status.Inativo;
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
            var result = helper.Post<string>(url, envio);

            return result;
        }

        private string PostUsuario(Usuario usuario)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/Principal/salvarUsuario/" + usuario.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                usuario,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<string>(url, envio);

            return result;
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

        public IEnumerable<Servico> GetServico(int profissionalId)
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarServico/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            var envio = new
            {
                idProfissional = profissionalId,
            };

            var helper = new ServiceHelper();
            var servico = helper.Post<IEnumerable<Servico>>(url, envio);

            return servico;
        }
    }
}