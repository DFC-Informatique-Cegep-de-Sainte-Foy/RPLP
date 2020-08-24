$((function () {
    var url;
    var redirectUrl;
    var target;

    $('body').append(`
            <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="myModalLabel">Attention</h4>
                </div>
                <div class="modal-body delete-modal-body">
                    
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal" id="cancel-delete">Annuler</button>
                    <button type="button" class="btn btn-danger" id="confirm-delete">Supprimer</button>
                </div>
                </div>
            </div>
            </div>`);

    //Delete Action
    $(".delete").on('click', (e) => {
        e.preventDefault();

        target = e.target;
        var Id = $(target).data('id');
        var controller = $(target).data('controller');
        var action = $(target).data('action');
        var bodyMessage = $(target).data('body-message');
        redirectUrl = $(target).data('redirect-url');

        url = "/" + controller + "/" + action + "?Id=" + Id;
        $(".delete-modal-body").text(bodyMessage);
        $("#deleteModal").modal('show');
        
    });

    ////Delete Action
    $("#confirm-delete").on('click', () => {
        $.get(url)
            .always(() => {

                $("#deleteModal").modal('hide');
                $('#DropDownList').submit();
            });
    });

    $("#confirm-delete").click(function () {
        var texto = $(this).text();
        $(this).text(texto == "Attendez..." ? "Attendez..." : "Attendez...");
    });

    $("#btnEnvoyerACodepost").click(function () {
        var texto = $(this).text();
        $(this).text(texto == "Attendez..." ? "Attendez..." : "Attendez...");
    });

}()));