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
using System.Web.Http.Description;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class CartsController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        /*[HttpGet]
        [Route("api/userssss")]
        [Authorize]
        public IHttpActionResult GetUser()
        {
            var identity = (ClaimsIdentity)User.Identity;
            //return Ok(identity.Name);
            return Ok(identity.Claims.Where(c => c.Type == "UId").Select(c => c.Value));
        }*/

        // GET: api/Carts
        // If admin, return carts of all user
        // If user, return their cart
        // One user have only one cart
        [Authorize]
        public IQueryable<Cart> GetCarts()
        {
            var identity = (ClaimsIdentity)User.Identity;
            // Get the role of the current logged in User
            var role = identity.Claims.
                Where(c => c.Type == ClaimTypes.Role).
                Select(c => c.Value);
            // Admin can get carts of all users
            if (role.Equals("Admin"))
            {
                return db.Carts;
            }
            // User can access only their cart
            else
            {
                int UId = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
                return db.Carts
                    .Where(item => item.UId == UId)
                    .AsQueryable();
            }
        }

        

        // POST: api/Carts
        // Add Item to Cart
        // UId of the current user is taken
        [ResponseType(typeof(Cart))]
        [Authorize(Roles = "User")]
        public IHttpActionResult PostCart(Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get UId from of current user
            var identity = (ClaimsIdentity)User.Identity;
            int UId = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            cart.UId = UId;

            db.Carts.Add(cart);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CartExists(cart.UId, cart.BId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cart.UId }, cart);
        }

        // DELETE: api/Carts
        // BId in request body
        // UId from user identity
        [ResponseType(typeof(Cart))]
        public IHttpActionResult DeleteCart(Cart curCart)
        {
            int BId = curCart.BId;
            var identity = (ClaimsIdentity)User.Identity;
            int UId = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            Cart cart = db.Carts.Find(UId, BId);
            if (cart == null)
            {
                return NotFound();
            }

            db.Carts.Remove(cart);
            db.SaveChanges();

            return Ok(cart);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CartExists(int UId, int BId)
        {
            return db.Carts.Count(e => e.UId == UId && e.BId == BId) > 0;
        }
    }
}