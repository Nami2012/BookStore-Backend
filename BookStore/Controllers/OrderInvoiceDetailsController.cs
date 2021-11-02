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
    public class OrderInvoiceDetailsController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: api/OrderInvoiceDetails
        public IQueryable<OrderInvoiceDetail> GetOrderInvoiceDetails()
        {
            return db.OrderInvoiceDetails;
        }

        // GET: api/OrderInvoiceDetails/5
        [ResponseType(typeof(OrderInvoiceDetail))]
        public IHttpActionResult GetOrderInvoiceDetail(string id)
        {
            OrderInvoiceDetail orderInvoiceDetail = db.OrderInvoiceDetails.Find(id);
            if (orderInvoiceDetail == null)
            {
                return NotFound();
            }

            return Ok(orderInvoiceDetail);
        }

        // PUT: api/OrderInvoiceDetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrderInvoiceDetail(string id, OrderInvoiceDetail orderInvoiceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderInvoiceDetail.OrderId)
            {
                return BadRequest();
            }

            db.Entry(orderInvoiceDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderInvoiceDetailExists(id))
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

        // POST: api/OrderInvoiceDetails
        [ResponseType(typeof(OrderInvoiceDetail))]
        public IHttpActionResult PostOrderInvoiceDetail(OrderInvoiceDetail orderInvoiceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderInvoiceDetails.Add(orderInvoiceDetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (OrderInvoiceDetailExists(orderInvoiceDetail.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = orderInvoiceDetail.OrderId }, orderInvoiceDetail);
        }

        // DELETE: api/OrderInvoiceDetails/5
        [ResponseType(typeof(OrderInvoiceDetail))]
        public IHttpActionResult DeleteOrderInvoiceDetail(string id)
        {
            OrderInvoiceDetail orderInvoiceDetail = db.OrderInvoiceDetails.Find(id);
            if (orderInvoiceDetail == null)
            {
                return NotFound();
            }

            db.OrderInvoiceDetails.Remove(orderInvoiceDetail);
            db.SaveChanges();

            return Ok(orderInvoiceDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderInvoiceDetailExists(string id)
        {
            return db.OrderInvoiceDetails.Count(e => e.OrderId == id) > 0;
        }
    }
}