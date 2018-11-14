using Admin.Models;
using Admin.Models.Financeiro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Status = Admin.Models.Financeiro.Status;

namespace Admin.Helppers
{
    public static class FinanceiroHelper
    {
        public static void InserirSaldo(decimal valor, string origem, string destino, int natureza, int tipo, string descricao, LoginViewModel usuario)
        {
            IList<Extrato> extratos = new List<Extrato>();

            var extrato = new Extrato(valor, natureza, tipo, origem, destino, null, Status.Aprovado)
            {
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                DataEdicao = DateTime.UtcNow,
                Descricao = descricao,
                IdCliente = usuario.idCliente,
                Nome = "Saldo",
                Status = 1,
                UsuarioCriacao = usuario.IdUsuario,
                UsuarioEdicao = usuario.IdUsuario,
                TipoOrigem = 2,
                TipoDestino = 3,
            };

            extratos.Add(extrato);

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/InserirCredito/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                extratos,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);
        }

        public static bool VerifcaSaldoCliente(decimal valorVaga, int idCliente, int idEmpresa, int idUsuario)
        {
            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/BuscarSaldo/" + idCliente + "/" + idUsuario;

            var envio = new
            {
                idCliente,
                idEmpresa
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);

            var saldo = Convert.ToDecimal(result);

            return saldo >= valorVaga;
        }

        public static void LancaTransacoes(OportunidadeViewModel vaga, LoginViewModel usuario)
        {
            IList<Extrato> extratos = new List<Extrato>();
            for (int i = 0; i < vaga.Quantidade; i++)
            {
                var valor1 = (vaga.Valor) * -1;

                var extrato1 = new Extrato(valor1, 2, 2, usuario.idEmpresa.ToString(),
                    usuario.idEmpresa.ToString(), vaga.ID, Status.Aprovado)
                {
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow,
                    DataEdicao = DateTime.UtcNow,
                    Descricao = "Debitando valor da vaga.",
                    IdCliente = usuario.idCliente,
                    Nome = "Débito",
                    Status = 1,
                    UsuarioCriacao = usuario.IdUsuario,
                    UsuarioEdicao = usuario.IdUsuario,
                    TipoOrigem = 3,
                    TipoDestino = 3,
                };

                var valor2 = vaga.Valor;

                var extrato2 = new Extrato(valor2, 2, 1, usuario.idEmpresa.ToString(),
                    "16", vaga.ID, Status.Bloqueado)
                {
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow,
                    DataEdicao = DateTime.UtcNow,
                    Descricao = "Disponibilizando valor da vaga.",
                    IdCliente = usuario.idCliente,
                    Nome = "Pagamento",
                    Status = 1,
                    UsuarioCriacao = usuario.IdUsuario,
                    UsuarioEdicao = usuario.IdUsuario,
                    TipoOrigem = 3,
                    TipoDestino = 2,
                };

                extratos.Add(extrato1);
                extratos.Add(extrato2);
            }

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/AlocarCredito/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                extratos,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);
        }

        public static void LancaTransacoes(decimal valor, string origem, int tipoOrigem, string destino, int tipoDestino, int natureza, int tipo, string descricao, LoginViewModel usuario)

        {
            IList<Extrato> extratos = new List<Extrato>();

            var extrato = new Extrato(valor, natureza, tipo, origem, destino, null, Status.Aprovado)
            {
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                DataEdicao = DateTime.UtcNow,
                Descricao = descricao,
                IdCliente = usuario.idCliente,
                Nome = descricao,
                Status = 1,
                UsuarioCriacao = usuario.IdUsuario,
                UsuarioEdicao = usuario.IdUsuario,
                TipoOrigem = tipoOrigem,
                TipoDestino = tipoDestino,
            };

            extratos.Add(extrato);

            var keyUrl = ConfigurationManager.AppSettings["UrlAPI"].ToString();
            var url = keyUrl + "/Seguranca/WpFinanceiro/AlocarCredito/" + usuario.idCliente + "/" + usuario.IdUsuario;

            var envio = new
            {
                extratos,
            };

            var helper = new ServiceHelper();
            var result = helper.Post<object>(url, envio);
        }

    }
}