﻿let documentos = [];

function Loading(elemento) {
    $(elemento).loading({
        theme: 'dark',
        message: 'Aguarde...'
    });
}

function LoadingStop(elemento) {
    $(elemento).loading('stop');
}

function guardaOpcao(dId) {
    documentos.forEach(function (item, index, array) {
        if (item.Id === dId) {
            array.splice(index, 1);
        }
    });

    let documento = {
        Id: dId,
        Status: $('#' + dId + ' option:selected').text(),
    };

    documentos.push(documento);
}

$('#btnAtualizar').on('click', function () {
    atualizar();
});

function atualizar() {
    Loading('body');
    documentos.forEach(function (item, index, array) {
        item.Observacoes = $('#observacoes').val();
    });

    let data = {
        Id: $('#mId').val(),
        UsuarioId: $('#uId').val(),
        Documentos: documentos
    };

    let settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Profissionais/Alterar",
        "method": "POST",
        "data": data
    };

    console.log(data);

    $.ajax(settings).done(function (response) {
        if (response == "ok") {
            window.location = "/Index";
        }
        else {
            alert(response);
        }

        LoadingStop('body');
    });
}

