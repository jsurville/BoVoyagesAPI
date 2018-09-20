function setLoader(element) {
    element.append($('<div class="loader"></div>'));
}

function getVoyages() {
    var divListeVoyages = $('#listeVoyages');
    setLoader(divListeVoyages);

    $.ajax({
        url: '/api/Voyages',
        success: function (voyages) {
            divListeVoyages.empty();
            if (voyages.length > 0) {
                for (voyage of voyages) {
                    var divVoyage = $('<div class="Voyage alert"></div>');
                    divVoyage.attr("data-id", voyage.Id);

                    if (voyage.Statut) {
                        divVoyage.addClass('alert-success');
                    }

                    var inputEtat = $('<input type="checkbox">');
                    inputEtat.attr('checked', voyage.Statut);
                    inputEtat.on('click', function () {
                        var div = $(this).closest(".Voyage");
                        var idVoyage = div.attr("data-id");
                        ModifierStatutVoyage(idVoyage, this);
                    });

                    var buttonDelete = $('<button class="btn btn-danger btn-sm"><i class="fas fa-trash"></i></button>');
                    buttonDelete.on('click', function () {
                        // Affichage d'une confirmation
                        $.bsAlert.confirmTitle = 'Suppression';
                        $.bsAlert.closeDisplay = 'Non';
                        $.bsAlert.sureDisplay = 'Oui';
                        $.bsAlert.confirm("Es-tu sûr?", () => {
                            var div = $(this).closest(".Voyage");
                            var idVoyage = div.attr("data-id");
                            SupprimerVoyage(idVoyage, div);
                        });
                    });

                    divVoyage.append(inputEtat);
                    divVoyage.append($('<label class="col-2"></label>').text(voyage.Destination.Pays));
                    divVoyage.append($('<label class="col-2"></label>').text(voyage.DateAller));
                    divVoyage.append($('<label class="col-2"></label>').text(voyage.DateRetour));
                    divVoyage.append($('<label class="col-2"></label>').text(voyage.PlaceDisponible));
                    divVoyage.append($('<label class="col-2"></label>').text(voyage.PrixParpersonne));
                    divVoyage.append(buttonDelete);

                    divListeVoyages.append(divVoyage);
                }
            } else {
                divListeVoyages.append($('<p class="lead">Aucune Voyage pour le moment...</p>'));
            }
        },
        error: Erreur
    });
}


function ModifierStatutVoyage(id, input) {
    $.ajax({
        type: 'PUT',
        url: '/api/Voyagestatut/' + id + '?statut=' + input.checked,
        success: function () {
            var divVoyage = $(input).closest(".Voyage");
            if (input.checked) {
                divVoyage.addClass('alert-success');
            } else {
                divVoyage.removeClass('alert-success');
            }
        },
        error: Erreur
    });
}

function SupprimerVoyage(id, div) {
    $.ajax({
        type: 'DELETE',
        url: '/api/Voyages/' + id,
        success: function () {
            getVoyages();
        },
        error: Erreur
    });
}

function Erreur() {
    alert("Erreur !");
}