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
        [HttpGet]
        [Route("api/Carts")]
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
                return db.Carts.Include("Book")
                    .Where(item => item.UId == UId).ToList() //nested json here. should we change it to join?
                    .AsQueryable();
            }
        }



        // POST: api/Carts
        // Add Item to Cart
        // UId of the current user is taken
        //[ResponseType(typeof(Cart))]
        // [Authorize(Roles = "User")]
        [HttpPost]
        [Route("api/Carts/")]
        public IHttpActionResult AddToCart([FromBody] int Bid)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Books.Find(Bid) == null)
            {
                return BadRequest("No Book Found");
            }
            // Get UId from of current user
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            Cart cart = db.Carts.Find(Uid, Bid);
            if (cart != null)
            {
                return BadRequest("Book Already exists in cart.");
            }
            cart = new Cart()
            {
                UId = Uid,
                BId = Bid,
                Count = 1,
                STATUS = true
            };
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

            return CreatedAtRoute("DefaultApi", new { controller = "Carts", id = cart.UId }, cart);
        }

        // PUT : api/Cart/decrement
        //decrement quantity
        [HttpPut]
        [Route("api/Carts")]
        public IHttpActionResult DecrementCartQuantity(int Bid, int count = 1)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Books.Find(Bid) == null)
            {
                return BadRequest("No Book Found");
            }
            // Get UId from of current user
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            Cart cart = db.Carts.Find(Uid, Bid);
            if (cart == null)
            {
                return BadRequest("No such book found in cart");
            }
            else
            {
                cart.Count = count < 0 ? 0 : count;
                if (cart.Count == 0)
                {
                    db.Carts.Remove(cart);
                    db.SaveChanges();
                    return Ok(cart);
                }
            }
            db.Entry(cart).Property(c => c.Count).IsModified = true;

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

            return Ok(cart);
        }


        // DELETE: api/Carts
        // BId in request body
        // UId from user identity
        [ResponseType(typeof(Cart))]
        [HttpDelete]
        [Route("api/Carts/{BId}")]
        public IHttpActionResult DeleteCart(int BId)
        {
            // int BId = curCart.BId;
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