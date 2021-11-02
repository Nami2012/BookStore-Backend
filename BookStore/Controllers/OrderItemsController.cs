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
    public class OrderItemsController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: api/OrderItems
        public IQueryable<OrderItem> GetOrderItems()
        {
            return db.OrderItems;
        }

        // GET: api/OrderItems/5
        [ResponseType(typeof(OrderItem))]
        public IHttpActionResult GetOrderItem(string id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        // PUT: api/OrderItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrderItem(string id, OrderItem orderItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderItem.OrderId)
            {
                return BadRequest();
            }

            db.Entry(orderItem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
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

        // POST: api/OrderItems
        [ResponseType(typeof(OrderItem))]
        public IHttpActionResult PostOrderItem(OrderItem orderItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderItems.Add(orderItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (OrderItemExists(orderItem.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = orderItem.OrderId }, orderItem);
        }

        // DELETE: api/OrderItems/5
        [ResponseType(typeof(OrderItem))]
        public IHttpActionResult DeleteOrderItem(string id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            db.OrderItems.Remove(orderItem);
            db.SaveChanges();

            return Ok(orderItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderItemExists(string id)
        {
            return db.OrderItems.Count(e => e.OrderId == id) > 0;
        }
    }
}