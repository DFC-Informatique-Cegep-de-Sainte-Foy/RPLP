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

    // Afficher le popup pour confirmer la suppression de l'assignment
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

    // Appeller la méthode SuppressionAssignment qui se trouve dans le controleur Teacher (passé en paramètre dans le bouton)
    // Cacher le popup
    // Appeller la page index du controlleur Teacher en POST avec le bon cours sélectionné dans la DropDownList
    $("#confirm-delete").on('click', () => {
        $.get(url)
            .always(() => {

                $("#deleteModal").modal('hide');
                $('#DropDownList').submit();
            });
    });

    // Changer le nom du bouton Supprimer dans le popup
    $("#confirm-delete").click(function () {
        var texto = $(this).text();
        $(this).text(texto == "Attendez..." ? "Attendez..." : "Attendez...");
    });

    // Changer le nom du bouton Envoyer à Codepost dans la page AjouterTravail
    $("#btnEnvoyerACodepost").click(function () {
        var texto = $(this).text();
        $(this).text(texto == "Attendez..." ? "Attendez..." : "Attendez...");
    });

}()));