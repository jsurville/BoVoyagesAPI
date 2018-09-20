using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoVoyagesAPI.Metier
{
    class CarteBancaireService
    {
        public bool ValiderSolvabilite(string numeroCarteBancaire, decimal prixTotal)
        {
            decimal solde = 0;
            decimal.TryParse(numeroCarteBancaire, out solde);
            return (solde > prixTotal);  
        }

        public bool Rembourser(string numeroCarteBancaire, decimal prixTotal)
        {
            return true ;
        }

        public bool PayerAgence(object p)
        {
            return true ;
        }
    }
}
