using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            return db.Coupons.Where(c => c.Status == true);
        }

        //api/admin/coupon --retrieve all coupons  --admin only
        [HttpGet]
        [Route("api/admin/Coupons/all")]
        public IQueryable<Coupon> GetCoupons()
        {
            return db.Coupons;
        }

        ///api/Coupons/Add --post admin only
        [ResponseType(typeof(Book))]
        [HttpPost]
        [Route("api/admin/Coupons/add")]
        public IHttpActionResult PostCoupon(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
        
        // private function apply coupon => return percentage of total value to be reduced and adds its to the coupon validation table
        //untested
        //public static float applyCoupon(string couponid, int userid)
        //{
        //    Coupon_Validation coupon = db.Coupon_Validation.SingleOrDefault(c => c.CouponId == couponid && c.UId == userid);
        //    if (coupon != null || coupon.CouponValid == false)
        //    {
        //        return -1;
        //    }
        //    Coupon applied_coupon = db.Coupons.SingleOrDefault(c => c.CouponId == couponid);
        //    return (float)applied_coupon.Discount;
        //}
        
        
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
