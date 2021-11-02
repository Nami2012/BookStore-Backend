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
using System.Web;
using System.Security.Principal;

namespace BookStore.Controllers
{
    public class AdminsController : ApiController
    {
        private static BookStoreDBEntities db = new BookStoreDBEntities();

        //Authenticates admin
        public static bool Validate(string username,string password)
        {
            Admin admin = db.Admins.Find(username);
            
            if( admin!=null && admin.Username == username && admin.Password == password)
            {
                return true;
            }
            return false;
        }

        //// POST: api/Admins/login
        //[Route("api/Admins/Login")]
        //[ResponseType(typeof(Admin))]
        //[HttpPost]
        //public IHttpActionResult Login(Admin a)
        //{
        //    Admin admin = db.Admins.Find(a.Username);
        //    if (admin.Username == a.Username && admin.Password==a.Password)
        //    {
        //        var identity = new GenericIdentity(admin.Username);
        //        var principal = new GenericPrincipal(identity, null);
        //        HttpContext.Current.User = principal;
        //        return Ok("Logged In");
                
        //    }

        //    return NotFound();
        //}

        ////POST: api/Admins/logout
        //[Route("api/Admins/Logout")]
        //[HttpPost]
        //public IHttpActionResult Logout()
        //{

        //    HttpContext.Current.User = null;
        //    return Ok("Logged Out");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdminExists(string id)
        {
            return db.Admins.Count(e => e.Username == id) > 0;
        }
    }
}