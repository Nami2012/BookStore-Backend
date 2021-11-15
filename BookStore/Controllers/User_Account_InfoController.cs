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

        // GET: api/userlist
        // Returns list of all users
        // Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        // Returns details of user with given UId
        [HttpGet]
        [Route("api/userdetails")]
        //[ResponseType(typeof(User_Account_Info))]
        public IHttpActionResult GetUserDetails(int id)
        {
            User_Account_Info user_Account_Info = db.User_Account_Info.Find(id);

            if (user_Account_Info == null)
            {
                return NotFound();
            }

            return Ok(db.usp_get_user_details(user_Account_Info.UId));
        }

        // PUT: api/user/edit/activestatus 
        // Activate or deactivate a user
        // Admin only
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
            // Active status of user of toggled
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


        // PUT: api/userInfo/edit
        // Edit the details of user
        [HttpPut]
        [Route("api/userInfo/edit")]
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

        // PUT
        // Edit user credentials
        // separate api for password change and credential change required
        [HttpPut]
        [Route("api/userCred/edit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditUserDetails
            (int id, User_Credentials user_credentials)
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

        // POST: api/register/user/cred
        // Create new user
        // Admin Only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/register/user/cred")]
        public IHttpActionResult CreateNewUserCredentials(User_Credentials user_Credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.usp_insert_user_credentials
                (user_Credentials.Username, user_Credentials.Password);

            User_Credentials CreatedUser
                = db.User_Credentials.SingleOrDefault
                (u => u.Username == user_Credentials.Username);
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

            // UId of newly created user is returned
            return Ok(CreatedUser.UId);
        }

        // POST: api/register/user/info
        // Create new user
        // Record is inserted to User Credentials table prior to this
        [HttpPost]
        [Route("api/register/user/info")]
        [ResponseType(typeof(User_Account_Info))]
        //change api to register into user credentials table too
        public IHttpActionResult CreateNewUserInfo
            (int id, User_Account_Info user_info)
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

        // DELETE: api/user/delete
        // Delete User
        // Admin only
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("api/user/delete")]
        public IHttpActionResult DeleteUser(int id)
        {
            // Records are deleted from User Credentials and User Details table
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

        // GET : api/isAdmin
        // Returns true if current token belongs to admin
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
