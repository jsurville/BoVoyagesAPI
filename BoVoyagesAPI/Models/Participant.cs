using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BoVoyagesAPI.Models
{

	public class Participant : Personne
	{
		public int NumeroUnique { get; set; }
		public double Reduction
		{
			get
			{
				if (Age < 12)
					return 0.6d;
				else
					return 0d;
			}
		}

		public int DossierReservationId { get; set; }

		[ForeignKey("DossierReservationId")]
		public DossierReservation DossierReservation { get; set; }
		public string NomComplet
		{
			get
			{
				return Civilite + " " + Nom + " " + Prenom;
			}
		}


	}
}

