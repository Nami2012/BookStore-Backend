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
    public class CouponsController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();
        //api/Coupons -- retrieve activecoupons
        [HttpGet]
        [Route("api/Coupons/all")]
        public IQueryable<Coupon> GetActiveCoupons()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var role = identity.Name;
            if (role == "admin")
                return db.Coupons;


            return db.Coupons.Where(c => c.Status == true);
        }

        ///api/Coupons/Add --post admin only
        [ResponseType(typeof(Book))]
        [HttpPost]
        [Route("api/admin/Coupons/add")]
        public IHttpActionResult PostCoupon(Coupon coupon)
        {
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/

            coupon.Status = true;
            db.Coupons.Add(coupon);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CouponExists(coupon.CouponId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("", new { id = coupon.CouponId }, coupon); //add route name here
        }

        //api/Coupons/Remove/id --delete admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ResponseType(typeof(Book))]
        [Route("api/admin/coupon/remove")]
        public IHttpActionResult RemoveCoupon(string id)
        {
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }
            db.Coupons.Remove(coupon);
            db.SaveChanges();

            return Ok(coupon);
        }

        private bool CouponExists(string id)
        {
            return db.Coupons.Count(e => e.CouponId == id) > 0;
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
