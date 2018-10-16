popularTabela();


function popularTabela() {
    LoadingInit('.content');
    //var Url = "GetProfissionais";
    var Url = "Profissionais/GetProfissionais";
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    }

    $.ajax(settings).done(function (response) {
        for (var i = 0; i < response.length; i++) {
            adicionarLinhaTabela(response[i]);
        }
        LoadingStop('.content');
    });

}

function adicionarLinhaTabela(response) {
    var newRow = $("<tr>"); var cols = "";
    cols += '<td>' + response.Id + '</td>';
    cols += '<td>' + response.Nome + '</td>';
    cols += '<td>' + response.Especialidade + '</td>';
    cols += '<td>' + response.Email + '</td>';
    cols += '<td>' + response.Telefone + '</td>';
    cols += '<td>';
    cols += '<a href="Profissionais/Detalhes/' + response.Id + '" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
    cols += '<i class="nc-icon nc-credit-card" ></i >';
    cols += '</td>';
    newRow.append(cols); $("#tbEmpresas").append(newRow);
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
