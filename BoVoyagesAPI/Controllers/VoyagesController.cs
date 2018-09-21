using BoVoyagesAPI.Data;
using BoVoyagesAPI.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace BoVoyagesAPI.Controllers
{
    public class VoyagesController : ApiController
    {
        private BoVoyagesContext db = new BoVoyagesContext();

        // GET: api/Voyages
        public IQueryable<Voyage> GetVoyages()
        {
            return db.Voyages.Include(x => x.Destination).Include(y => y.AgenceVoyage);
        }

        // GET: api/Voyages/5
        [ResponseType(typeof(Voyage))]
        public IHttpActionResult GetVoyage(int id)
        {
            Voyage voyage = db.Voyages.Include(x => x.Destination).Include(y => y.AgenceVoyage).SingleOrDefault(z => z.Id == id);
            if (voyage == null)
            {
                return NotFound();
            }

            return Ok(voyage);
        }

        // GET: api/Voyages
        [Route("api/Voyages/search")]
        public IQueryable<Voyage> GetRechercherVoyages(DateTime? dateDebut, DateTime? dateFin, string nom)
        {
            return db.Voyages.Include(x => x.Destination)
                .Where(x => x.Destination.Description.Contains(nom)
                || x.Destination.Continent.Contains(nom)
                || x.Destination.Pays.Contains(nom)
                || x.Destination.Region.Contains(nom)
                 || (x.DateAller > dateDebut
                && x.DateRetour < dateFin));
        }

        // PUT: api/Voyages/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVoyage(int id, Voyage voyage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voyage.Id)
            {
                return BadRequest();
            }

            db.Entry(voyage).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoyageExists(id))
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

        // POST: api/Voyages
        [ResponseType(typeof(Voyage))]
        public IHttpActionResult PostVoyage(Voyage voyage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (voyage.DateAller > DateTime.Now.AddDays(3) && voyage.DateRetour > voyage.DateAller.AddDays(2))
            {
                db.Voyages.Add(voyage);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = voyage.Id }, voyage);
            }
            else
                return BadRequest("Mauvaise Date");

        }

        // DELETE: api/Voyages/5
        [ResponseType(typeof(Voyage))]
        public IHttpActionResult DeleteVoyage(int id)
        {
            Voyage voyage = db.Voyages.Find(id);
            if (voyage == null)
            {
                return NotFound();
            }
            var dossierReservation = db.DossierReservations.ToList().Find(x => x.VoyageId == id);
            if (dossierReservation != null)
            {
                return BadRequest("le Voyage sélectionné est lié dans un Dossier de Réservation");
            }
            else
            {
                db.Voyages.Remove(voyage);
                db.SaveChanges();
            }
            return Ok(voyage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoyageExists(int id)
        {
            return db.Voyages.Count(e => e.Id == id) > 0;
        }
    }
}