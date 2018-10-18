﻿let documentos = [];

function guardaOpcao(dId) {
    let r = false;
    documentos.forEach(function (item, index) {
        if (item.id === dId) {
            r = true;
        }
    });

    if (!r) {
        let documento = {
            Id: dId,
            Status: $('#' + dId + ' option:selected').text(),
        };
        documentos.push(documento);
    }
}

$('#btnAtualizar').on('click', function () {
    atualizar();
});

function atualizar() {
    console.log('k');

    var data = {
        Id: $('#mId').val(),
        UsuarioId: $('#uId').val(),
        Documentos: documentos
    };

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Profissionais/Alterar",
        "method": "POST",
        "data": data
    };

    $.ajax(settings).done(function (response) {
        if (response == "ok") {
            window.location = "/Index";
        }
        else {
            alert(response);
        }
    });
}

