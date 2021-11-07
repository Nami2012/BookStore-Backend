using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookStore.Controllers
{
    public class WishListsController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        private bool WishlistExists(int bid, int uid)
        { 
            return db.Wishlists.Count(e => ((e.BId == bid) && (e.UId == uid))) > 0;
        }

        int getUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            User_Account_Info user = db.User_Account_Info.SingleOrDefault(u => u.UId == int.Parse(identity.Name));
            if (user == null)
            {
                return -1;
            }
            return user.UId;
        }

        //GET /api/wishlist 
        [ResponseType(typeof(List<Wishlist>))]
        public IHttpActionResult GetWishList()
        {

            int uid = getUserId();
            if (uid < 0)
            {
                return BadRequest();
            }
            List<Wishlist> WishLists = db.Wishlists.Where(W => W.UId == uid).ToList<Wishlist>();
            return Ok(WishLists);
        }
        //POST /wishlist/add queryparam=> bi

        [ResponseType(typeof(Wishlist))]
        public IHttpActionResult PostWishlist(int bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int uid = getUserId();
            if (uid < 0)
            {
                return BadRequest();
            }
            Wishlist wishlist = new Wishlist()
            {
                BId = bid,
                UId = uid,
                STATUS = true
            };
            db.Wishlists.Add(wishlist);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (WishlistExists(wishlist.BId, wishlist.UId))
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
        //DELETE /wishlist/remove queryparam => bid
        // DELETE: api/Books/5
        [ResponseType(typeof(Wishlist))]


        public IHttpActionResult DeleteRemoveFromWishlist(int bid, int uid)
        {
           

            Wishlist wishlist = db.Wishlists.SingleOrDefault(w => w.UId == uid && w.BId == bid);
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

    }
}
