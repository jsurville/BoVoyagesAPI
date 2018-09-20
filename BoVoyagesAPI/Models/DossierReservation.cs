using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoVoyagesAPI.Models
{
    public class DossierReservation
    {
      public  DossierReservation()
        {
            this.Participants = new HashSet<Participant>();
            this.Assurances = new HashSet<Assurance>();
        }

        public int Id { get; set; }
        public int NumeroUnique { get; set; }
        public string NumeroCarteBancaire { get; set; }
        public decimal PrixParPersonne { get; set; }
        public decimal PrixTotal
        {
            get
            {
                decimal prixTotal = 0;
                foreach (var participant in this.Participants)
                {
                    prixTotal += (1 - (decimal)participant.Reduction) * PrixParPersonne;
                }

                foreach (var assurance in this.Assurances)
                {
                    if (assurance.TypeAssurance == TypeAssurance.Annulation)
                    {
                        prixTotal += (decimal)assurance.Montant;
                    }
                }
                return prixTotal;
            }
        }
        public EtatDossierReservation EtatDossierReservation { get; set; }
        public RaisonAnnulationDossier RaisonAnnulationDossier { get; set; }
        public int VoyageId { get; set; }
        public int ClientId { get; set; }


        [ForeignKey("VoyageId")]
        public Voyage Voyage { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public ICollection<Assurance> Assurances { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }


    public enum EtatDossierReservation { EnAttente, EnCours, Refuse, Accepte, Clos, Annule }
    public enum RaisonAnnulationDossier { Client = 1, PlacesInsuffisantes = 2, PaiementRefuse = 3 }
}
