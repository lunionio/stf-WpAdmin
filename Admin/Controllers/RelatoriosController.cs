using Admin.Helppers;
using Admin.Helppser;
using Admin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class RelatoriosController : Controller
    {
        // GET: Relatorios
        public ActionResult Index()
        {
            var relatorios = GetRelatorios();

            return View(relatorios);
        }

        private static IList<RelatorioModel> GetRelatorios()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpRelatorios/buscarRelatorios/" + usuario.idCliente + "/" +
                PixCoreValues.UsuarioLogado.IdUsuario;

            var envio = new
            {
                tipo = 1,
            };

            var helper = new ServiceHelper();
            var relatorios = helper.Post<IList<RelatorioModel>>(url, envio);
            return relatorios;
        }

        public ActionResult Gerar(int relatorioId)
        {
            if (relatorioId == 1)
            {
                var result = GetOptRelatorios();

                var json = JsonConvert.SerializeObject(result);
                var dt = (DataTable)JsonConvert.DeserializeObject(json, typeof(DataTable));

                var sb = new StringBuilder();
                var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));
                foreach (DataRow row in dt.Rows)
                {
                    var fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
                using (var ms = new MemoryStream(fileBytes))
                {
                    var bytes = ms.ToArray();
                    Response.Clear();
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", "attachment;    filename=Relatorio.csv");
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }

            var relatorios = GetRelatorios();
            return View("Index", relatorios);
        }

        private IEnumerable<RelatorioViewModel> GetOptRelatorios()
        {
            var usuario = PixCoreValues.UsuarioLogado;
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpRelatorios/OptsStaffPro/" + usuario.idCliente + "/" +
                PixCoreValues.UsuarioLogado.IdUsuario;

            var helper = new ServiceHelper();
            var result = helper.Get<IEnumerable<RelatorioViewModel>>(url);
            return result;
        }

        [HttpGet]
        public JsonResult Relatorios(int relatorioId)
        {
            if (relatorioId == 1)
            { 
                var result = Json(GetOptRelatorios().ToList(), JsonRequestBehavior.AllowGet);
                return result;
            }

            return Json("Nenhum relatório gerado.", JsonRequestBehavior.AllowGet);
        }
    }
}