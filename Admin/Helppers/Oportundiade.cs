﻿using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Helppser
{
    public static class Oportundiade
    {
        public static OportunidadeViewModel Convert(VagaViewModel vaga)
        {
            var endereco = new Endereco
            {
                IdCliente = PixCoreValues.UsuarioLogado.idCliente,
                IdUsuario = PixCoreValues.UsuarioLogado.IdUsuario,
                Local = vaga.Rua,
                NumeroLocal = vaga.Numero.ToString(),
                Ativo = true,
                Bairro = vaga.Bairro,
                Complemento = vaga.Complemento,
                Cidade = vaga.Cidade,
                Descricao = (vaga.Rua + ". " + vaga.Numero + ". " + vaga.Bairro + " - " + vaga.Cidade + " /" +
                vaga.Uf + " (" + vaga.Cep + ")"),
                Estado = vaga.Uf,
                CEP = vaga.Cep,
                ID = vaga.EnderecoId,
                Status = vaga.status,
                DataCriacao = System.Convert.ToDateTime(vaga.EnderecoDataCriacao),
                OportunidadeId = vaga.Id,
                UsuarioEdicao = PixCoreValues.UsuarioLogado.idCliente,
                Uf = vaga.Uf,
                LocalOportunidade = vaga.LocalOportunidade,
            };

            return new OportunidadeViewModel
            {
                ID = vaga.Id,
                Ativo = true,
                DataCriacao = System.Convert.ToDateTime(vaga.DataCriacao),
                DataOportunidade = vaga.DataEvento,
                DescProfissional = vaga.ProfissionalNome,
                Nome = vaga.Nome,
                Quantidade = vaga.Qtd,
                TipoProfissional = vaga.Profissional,
                UsuarioCriacao = PixCoreValues.UsuarioLogado.IdUsuario,
                Endereco = endereco,
                HoraInicio = vaga.Hora,
                HoraFim = vaga.Hora,
                OportunidadeStatusID = vaga.status,
                IdEmpresa = vaga.IdEmpresa,
                IdCliente = PixCoreValues.UsuarioLogado.idCliente,
                Valor = vaga.Valor,
                Status = vaga.status,
                UsuarioEdicao = PixCoreValues.UsuarioLogado.idCliente,
                DescServico = vaga.Servico
            };

        }
    }
}