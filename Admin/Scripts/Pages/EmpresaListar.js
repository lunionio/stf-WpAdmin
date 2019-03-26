popularTabela();


function popularTabela() {
    LoadingInit('body');
    var Url = "GetEmpresas";
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    }

    $.ajax(settings).done(function (result) {

        for (var i = 0; i < result.length; i++) {
            console.log(result[i]);
            var response = result[i];

            var newRow = $("<tr>"); var cols = "";
            cols += '<td>' + response.CNPJ + '</td>';
            cols += '<td>' + response.RazaoSocial + '</td>';
            cols += '<td>' + response.Email + '</td>';
            cols += '<td>' + response.Telefone.Numero + '</td>';
            cols += '<td>';
            cols += '<a href="Editar/' + response.ID + '" class="btn btnAcao btn-success btn-link btn-xs" data-original-title="Editar">';
            cols += '<i class="nc-icon nc-credit-card" ></i >';

            cols += '<a href="Excluir/' + response.ID + '" rel="tooltip" title="Excluir" class="btn btnAcao btn-danger btn-link btn-xs" data-original-title="Remover">';
            cols += '<i class="nc-icon nc-simple-remove icon-bold" ></i></a>';
            cols += '</td>';
            newRow.append(cols); $("#tbEmpresas").append(newRow);
        }

        $("#tbEmpresas").DataTable({
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
