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
            try
            {
                return ((int.Parse(numeroCarteBancaire) -(int) prixTotal) % 2) == 0;
            }
            catch
            {
                return false;
            }
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
