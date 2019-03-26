popularTabela();

$('#acao').on('click', function () {
    LoadingInit('body');

});

function popularTabela() {
    LoadingInit('body');
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
        $("#tbAtendimentoFechado").DataTable({
            "pagingType": "numbers",
            "columnDefs": [{
                "targets": '_all',
                "orderable": true,
            }],
            "dom": '<"top"f>rt' + "<'bottom col-sm-12'" +
                "<'row'" +
                "<'col-sm-6'l>" +
                "<'col-sm-6'p>" +
                ">" +
                ">" + '<"clear">',
            "oLanguage": {
                "sLengthMenu": "_MENU_",
                "sZeroRecords": "Nada encontrado",
                "sInfo": "Mostrando oágina _PAGE_ de _PAGES_",
                "sInfoEmpty": "Nenhum dado para mostrar",
                "sInfoFiltered": "(Filtrado de _MAX_ registros)",
                "sSearch": "Pesquisar:",
            },
        });

        $("#tbAtendimento").DataTable({
            "pagingType": "numbers",
            "columnDefs": [{
                "targets": '_all',
                "orderable": true,
            }],
            "dom": '<"top"f>rt' + "<'bottom col-sm-12'" +
                "<'row'" +
                "<'col-sm-6'l>" +
                "<'col-sm-6'p>" +
                ">" +
                ">" + '<"clear">',
            "oLanguage": {
                "sLengthMenu": "_MENU_",
                "sZeroRecords": "Nada encontrado",
                "sInfo": "Mostrando oágina _PAGE_ de _PAGES_",
                "sInfoEmpty": "Nenhum dado para mostrar",
                "sInfoFiltered": "(Filtrado de _MAX_ registros)",
                "sSearch": "Pesquisar:",
            },
        });
        LoadingStop('body');
    });

}

function adicionarLinhaTabela(response) {
    if (response.TicketStatusID == 1) {
        var newRow = $("<tr>"); var cols = "";
        cols += '<td>' + response.Numero + '</td>';
        cols += '<td>' + response.Email + '</td>';
        cols += '<td>' + response.Tipo.Nome + '</td>';
        cols += '<td>' + response.Data + '</td>';
        cols += '<td>' + response.Origem + '</td>';
        cols += '<td>';
        cols += '<a href="Editar/' + response.ID + '" id="acao" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
        cols += '<i class="nc-icon nc-credit-card" ></i >';
        cols += '</td>';
        newRow.append(cols); $("#tbAtendimento").append(newRow);
    }
}

function adicionarLinhaTabelaFechada(response) {
    if (response.TicketStatusID == 3) {
        var newRow = $("<tr>"); var cols = "";
        cols += '<td>' + response.Numero + '</td>';
        cols += '<td>' + response.Email + '</td>';
        cols += '<td>' + response.Tipo.Nome + '</td>';
        cols += '<td>' + response.Data + '</td>';
        cols += '<td>' + response.Origem + '</td>';
        cols += '<td>';
        cols += '<a href="Editar/' + response.ID + '" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
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
