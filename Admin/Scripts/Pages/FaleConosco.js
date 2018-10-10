$("#miolo").addClass("card-wizard col-md-10");

controlChanges();
$('.sweet-confirm btn btn-info btn-fill').click(function () {
    window.location = "/home/index";
});

$('#enviar').click(function () {


    enviar();

});

function controlChanges() {

    $('#assunto').change(function () {
        $('#email').focus();

    })
    $('#email').change(function () {
        $('#mensagem').focus();

    });
    $('#mensagem').change(function () {
        $('#enviar').focus();
    })
}

function getForm() {
    return {
        assunto: $('#assunto'),
        email: $('#email'),
        mensagem: $('#mensagem')
    }
}

function getFormData() {
    return {
        assunto: $('#assunto option:selected').val(),
        assuntoNome: $('#assunto option:selected').text(),
        email: $('#email').val(),
        mensagem: $('#mensagem').val()
    }
}

function LoadingStop(elemento) {
    $(elemento).loading('stop');
}

function LoadingStart(elemento) {
    $(elemento).loading({
        theme: 'dark',
        message: 'Aguarde...'
    });
}


function enviar() {
    LoadingStart('body');

    var data = getFormData();
    var settings = {
        "url": "FaleConosco/Enviar",
        "method": "POST",
        "data": data
    }

    

    $.ajax(settings).done(function (response) {
        var mensagem = "#" + response;
       
        swal(mensagem, "Recebemos sua solicitação, em breve enntraremos em contato", "success");
        //LoadingStop('body');
        setTimeout(function () {
            window.location = "/home/index";
        }, 4500);

    });
}
