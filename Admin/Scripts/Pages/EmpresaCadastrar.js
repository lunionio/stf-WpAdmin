
init();
controlarPaineis();
aplicarMascaras();

$('#cadastrar').on('click', function () {
    if (validarCampos() == true) {
        Save();
    }
});


function PublicarAgora() {

    LoadingInitBase('.body');
    var vaga = VagaViewModel();
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "PublicarAgora",
        "method": "POST",
        "data": vaga
    }

    $.ajax(settings).done(function (response) {
        if (response == "ok") {

            window.location = "/Vaga";
        }
        else {
            alert(response);
        }
    });
}

function PublicarDepois() {


}

function getFormData() {
    return {
        nome: $('#nome').val(),
        cep: $('#cep').val(),
        rua: $('#rua').val(),
        bairro: $('#bairro').val(),
        numero: $('#numero').val(),
        cidade: $('#cidade').val(),
        uf: $('#uf').val(),
        data: $('#data').val(),
        razaoSocial: $('#razaoSocial').val(),
        cnpj: $('#cnpj').val(),
        cnae: $('#cnae').val(),
        Qtd: $('#qtd').val(),
        Total: $('#total').val()
    }
}

function aplicarMascaras() {
    $('#data').mask('00/00/0000');
    $('#numero').mask('9999');
    $('#cep').mask('99999-999');
}

function EmpresaViewModel() {
    var VagaViewModel = {
        Nome: $('#nome').val(),
        Cep: $('#cep').val(),
        Rua: $('#rua').val(),
        Bairro: $('#bairro').val(),
        Numero: $('#numero').val(),
        Cidade: $('#cidade').val(),
        Complemento: $('#complemento').val(),
        Referencia: $('#referencia').val(),
        Uf: $('#uf').val(),
        RazaoSocial: $('#razaoSocial').val(),
        Cnpj: $('#cnpj').val(),
        Cnae: $('#cnae').val()
    }
    return VagaViewModel;
}

function getCep() {
    var cep = $('#cep').val();
    cep = cep.replace("-", "");
    return int.parse(cep);
}

function getForm() {
    return {
        nome: $('#nome'),
        cep: $('#cep'),
        rua: $('#rua'),
        bairro: $('#bairro'),
        cidade: $('#cidade'),
        complemento: $('#complemento'),
        uf: $('#uf'),
        razaoSocial: $('#razaoSocial'),
        cnpj: $('#cnpj'),
        cnae: $('#cnae'),
        referencia: $('#referencia')
    }
}

function LoadingInit(elemento) {
    $(elemento).loading({
        theme: 'dark',
        message: 'Aguarde...',
        onStart: function (loading) {
            loading.overlay.slideDown(400);
        },
        onStop: function (loading) {
            loading.overlay.slideUp(400);
        }
    });
}

function LoadingInitBase(elemento) {
    $(elemento).loading({
        theme: 'dark',
        message: 'Aguarde...'
    });
}

function LoadingStop() {
    $('.card-body').loading('stop');
}

function LoadingBodyStop() {
    $('body').loading('stop');
}

function init() {
    $('.selectpicker').selectpicker();
    $("#miolo").addClass("card-wizard");
}

function limpaEndereco() {
    $("#cep").val("");
    $('#rua').val("");
    $('#bairro').val("");
    $('#cidade').val("");
    $('#uf').val("");
}

function desbloqueiaEndereco() {
    $("#rua").prop("disabled", false);
    $("#bairro").prop("disabled", false);
    $("#cidade").prop("disabled", false);
    $("#uf").prop("disabled", false);
    $("#numero").focus();

}

function bloqueiaEndereco() {
    $("#rua").prop("disabled", true);
    $("#bairro").prop("disabled", true);
    $("#cidade").prop("disabled", true);
    $("#uf").prop("disabled", true);
    $("#numero").focus();
    $('.card-body').loading('stop');
}

function preencherEndereco(cep) {
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "http://seguranca.mundowebpix.com.br:5300/api/seguranca/endereco/BuscarEnderecoPorCep/2/999",
        "method": "POST",
        "headers": {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache",
            "Postman-Token": "fa43c255-9862-4a93-a554-47cb98066396"
        },
        "processData": false,
        "data": "{\"cep\": \"" + cep + "\"}"
    }

    LoadingInit('.card-body');
    $.ajax(settings).done(function (response) {
        if (response.endereco == '' || response.endereco == null) {
            demo.showNotification('top', 'right', 'Por favor digite um CEP válido!');
            limpaEndereco();
            desbloqueiaEndereco();
            $("#cep").focus();
            LoadingStop();
        }
        else {
            $('#rua').val(response.endereco);
            $('#bairro').val(response.bairro);
            $('#cidade').val(response.cidade);
            $('#uf').val(response.uf);

            bloqueiaEndereco();
            LoadingStop();
        }

    });


}

function controlarPaineis() {

    getForm().complemento.on("change", function () {
        $("#referencia").focus();
    });

    getForm().razaoSocial.on("change", function () {
        $("#cnpj").focus();
    });
    
    getForm().cnpj.on("change", function () {
        $("#cnae").focus();
    });

    getForm().cnae.on("change", function () {
        $("#nome").focus();
    });

    getForm().nome.on("change", function () {
        $("#cep").focus();
    });

    getForm().cep.on("change", function () {
        preencherEndereco($(this).val());
    });

}

function validarCampos() {
    var form = getFormData();

    if (form.nome == "" || form.nome == null) {
        demo.showNotification('top', 'right', 'Campo nome é obrigatório!');
        $('#nome').focus();
        return false;
    }
    else if (form.cep == "" || form.rua == "" || form.cidade == "" || form.uf == "" || form.bairro == "") {
        demo.showNotification('top', 'right', 'Informe um endereço válido!');
        $('#cep').focus();
        return false;
    }
    else if (form.numero == "" || form.numero == null) {
        demo.showNotification('top', 'right', 'Digite um numero para o endereço!');
        $('#numero').focus();

        return null;
    }
    else
        return true;
}

function Save() {

    LoadingInit('body');
    var empresaViewModel = EmpresaViewModel();
    var settings = {
        "url": "Salvar",
        "method": "POST",
        "data": empresaViewModel
    }

    $.ajax(settings).done(function (response) {
        //$('#modal').html(response);
        //$('#myModal').modal('show');
        //LoadingBodyStop();
    });

    return true;
}
