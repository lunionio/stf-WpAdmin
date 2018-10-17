popularTabela();

$('#acao').on('click', function () {
    LoadingInit('body');

});

function popularTabela() {
    LoadingInit('.content');
    var Url = "GetAtendimentos";
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    }

    $.ajax(settings).done(function (response) {
        for (var i = 0; i < response.length; i++) {
            adicionarLinhaTabela(response[i]);
            adicionarLinhaTabelaFechada(response[i]);
        }
        LoadingStop('.content');
    });

}

function adicionarLinhaTabela(response) {
    if (response.status == 1) {
        var newRow = $("<tr>"); var cols = "";
        cols += '<td>' + response.numero + '</td>';
        cols += '<td>' + response.email + '</td>';
        cols += '<td>' + response.tipo.descricao + '</td>';
        cols += '<td>' + response.dataCriacao + '</td>';
        cols += '<td>' + response.origem + '</td>';
        cols += '<td>';
        cols += '<a href="Editar/' + response.id + '" id="acao" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
        cols += '<i class="nc-icon nc-credit-card" ></i >';
        cols += '</td>';
        newRow.append(cols); $("#tbAtendimento").append(newRow);
    }
}


function adicionarLinhaTabelaFechada(response) {
    if (response.status == 2) {
        var newRow = $("<tr>"); var cols = "";
        cols += '<td>' + response.numero + '</td>';
        cols += '<td>' + response.email + '</td>';
        cols += '<td>' + response.tipo.descricao + '</td>';
        cols += '<td>' + response.dataCriacao + '</td>';
        cols += '<td>' + response.origem + '</td>';
        cols += '<td>';
        cols += '<a href="Editar/' + response.id + '" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
        cols += '<i class="nc-icon nc-credit-card" ></i >';
        cols += '</td>';
        newRow.append(cols); $("#tbAtendimentoFechado").append(newRow);
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

function LoadingStop(elemento) {
    $(elemento).loading('stop');
}
