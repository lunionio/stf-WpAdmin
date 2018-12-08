$(document).ready(function () {
    var status = $('#statusId').val();
    if (status == 3) { //Fechado
        $('#respostaTicket').prop('disabled', true);
        $('#btnEncerrar').prop('disabled', true);
    }
});

$('#btnEncerrar').on('click', function () {
    encerrar();
});

function encerrar() {
    let data = {
        Id: $('#atdId').val(),
        Email: $('#email').val(),
        Categoria: $('#categoria').val(),
        Origem: $('#origem').val(),
        Numero: $('#numero').val(),
        Descricao: $('#mensagem').text(),
        Resposta: $('#respostaTicket').val(),
        TicketId: $('#tId').val()
    };

    console.log(data);

    let settings = {
        "async": true,
        "crossDomain": true,
        "url": "/Atendimento/ResponderTicket",
        "method": "POST",
        "data": data
    };

    $.ajax(settings).done(function (response) {
        if (response == "ok") {
            window.location = "/Atendimento/Index";
        }
        else {
            alert(response);
        }
    });
}