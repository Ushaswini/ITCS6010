using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Homework1.Models;
using Microsoft.AspNet.Identity;
using System.IdentityModel.Tokens;
using System.IdentityModel.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace Homework1.Controllers
{
    [RoutePrefix("api/Messages")]
    public class MessagesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Messages
        
        public IQueryable<Message> GetMessages()
        {
            return db.Messages;
        }
        
        
        public IQueryable<Message> GetMessagesForReceiver(string receiverId)
        {
            var result = from d in db.Messages where d.ReceiverId == (receiverId) select d;
            return result;
        }

        //api/Messages/EditReadStatus
        [Route("EditReadStatus")]
        public async Task<IHttpActionResult> EditReadStatus(int messageId)
        {
            Message message = await db.Messages.FindAsync(messageId);

            if(message != null)
            {
                message.IsRead = true;
                db.Messages.AddOrUpdate(message);
                await db.SaveChangesAsync();

                return Ok(message);
            }
            return NotFound();
        }

        //api/Messages/EditLockStatus
        [Route("EditLockStatus")]
        public async Task<IHttpActionResult> EditLockStatus(int messageId)
        {
            Message message = await db.Messages.FindAsync(messageId);

            if (message != null)
            {
                message.IsUnLocked = true;
                db.Messages.AddOrUpdate(message);
                await db.SaveChangesAsync();

                return Ok(message);
            }
            return NotFound();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        // GET: api/Messages/5
        [ResponseType(typeof(Message))]

        public async Task<IHttpActionResult> GetMessage(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        [ResponseType(typeof(void))]
        
        public async Task<IHttpActionResult> PutMessage(int id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return BadRequest();
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Messages
        [ResponseType(typeof(Message))]
       
        public async Task<IHttpActionResult> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [ResponseType(typeof(Message))]
        
        public async Task<IHttpActionResult> DeleteMessage(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(int id)
        {
            return db.Messages.Count(e => e.Id == id) > 0;
        }
    }
}