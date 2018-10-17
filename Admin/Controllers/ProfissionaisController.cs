﻿using Admin.Helppser;
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
            var profissionais = jss.Deserialize<IEnumerable<Profissional>>(result);

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
            var jss = new JavaScriptSerializer();

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/BuscarPorId/" + usuario.idCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                idProfissional = id,
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

            var result = string.Empty;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                if (string.IsNullOrEmpty(result)
                    || "null".Equals(result.ToLower()))
                {
                    throw new Exception("Ouve um erro durante o processo.");
                }
            }

            var response = jss.Deserialize<Profissional>(result);

            //var ret = new ProfissionalViewModel(response.ID, response.Nome, string.Empty, response.Telefone.Numero, null, response.Email, response.Endereco) { Documentos = new List<DocumentoViewModel>() };
            return View();
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

                p.DataCriacao =profissional.DataCriacao;

                var result = PostProfissional(p);
            }

            return RedirectToAction("index");
        }

        public string PostProfissional(Profissional profissional)
        {
            var jss = new JavaScriptSerializer();
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/wpProfissionais/SalvarProfissional/" + profissional.IdCliente + "/" + PixCoreValues.UsuarioLogado.IdUsuario;

            object envio = new
            {
                profissional,
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

            var result = string.Empty;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                if (string.IsNullOrEmpty(result)
                    || "null".Equals(result.ToLower()))
                {
                    throw new Exception("Ouve um erro durante o processo.");
                }
            }

            return result;
        }
    }
}