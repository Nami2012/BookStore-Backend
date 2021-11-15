using BookStore.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookStore.Controllers
{
    public class CouponsController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();


        // GET : api/Coupons/all
        // Returns all coupons for admin
        // Returns all active coupons for normal request
        [HttpGet]
        [Route("api/Coupons/all")]
        public IQueryable<Coupon> GetActiveCoupons()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var role = identity.Name;

            // If admin return all coupons
            if (role == "admin")
                return db.Coupons;
            
            // Else return active coupons
            return db.Coupons.Where(c => c.Status == true);
        }


        // GET: api/Coupons/valid
        // Get valid coupons for a user
        [HttpGet]
        [Route("api/Coupons/valid")]
        [Authorize(Roles = "User")]
        public IHttpActionResult GetValidCouponsForUser()
        {
            var identity = (ClaimsIdentity)User.Identity;
            int UId = int.Parse(
                       identity.Claims.Where(c => c.Type == "UId")
                       .Select(c => c.Value).FirstOrDefault()
                   );

            return Ok(db.usp_valid_coupons(UId));
        }


        // POST : api/admin/Coupons/add
        // Create new Coupons
        // Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/admin/Coupons/add")]
        public IHttpActionResult PostCoupon(Coupon coupon)
        {
            // New coupons are made active by default
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
            return Ok();
        }

        // DELETE : api/admin/coupon/remove
        // Delete Coupon with given couponId
        // Admin Only
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("api/admin/coupon/remove")]
        public IHttpActionResult RemoveCoupon(string couponId)
        {
            Coupon coupon = db.Coupons.Find(couponId);

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
