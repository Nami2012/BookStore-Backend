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
    public class WishlistsController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: api/Wishlists
        public IQueryable<Wishlist> GetWishlists()
        {
            return db.Wishlists;
        }

        // GET: api/Wishlists/5
        [ResponseType(typeof(Wishlist))]
        public IHttpActionResult GetWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            return Ok(wishlist);
        }

        // PUT: api/Wishlists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWishlist(int id, Wishlist wishlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wishlist.UId)
            {
                return BadRequest();
            }

            db.Entry(wishlist).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(id))
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

        // POST: api/Wishlists
        [ResponseType(typeof(Wishlist))]
        public IHttpActionResult PostWishlist(Wishlist wishlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wishlists.Add(wishlist);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (WishlistExists(wishlist.UId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = wishlist.UId }, wishlist);
        }

        // DELETE: api/Wishlists/5
        [ResponseType(typeof(Wishlist))]
        public IHttpActionResult DeleteWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            db.Wishlists.Remove(wishlist);
            db.SaveChanges();

            return Ok(wishlist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WishlistExists(int id)
        {
            return db.Wishlists.Count(e => e.UId == id) > 0;
        }
    }
}