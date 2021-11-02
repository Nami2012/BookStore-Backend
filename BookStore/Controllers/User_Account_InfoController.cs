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
using BookStore.filters;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class User_Account_InfoController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: api/User_Account_Info
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        public IQueryable<User_Account_Info> GetUser_Account_Info()
        {
            return db.User_Account_Info;
        }

        // GET: api/User_Account_Info/5
        [AuthenticationFilter]
        [ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult GetUser_Account_Info(int id)
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
            if (user_Account_Info == null)
            {
                return NotFound();
            }

            return Ok(user_Account_Info);
        }

        // PUT: api/User_Account_Info/5
        [AuthenticationFilter]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser_Account_Info(int id, User_Account_Info user_Account_Info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user_Account_Info.UId)
            {
                return BadRequest();
            }

            db.Entry(user_Account_Info).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!User_Account_InfoExists(id))
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

        // POST: api/User_Account_Info //change to api/register
        [ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult PostUser_Account_Info(User_Account_Info user_Account_Info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.User_Account_Info.Add(user_Account_Info);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (User_Account_InfoExists(user_Account_Info.UId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user_Account_Info.UId }, user_Account_Info);
        }

        // DELETE: api/User_Account_Info/5
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult DeleteUser_Account_Info(int id)
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
            if (user_Account_Info == null)
            {
                return NotFound();
            }

            db.User_Account_Info.Remove(user_Account_Info);
            db.SaveChanges();

            return Ok(user_Account_Info);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool User_Account_InfoExists(int id)
        {
            return db.User_Account_Info.Count(e => e.UId == id) > 0;
        }
    }
}