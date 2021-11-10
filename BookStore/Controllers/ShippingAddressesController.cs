﻿using System;
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
        [HttpGet]
        [Route("api/ShippingAddresses")]
        [Authorize]
        public IQueryable<ShippingAddress> GetShippingAddresses()
        {
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            return db.ShippingAddresses.Where(s=>s.UId==Uid);
        }

        // GET: api/ShippingAddresses/5
        [HttpGet]
        [Route("api/ShippingAddresses/{SHid}")]
        [ResponseType(typeof(ShippingAddress))]
        public IHttpActionResult GetShippingAddress(int SHid)
        {
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );
            ShippingAddress shippingAddress = db.ShippingAddresses.Where(s=>s.UId==Uid && s.ShId==SHid).First();
            if (shippingAddress == null)
            {
                return NotFound();
            }

            return Ok(shippingAddress);
        }

        // PUT: api/ShippingAddresses/5
        [HttpPut]
        [Route("api/ShippingAddresses/{SHid}")]
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShippingAddress(int SHid, ShippingAddress shippingAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );

            if (SHid != shippingAddress.ShId || (Uid!=0 && shippingAddress.UId!=Uid))
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ShippingAddresses
        [HttpPost]
        [Authorize]
        [Route("api/ShippingAddresses")]
        [ResponseType(typeof(ShippingAddress))]
        public IHttpActionResult PostShippingAddress(ShippingAddress shippingAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = (ClaimsIdentity)User.Identity;
            int Uid = int.Parse(
                        identity.Claims.Where(c => c.Type == "UId")
                        .Select(c => c.Value).FirstOrDefault()
                    );

            db.usp_insert_shipping_address(Uid, shippingAddress.Street, shippingAddress.City,
                shippingAddress.State, shippingAddress.Pincode);
            return Ok(true);
        }

        // DELETE: api/ShippingAddresses/5
        [HttpDelete]
        [Authorize]
        [Route("api/ShippingAddresses/{SHid}")]
        [ResponseType(typeof(ShippingAddress))]
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
            if(shippingAddress.UId != Uid)
            {
                return BadRequest();
            }

            db.ShippingAddresses.Remove(shippingAddress);
            db.SaveChanges();

            return Ok(shippingAddress);
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