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

namespace BoVoyagesAPI.Controllers
{
    public class ClientsController : ApiController
    {
        private BoVoyagesContext db = new BoVoyagesContext();

        // GET: api/Clients
        public IQueryable<Client> GetClients()
        {
            return db.Clients;
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClient(int id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }


        // GET: api/Clients/search
        [Route("api/Clients/search")]
        public IQueryable<Client> GetRechercherClient(string nom)
        {
            return db.Clients.Where(x => x.Nom.Contains(nom) || x.Prenom.Contains(nom) || x.Email.Contains(nom));
        }

        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(int id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Le Formulaire doit renseigner un Nom et une Date de Naissance");
            }

            if (id != client.Id)
            {
                return BadRequest();
            }

            if (client.DateNaissance == null)
            {
                return BadRequest("Vous devez rentrer obligatoirement une Date de Naissance");
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // POST: api/Clients
        [ResponseType(typeof(Client))]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (client.DateNaissance.AddYears(18) < DateTime.Now)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return CreatedAtRoute("DefaultApi", new { id = client.Id }, client);
            }
            else
            {
                return BadRequest("Client non majeur");
            }            
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = db.Clients.Find(id);
            
            if (client == null)
            {
                return NotFound();
            }

            DossierReservation dossierReservation = db.DossierReservations.ToList().Find(x => x.ClientId == id);

            if (dossierReservation != null)
            {
                return BadRequest("Impossible: le Client sélectionné est enregistré dans un Dossier de Réservation");
            }
            else
            {
                db.Clients.Remove(client);
                db.SaveChanges();

                return Ok(client);
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Clients.Count(e => e.Id == id) > 0;
        }
    }
}