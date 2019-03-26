$(document).ready(function () {
    LoadingStart('body');
    GetDashboards();
    GetSaldoEmpresas();
  
});


function GetDashboards() {
    var settings = {
        "url": "/home/DashTotaisHome",
        "method": "GET"
    }
    setInterval(function () {
        $.ajax(settings).done(function (response) {

            console.log(response[0].saldoStaffPro);
            console.log(response[0]);

            $('#saldoOperacao').html("R$ " + response[0].saldoOperacao);
            $('#saldoStaffPro').html("R$" + response[0].saldoStaffPro);
            $('#totalEmpresas').html(response[0].totalEmpresas);
            $('#totalOportunidades').html(response[0].totalOportunidades);

        });


    }, 3000);
}

function GetSaldoEmpresas() {
    var settings = {
        "url": "/home/SaldoEmpresas",
        "method": "GET"
    }
    setInterval(function () {
        $.ajax(settings).done(function (response) {
            var table = "";
            table += " <table class='table'>";
            table += " <thead>";
            table += " <tr>";
            table += " <th>ID</th>";
            table += " <th>Nome</th>";
            table += " <th>Saldo</th>";
            table += " </tr>";
            table += " </thead>";

            table += "<tbody>";
          
          

            for (var i = 0; i < response.length; i++) {
                table += "<tr>";
                table += "<td>" + response[i].id + "</td>";
                table += "<td>" + response[i].nome + "</td>";
                table += "<td class='text-right'>";
                table += "R$" + response[i].saldo;
                table += "</td>";
                table += "</tr>";
            }
            table += "</tbody>";
            table += "</table>";

            $('#saldosEmpresas').html(table);
            LoadingStop('body');

        });

    }, 3000);

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