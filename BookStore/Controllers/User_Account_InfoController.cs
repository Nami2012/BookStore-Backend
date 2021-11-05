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
    public class User_Account_InfoController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/User_Account_Info
        // admin only

        [HttpGet]
        [Route("api/userlist")]
        public IQueryable<User_Account_Info> GetAllUsers()
        {
            return db.User_Account_Info;
        }

        // GET: api/User_Account_Info/5
        //admin only
        [HttpGet]
        [Route("api/userdetails")]
        [ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult GetUserDetails(int id)
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
            if (user_Account_Info == null)
            {
                return NotFound();
            }

            return Ok(user_Account_Info);
        }

        // GET: api/user/{UserId}/activestatus
        //Do we need this api?
        //[HttpGet]
        //[Route("api/user/{id}/activestatus")]
        //public IHttpActionResult GetUserActiveStatus(int id)
        //{
        //    User_Account_Info user = db.User_Account_Info.Find(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(user.ActiveStatus);
        //}

        // PUT: api/user/{id}/activestatus -- is id required in the header?
        [HttpPut]
        [Route("api/user/edit/activestatus")]
        public IHttpActionResult EditUserActiveStatus(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User_Account_Info user = db.User_Account_Info.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.ActiveStatus = !user.ActiveStatus;
            db.Entry(user).Property(u => u.ActiveStatus).IsModified = true;
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

        // PUT: api/User_Account_Info/5
        //both admin and users
        [HttpPut]
        [Route("api/user/edit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditUserDetails(int id, User_Account_Info user_Account_Info)
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

        //separate api for password change required

        // POST: api/User_Account_Info
        //create user
        [HttpPost]
        [Route("api/register")]
        [ResponseType(typeof(User_Account_Info))]
        //change api to register into user credentials table too
        public IHttpActionResult CreateNewUser(User_Account_Info user_Account_Info)
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

        //// DELETE: api/User_Account_Info/5
        ////change api to remove creds from user credentials table too
        //[HttpDelete]
        //[Route("api/user/delete")]
        //[ResponseType(typeof(User_Account_Info))]
        //public IHttpActionResult DeleteUser(int id)
        //{
        //    User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
        //    if (user_Account_Info == null)
        //    {
        //        return NotFound();
        //    }
        //    db.User_Account_Info.Remove(user_Account_Info);
        //    db.SaveChanges();

        //    return Ok(user_Account_Info);
        //}

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
