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
            int UId = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            User_Account_Info user = db.User_Account_Info.SingleOrDefault(u => u.UId == UId);
            if (user == null)
            {
                return -1;
            }
            return user.UId;
        }

        //GET /api/wishlist 
        [Route("api/wishlist")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetWishList()
        {

            int uid = getUserId();
            if (uid < 0)
            {
                return BadRequest();
            }

            return Ok(db.usp_get_wishlist_by_uid(uid));
        }

        [HttpGet]
        [Route("api/carts/isinwishlist/{Bid}")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult IsPresentInWishlist(int Bid)
        { // Get UId from of current user
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Wishlists.Find(Uid, Bid) != null)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        //POST /wishlist/add queryparam=> bi
        [Route("api/wishlist")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult PostWishlist([FromBody] int bid)
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
        //POST /wishlist/add queryparam=> bi
        [Route("api/wishlist/{bid}")]
        [Authorize]
        [HttpDelete]
        public IHttpActionResult RemoveFromWishlist(int bid)
        {
            int uid = getUserId();
            if (uid < 0)
            {
                return BadRequest();
            }

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
