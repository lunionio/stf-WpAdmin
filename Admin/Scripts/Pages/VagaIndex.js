let pagamentosParaLiberar = [];

$(document).ready(function () {
    $(".chosen-select").chosen({ no_results_text: "Nada encontrado!" });
    $(".chosen-select").chosen({ allow_single_deselect: true });

    $('#empresas').on('change', function (e) {
        LoadPanels(this.value);
    });
});

$("#linhaPadrao").remove();
$('.carousel').carousel();

function LoadPanels(idEmpresa) {
    Loading('body');
    getOportunidades(idEmpresa);
}

function Loading(elemento) {
    $(elemento).loading({
        theme: 'dark',
        message: 'Aguarde...'
    });
}

function LoadingStop(elemento) {
    $(elemento).loading('stop');
}

function getOportunidades(idEmpresa) {

    var Url = "/Vaga/_listarOportunidades?idEmpresa=" + idEmpresa;
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    };

    $.ajax(settings).done(function (response) {
        LoadingStop('body');
        $('#oportunidades').hide();
        $('#oportunidades').html(response);
        $('#oportunidades').fadeIn();

        $('#tbOportunidades').dataTable({
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
    });
}

function getProfissionais() {
    var Url = "vaga/_lsitarProfissionais";
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    }

    $.ajax(settings).done(function (response) {
        $('#profissionais').hide();
        $('#profissionais').html(response);
        $('#profissionais').fadeIn();
    });
}

function getVinculoProfissional() {
    var Url = "vaga/_vincularPorifissionais";
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    }

    $.ajax(settings).done(function (response) {
        $('#vinculoProfissionais').hide();
        $('#vinculoProfissionais').html(response);
        $('#vinculoProfissionais').fadeIn();

    });
}

function getModalMatch(idOpt) {
    Loading('body');
    var Url = "vaga/ModalMatch?optId=" + idOpt;
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    };

    $.ajax(settings).done(function (response) {
        LoadingStop('body');

        $('#modal').html(response);
        $('#myModal').modal('show');

        $('#tbContratar').dataTable({
            "pagingType": "numbers",
            "columnDefs": [{
                "targets": "_all",
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
                "sZeroRecords": "Nenhuma candidatura registrada.",
                "sInfo": "Mostrando oágina _PAGE_ de _PAGES_",
                "sInfoEmpty": "Nenhum dado para mostrar",
                "sInfoFiltered": "(Filtrado de _MAX_ registros)",
                "sSearch": "Pesquisar:",
            },
        });

        $('#tbContratados').dataTable({
            "pagingType": "numbers",
            "columnDefs": [{
                "targets": "_all",
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
                "sZeroRecords": "Nenhum profissional contratado.",
                "sInfo": "Mostrando oágina _PAGE_ de _PAGES_",
                "sInfoEmpty": "Nenhum dado para mostrar",
                "sInfoFiltered": "(Filtrado de _MAX_ registros)",
                "sSearch": "Pesquisar:",
            },
        });

        $('#tbCheckIn').dataTable({
            "pagingType": "numbers",
            "columnDefs": [{
                "targets": "_all",
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
                "sZeroRecords": "Nenhum check-in registrado.",
                "sInfo": "Mostrando oágina _PAGE_ de _PAGES_",
                "sInfoEmpty": "Nenhum dado para mostrar",
                "sInfoFiltered": "(Filtrado de _MAX_ registros)",
                "sSearch": "Pesquisar:",
            },
        });

        $('.stars').barrating({
            theme: 'fontawesome-stars-o',
            readonly: true
        });
    });
}

function aprovarProfissional(userXOpt, optId, userId) {
    Loading('body');
    var obj = {
        ID: userXOpt,
        UserId: userId,
        OportunidadeId: optId,
        StatusId: 1, //Aprovado
        IdEmpresa: $("#idEmpresa").val(),
    };

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Vaga/Match",
        "method": "POST",
        "contentType": "application/json",
        "data": JSON.stringify(obj)
    };

    console.log(obj);

    $.ajax(settings).done(function (response) {
        try {
            var p = $.parseJSON(response);
            console.log(p);

            if (typeof p == 'object') {
                if (p.Id == undefined) {
                    swal(response, "", "success");
                }
                else {
                    let table = $('#tbContratar').DataTable();
                    table.row("#" + userId).remove().draw();
                    var contratados = $('#tbContratados').DataTable();
                    var row = contratados.row.add([
                        p.UsuarioId,
                        p.Nome,
                        p.Especialidade,
                        p.Endereco.Local,
                        p.Valor,
                        '<select class="stars" id="' + p.Avaliacao + '" disabled>' +
                        '<option value = "' + Math.floor(p.Avaliacao) + '" >' + p.Avaliacao + '</option > ' +
                        '</select>'
                    ]).draw(false);
                }
            }
            else {
                alert(response);
            }
        }
        catch (e) {
            alert(response);
        }
        LoadingStop('body');
    });
}

function reprovarProfissional(userXOpt, optId, userId) {
    Loading('body');

    var obj = {
        ID: userXOpt,
        UserId: userId,
        OportunidadeId: optId,
        StatusId: 3, //Reprovado
        IdEmpresa: $("#idEmpresa").val(),
    };

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Vaga/Match",
        "method": "POST",
        "data": obj
    };

    $.ajax(settings).done(function (response) {

        var p = $.parseJSON(response);

        if (typeof p == 'object') {
            if (p.Id == undefined) {
                alert(response);
            }
            else {
                let table = $('#tbContratar').DataTable();
                table.row("#" + userId).remove().draw();
            }
        }
        else {
            alert(response);
        }

        LoadingStop('body');
    });
}

function getProfissional(id) {
    Loading('body');

    var Url = "/Vaga/ModalProfissional?pId=" + id;
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    };

    $.ajax(settings).done(function (response) {
        $('#modalProfissional').html(response);
        $('#profissionalModal').modal('show');

        LoadingStop('body');
    });
}

function guardaCache(optId, profissionalId) {
    pagamentosParaLiberar.forEach(function (item, index, array) {
        if (item.IdUsuario === profissionalId) {
            array.splice(index, 1);
        }
    });

    let pagamento = {
        IdUsuario: profissionalId,
        OportunidadeId: optId,
        Status: $('#' + optId + ' option:selected').text(),
        StatusPagamento: $('#' + optId + ' option:selected').val()
    };

    pagamentosParaLiberar.push(pagamento);
}

function liberarPagamentos() {
    Loading('body');

    let settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Vaga/LiberarPagamentos",
        "method": "POST",
        "data": { models: pagamentosParaLiberar }
    };

    $.ajax(settings).done(function (response) {

        LoadingStop('body');
        alert(response);
    });
}

function getModalCheckIn(profissionalId) {
    Loading('body');

    var Url = "/Vaga/ModalCheckIn?pId=" + profissionalId;
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": Url,
        "method": "GET"
    };

    $.ajax(settings).done(function (response) {
        $('#modalCheckIn').html(response);
        $('#checkinModal').modal('show');

        LoadingStop('body');
    });
}