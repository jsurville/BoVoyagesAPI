using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BoVoyagesAPI.Data;
using BoVoyagesAPI.Models;
using BoVoyagesAPI.Metier;

namespace BoVoyagesAPI.Controllers
{
    public class DossierReservationsController : ApiController
    {
        private BoVoyagesContext db = new BoVoyagesContext();

        // GET: api/DossierReservations
        public IQueryable<DossierReservation> GetDossierReservations()
        {
            return db.DossierReservations.Include(x => x.Participants).Include(y => y.Client)
                .Include(t => t.Voyage)
                .Include(t => t.Voyage.Destination)
                .Include(z => z.Assurances);
        }

        // GET: api/DossierReservations/5
        [ResponseType(typeof(DossierReservation))]
        public IHttpActionResult GetDossierReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Include(x => x.Participants).Include(y => y.Client).Include(t => t.Voyage)
                .Include(z => z.Assurances).SingleOrDefault(u => u.Id == id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            return Ok(dossierReservation);
        }

        // PUT: api/DossierReservations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDossierReservation(int id, DossierReservation dossierReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dossierReservation.Id)
            {
                return BadRequest();
            }

            db.Entry(dossierReservation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DossierReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        [Route("api/AnnulerReservation/{id}")]
        [HttpPut]
        public IHttpActionResult PutAnnulerReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Find(id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            if (dossierReservation.EtatDossierReservation != EtatDossierReservation.Clos
                && dossierReservation.EtatDossierReservation != EtatDossierReservation.Annule)
            {
                if (dossierReservation.EtatDossierReservation != EtatDossierReservation.Refuse)
                {
                    if (dossierReservation.EtatDossierReservation == EtatDossierReservation.Accepte)
                    {
                        if (dossierReservation.Assurances.Where(x => x.TypeAssurance == TypeAssurance.Annulation).Count() > 0)
                        {
                            var rembourser = new CarteBancaireService().Rembourser(dossierReservation.NumeroCarteBancaire,
                                dossierReservation.PrixTotal);
                        }
                    }
                    dossierReservation.RaisonAnnulationDossier = RaisonAnnulationDossier.Client;
                }

                dossierReservation.EtatDossierReservation = EtatDossierReservation.Annule;
                db.SaveChanges();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        [Route("api/ValiderReservation/{id}")]
        [HttpPut]
        public IHttpActionResult PutValiderReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Find(id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            if (dossierReservation.EtatDossierReservation == EtatDossierReservation.EnAttente)
            {
                var carteBancaireServie = new CarteBancaireService();
                if (carteBancaireServie.ValiderSolvabilite(dossierReservation.NumeroCarteBancaire,
                    dossierReservation.PrixTotal))
                {
                    dossierReservation.EtatDossierReservation = EtatDossierReservation.EnCours;
                }    else
                {
                    dossierReservation.EtatDossierReservation = EtatDossierReservation.Refuse;
                    dossierReservation.RaisonAnnulationDossier = RaisonAnnulationDossier.PaiementRefuse;
                }
                db.SaveChanges();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        [Route("api/AccepterReservation/{id}")]
        [HttpPut]
        public IHttpActionResult PutAccepterReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Find(id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            if (dossierReservation.EtatDossierReservation == EtatDossierReservation.EnCours)
            {
                var carteBancaireServie = new CarteBancaireService();
                if (carteBancaireServie.ValiderSolvabilite(dossierReservation.NumeroCarteBancaire,
                    dossierReservation.PrixTotal))
                {
                    dossierReservation.EtatDossierReservation = EtatDossierReservation.EnCours;
                }
                else
                {
                    dossierReservation.EtatDossierReservation = EtatDossierReservation.Refuse;
                    dossierReservation.RaisonAnnulationDossier = RaisonAnnulationDossier.PaiementRefuse;
                }
                db.SaveChanges();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DossierReservations
        [ResponseType(typeof(DossierReservation))]
        public IHttpActionResult PostDossierReservation(DossierReservation dossierReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DossierReservations.Add(dossierReservation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dossierReservation.Id }, dossierReservation);
        }

        // DELETE: api/DossierReservations/5
        [ResponseType(typeof(DossierReservation))]
        public IHttpActionResult DeleteDossierReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Find(id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            db.DossierReservations.Remove(dossierReservation);
            db.SaveChanges();

            return Ok(dossierReservation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DossierReservationExists(int id)
        {
            return db.DossierReservations.Count(e => e.Id == id) > 0;
        }
    }
}