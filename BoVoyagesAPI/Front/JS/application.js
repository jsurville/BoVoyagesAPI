function setLoader(element) {
    element.append($('<div class="loader"></div>'));
}

function Erreur() {
	alert("Erreur !");
}

function AfficheErreur(Erreur) {
	alert(Erreur.responseJSON.Message);
}

