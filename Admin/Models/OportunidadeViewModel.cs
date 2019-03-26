﻿using System;

namespace Admin.Models
{
    public class OportunidadeViewModel:Base
    {
      
        public int TipoProfissional { get; set; }
        public int TipoServico { get; set; }
        public string DescProfissional { get; set; }
        public string DescServico { get; set; }
        public DateTime DataOportunidade { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public Endereco Endereco { get; set; }
        public int OportunidadeStatusID { get; set; }
        public int IdEmpresa { get; set; }
        public string EmailEmpresa { get; set; }
    }

    public class Endereco : Base
    {
        public Endereco(int id, string cEP, string estado, string cidade, string bairro, string local, int numeroLocal, string complemento, int idUsuario)
        {
            ID = id;
            CEP = cEP;
            Estado = estado;
            Cidade = cidade;
            Bairro = bairro;
            Local = local;
            NumeroLocal = numeroLocal.ToString();
            Complemento = complemento;
            IdUsuario = idUsuario;
        }

        public Endereco()
        {

        }

        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Local { get; set; }
        public string NumeroLocal { get; set; }
        public string Complemento { get; set; }
        public int IdUsuario { get; set; }
        public int OportunidadeId { get; set; }
        public int ProfissionalId { get; set; }
        public string Uf { get; set; }
        public string LocalOportunidade { get; set; }

    }

    public class Base
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataEdicao { get; set; }
        public int UsuarioCriacao { get; set; }
        public int UsuarioEdicao { get; set; }
        public bool Ativo { get; set; }
        public int Status { get; set; }
        public int IdCliente { get; set; }
    }
}