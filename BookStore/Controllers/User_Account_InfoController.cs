using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using BookStore.Models;

namespace BookStore.Controllers
{

    public class User_Account_InfoController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/User_Account_Info
        // admin only
        //works fine
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/userlist")]
        public IHttpActionResult GetAllUsers()
        {
            var userlist = db.User_Account_Info.Select(u => new
            {
                u.UId,
                u.Name,
                u.ActiveStatus
            });
            return Ok(userlist);
        }

        // GET: api/User_Account_Info/5

        [HttpGet]
        [Route("api/userdetails")]
        //[ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult GetUserDetails(int id) //check whether its admin or the user id itself
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
            if (user_Account_Info == null)
            {
                return NotFound();
            }
            return Ok(db.usp_get_user_details(user_Account_Info.UId));
        }

        // PUT: api/user/edit/activestatus 
        [Authorize(Roles = "Admin")]
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
        [Route("api/userInfo/edit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditUserDetails(int id, User_Account_Info user_Account_Info)
        { //check if its the user itself or is it the admin logging in
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


        //separate api for password change and credential change required
        [HttpPut]
        [Route("api/userCred/edit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditUserDetails(int id, User_Credentials user_credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user_credentials.UId)
            {
                return BadRequest();
            }

            db.Entry(user_credentials).State = EntityState.Modified;

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
        //create user
        [HttpPost]
        [Route("api/register/user/cred")]
        //change api to register into user credentials table too
        public IHttpActionResult CreateNewUserCredentials(User_Credentials user_Credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.usp_insert_user_credentials(user_Credentials.Username, user_Credentials.Password);
            User_Credentials CreatedUser = db.User_Credentials.SingleOrDefault(u => u.Username == user_Credentials.Username);
            // db.User_Account_Info.Add(user_info);
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
            return Ok(CreatedUser.UId);
        }

        // POST: api/User_Account_Info
        //create user
        [HttpPost]
        [Route("api/register/user/info")]
        [ResponseType(typeof(User_Account_Info))]
        //change api to register into user credentials table too
        public IHttpActionResult CreateNewUserInfo(int id, User_Account_Info user_info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user_info.UId = id;
            db.User_Account_Info.Add(user_info);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (User_Account_InfoExists(user_info.UId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        // DELETE: api/User_Account_Info/5
        //change api to remove creds from user credentials table too
        [HttpDelete]
        [Route("api/user/delete")]
        [ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult DeleteUser(int id)
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);
            User_Credentials user_Credentials = db.User_Credentials.Find(id);
            if (user_Account_Info == null || user_Credentials == null)
            {
                return NotFound();
            }
            db.User_Account_Info.Remove(user_Account_Info);
            db.User_Credentials.Remove(user_Credentials);
            db.SaveChanges();

            return Ok(user_Account_Info);
        }

        // GET : api/isAdminCheck

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/isAdmin")]
        public IHttpActionResult IsAdminCheck()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var role = identity.Name;
            if (role == "admin")
                return Ok(true);
            else
                return Ok(false);
            //return Ok("isAdmin");
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
        private bool User_CredentialsExists(int id)
        {
            return db.User_Credentials.Count(u => u.UId == id) > 0;
        }
    }
}
