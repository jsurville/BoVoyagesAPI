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
    public class DestinationsController : ApiController
    {
        private BoVoyagesContext db = new BoVoyagesContext();

        // GET: api/Destinations
        public IQueryable<Destination> GetDestinations()
        {
            return db.Destinations;
        }

        // GET: api/Destinations/5
        [ResponseType(typeof(Destination))]
        public IHttpActionResult GetDestination(int id)
        {
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return NotFound();
            }

            return Ok(destination);
        }

        // GET: api/Destinations
        [Route("api/Destinations/search")]
        public IQueryable<Destination> GetRechercherDestination(string nom)
        {
            return db.Destinations.Where(x => x.Continent.Contains(nom) || x.Description.Contains(nom) || x.Pays.Contains(nom) || x.Region.Contains(nom) );
        }


        // PUT: api/Destinations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDestination(int id, Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != destination.Id)
            {
                return BadRequest();
            }

            db.Entry(destination).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
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

        // POST: api/Destinations
        [ResponseType(typeof(Destination))]
        public IHttpActionResult PostDestination(Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Destinations.Add(destination);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = destination.Id }, destination);
        }

        // DELETE: api/Destinations/5
        [ResponseType(typeof(Destination))]
        public IHttpActionResult DeleteDestination(int id)
        {
            Destination destination = db.Destinations.Find(id);
            var possedeVoyages = db.Voyages.Any(x => x.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            if (possedeVoyages)
            {
                return BadRequest("Impossible: un Voyage est enregistré pour cette Destination");
            }
            else
            {
                db.Destinations.Remove(destination);
                db.SaveChanges();

                return Ok(destination);
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

        private bool DestinationExists(int id)
        {
            return db.Destinations.Count(e => e.Id == id) > 0;
        }
    }
}