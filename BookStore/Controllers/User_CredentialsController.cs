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
using BookStore.Models;

namespace BookStore.Controllers
{
    public class User_CredentialsController : ApiController
    {
        private static BookStoreDBEntities db = new BookStoreDBEntities();

        public static bool Validate(string username,string password)
        {
            User_Credentials User = db.User_Credentials.SingleOrDefault(user => user.Username == username);
            if(User!=null && User.Username == username && User.Username == password)
            {
                return true;
            }
            return false;
        }
        // GET: api/User_Credentials
        public IQueryable<User_Credentials> GetUser_Credentials()
        {
            return db.User_Credentials;
        }

        // GET: api/User_Credentials/5
        [ResponseType(typeof(User_Credentials))]
        public IHttpActionResult GetUser_Credentials(int id)
        {
            User_Credentials user_Credentials = db.User_Credentials.Find(id);
            if (user_Credentials == null)
            {
                return NotFound();
            }

            return Ok(user_Credentials);
        }

        // PUT: api/User_Credentials/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser_Credentials(int id, User_Credentials user_Credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user_Credentials.UId)
            {
                return BadRequest();
            }

            db.Entry(user_Credentials).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!User_CredentialsExists(id))
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

        // POST: api/User_Credentials
        [ResponseType(typeof(User_Credentials))]
        public IHttpActionResult PostUser_Credentials(User_Credentials user_Credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.User_Credentials.Add(user_Credentials);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (User_CredentialsExists(user_Credentials.UId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user_Credentials.UId }, user_Credentials);
        }

        // DELETE: api/User_Credentials/5
        [ResponseType(typeof(User_Credentials))]
        public IHttpActionResult DeleteUser_Credentials(int id)
        {
            User_Credentials user_Credentials = db.User_Credentials.Find(id);
            if (user_Credentials == null)
            {
                return NotFound();
            }

            db.User_Credentials.Remove(user_Credentials);
            db.SaveChanges();

            return Ok(user_Credentials);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool User_CredentialsExists(int id)
        {
            return db.User_Credentials.Count(e => e.UId == id) > 0;
        }
    }
}