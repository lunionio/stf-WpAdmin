using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class SharedController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetMenus()
        {

            var perfil = GetPerfil(PixCoreValues.UsuarioLogado.idPerfil);
            var permissoes = GetPermissoes(perfil.idPermissao.Split(',').Select(id => Convert.ToInt32(id)));
            var model = GetEstruturas(1, permissoes);

            return PartialView("PartialMenu", model);
        }

        private IEnumerable<Estrutura> GetEstruturas(int tipo, IEnumerable<Permissao> permissoes)
        {
            var url = ConfigurationManager.AppSettings["UrlAPI"];
            var serverUrl = $"{ url }/Seguranca/Principal/BuscaEstruturasPorMotor/{PixCoreValues.UsuarioLogado.idCliente}/{ PixCoreValues.UsuarioLogado.IdUsuario }";

            IDictionary<string, string> valuePairs = new Dictionary<string, string>();

            foreach (var item in permissoes)
            {
                valuePairs.Add(item.IdAux.ToString(), item.idTipoAcao);
            }

            var envio = new
            {
                tipo,
                PixCoreValues.UsuarioLogado.idCliente,
                valuePairs,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<IEnumerable<EstruturaViewModel>>(serverUrl, envio).OrderBy(e => e.Ordem);

            var model = default(IList<Estrutura>);

            if (result != null && result.Count() > 0)
            {
                model = new List<Estrutura>();
                foreach (var r in result)
                {
                    r.SubMenus = result.Where(e => e.IdPai.Equals(r.ID)).OrderBy(x => x.Ordem);

                    if (r.IdPai == 0) //TODO: Melhorar para sub-estrutura com filhos...
                    {
                        var estrutura = new Estrutura(r.ID, r.UrlManual, r.Imagem, r.Nome)
                        {
                            SubEstruturas = r.SubMenus.Select(s => new Estrutura(s.ID, s.UrlManual, s.Imagem, s.Nome))
                        };

                        model.Add(estrutura);
                    }
                }
            }

            return model;
        }

        private Perfil GetPerfil(int id)
        {
            var url = ConfigurationManager.AppSettings["UrlAPI"];
            var serverUrl = $"{ url }/Perfil/GetPerfilByID/{ id }";

            var helper = new ServiceHelper();
            var result = helper.Get<Perfil>(serverUrl);

            return result;
        }

        private IEnumerable<Permissao> GetPermissoes(IEnumerable<int> ids)
        {
            var url = ConfigurationManager.AppSettings["UrlAPI"];
            var serverUrl = $"{ url }/Permissao/BuscaPermissoesPorIds/";

            var helper = new ServiceHelper();
            var result = helper.Post<IEnumerable<Permissao>>(serverUrl, ids);

            return result;
        }
    }
}