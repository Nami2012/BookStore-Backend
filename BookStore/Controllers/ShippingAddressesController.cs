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
    public class ShippingAddressesController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/ShippingAddresses
        // Returns the shipping address of user
        [HttpGet]
        [Authorize]
        [Route("api/ShippingAddresses")]
        public IQueryable<ShippingAddress> GetShippingAddresses()
        {
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            return db.ShippingAddresses.Where(s => s.UId == Uid);
        }

        // GET: api/ShippingAddresses/5
        // Returns shipping address of user with given shippind Id
        [HttpGet]
        [Route("api/ShippingAddresses/{SHid}")]
        public IHttpActionResult GetShippingAddress(int SHid)
        {
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            ShippingAddress shippingAddress
                = db.ShippingAddresses
                .Where(s => s.UId == Uid && s.ShId == SHid)
                .FirstOrDefault();
            if (shippingAddress == null)
            {
                return NotFound();
            }

            return Ok(shippingAddress);
        }

        // PUT: api/ShippingAddresses/5/1
        // Edit the shipping address of given user
        [HttpPut]
        [Authorize]
        [Route("api/ShippingAddresses/{SHid}/{UserId}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShippingAddress
            (int SHid, int UserId, ShippingAddress shippingAddress)
        {
            
            var identity = (ClaimsIdentity)User.Identity;

            var role = identity.Name;

            int Uid;

            // Admin can edit all users, so UId is taken 
            // directly from request
            if (role == "admin") {
                Uid = UserId;
            }
            // else user can edit only their shipping addresses
            else
            {
                Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            }

                

            if (SHid != shippingAddress.ShId || (Uid != 0 && shippingAddress.UId != Uid))
            {
                return BadRequest();
            }

            db.Entry(shippingAddress).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingAddressExists(SHid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated");
        }

        // POST: api/ShippingAddresses
        // Create new shipping address
        [HttpPost]
        [Route("api/ShippingAddresses")]
        [ResponseType(typeof(ShippingAddress))]
        public IHttpActionResult PostShippingAddress(ShippingAddress shippingAddress)
        {
            db.usp_insert_shipping_address
                (
                    shippingAddress.UId, 
                    shippingAddress.Street, 
                    shippingAddress.City,
                    shippingAddress.State, 
                    shippingAddress.Pincode
                );
            return Ok(true);
        }

        // DELETE: api/ShippingAddresses/5
        // Delete shipping address of a user
        [HttpDelete]
        [Authorize]
        [Route("api/ShippingAddresses/{SHid}")]
        public IHttpActionResult DeleteShippingAddress(int SHid)
        {
            ShippingAddress shippingAddress = db.ShippingAddresses.Find(SHid);
            if (shippingAddress == null)
            {
                return NotFound();
            }

            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            // user can delete only their shipping address
            if (shippingAddress.UId != Uid)
            {
                return BadRequest();
            }

            db.ShippingAddresses.Remove(shippingAddress);
            db.SaveChanges();

            return Ok(shippingAddress);
        }

        // GET : api/ShippingAddresses/user/1
        // Returns shipping address of user
        // Admin Only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/ShippingAddresses/user/{uid}")]
        public List<ShippingAddress> GetShippingAddressesByUId(int uid)
        {
            return db.ShippingAddresses.Where(s => s.UId == uid).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShippingAddressExists(int id)
        {
            return db.ShippingAddresses.Count(e => e.ShId == id) > 0;
        }
    }
}