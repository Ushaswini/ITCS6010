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
using PracticeApp.Models;

namespace PracticeApp.Controllers
{
    public class UserDetailsController : ApiController
    {
        private PracticeAppContext db = new PracticeAppContext();

        // GET: api/UserDetails
        public IQueryable<User> GetUserDetails()
        {
            return db.UserDetails;
        }

        // GET: api/UserDetails/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUserDetails(int id)
        {
            User userDetails = db.UserDetails.Find(id);
            if (userDetails == null)
            {
                return NotFound();
            }

            return Ok(userDetails);
        }

        // PUT: api/UserDetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserDetails(int id, User userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userDetails.Id)
            {
                return BadRequest();
            }

            db.Entry(userDetails).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDetailsExists(id))
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

        // POST: api/UserDetails
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUserDetails(User userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserDetails.Add(userDetails);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = userDetails.Id }, userDetails);
        }

        // DELETE: api/UserDetails/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUserDetails(int id)
        {
            User userDetails = db.UserDetails.Find(id);
            if (userDetails == null)
            {
                return NotFound();
            }

            db.UserDetails.Remove(userDetails);
            db.SaveChanges();

            return Ok(userDetails);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserDetailsExists(int id)
        {
            return db.UserDetails.Count(e => e.Id == id) > 0;
        }
    }
}