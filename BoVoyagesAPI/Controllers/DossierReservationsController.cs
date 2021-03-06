﻿using System;
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

        // GET: api/DossierReservation/search
        [Route("api/DossierReservation/search")]
        [HttpGet]
        public IQueryable<DossierReservation> GetRechercherReservations(DateTime? dateDebut,
            DateTime? dateFin, string destination, string nomClient)
        {
            return db.DossierReservations.Include(x => x.Participants).Include(y => y.Client)
                .Include(t => t.Voyage)
                .Include(t => t.Voyage.Destination)
                .Include(z => z.Assurances)
                .Where(x => x.Voyage.Destination.Pays.Contains(destination)
                || x.Voyage.Destination.Continent.Contains(destination)
                || x.Voyage.Destination.Region.Contains(destination)
                || x.Voyage.Destination.Description.Contains(destination)
                || x.Client.Nom.Contains(nomClient)
                || x.Client.Prenom.Contains(nomClient)
                || (x.Voyage.DateAller> dateDebut 
                && x.Voyage.DateRetour<dateFin));        
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

            if (dossierReservation.Annuler())
            {
                db.SaveChanges();
                return Ok(dossierReservation.EtatDossierReservation);
            }
            else return BadRequest();       
        }

        [ResponseType(typeof(void))]
        [Route("api/CloturerReservation/{id}")]
        [HttpPut]
        public IHttpActionResult PutCloturerReservation(int id)
        {
            DossierReservation dossierReservation = db.DossierReservations.Find(id);
            if (dossierReservation == null)
            {
                return NotFound();
            }

            if (dossierReservation.Cloturer())
            {
                db.SaveChanges();
                return Ok(dossierReservation.EtatDossierReservation);
            }
            else return BadRequest();
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
            if (dossierReservation.ValiderSolvabilite())
            {
                db.SaveChanges();
                return Ok(dossierReservation.EtatDossierReservation);
            }
            else
                return BadRequest();       
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
            var voyage = db.Voyages.Find(dossierReservation.VoyageId);
            var placeDisponible = voyage.Reserver(dossierReservation.Participants.Count);

            if (dossierReservation.Accepter(placeDisponible))            
                db.SaveChanges();            
            else
                return BadRequest();

            return Ok(dossierReservation.EtatDossierReservation);
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