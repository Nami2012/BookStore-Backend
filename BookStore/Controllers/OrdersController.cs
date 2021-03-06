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
    [Authorize]
    public class OrdersController : ApiController
    {
        private readonly Random random = new Random();
        private BookStoreDBEntities db = new BookStoreDBEntities();

 
        // POST : api/PlaceOrder
        // Create a new order
        [HttpPost]
        [Route("api/PlaceOrder")]
        public IHttpActionResult PlaceOrder()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int orderid=0;

            do
            {
            // generate random order id
             orderid = random.Next(10000); 
            } while (db.OrderInvoiceDetails.Find(orderid.ToString()) != null);
            //check if the object with the same random number doesnt exist

            // insert it  into order invoice
            var identity = (ClaimsIdentity)User.Identity;
            int userid = int.Parse(identity.Name);
            User_Account_Info user = db.User_Account_Info
                .SingleOrDefault(u => u.UId == userid);
            if (user == null)
            {
                return NotFound();
            }
            OrderInvoiceDetail orderInvoice = new OrderInvoiceDetail()
            {
                OrderId = orderid.ToString(),
                UId = user.UId,
                ShippingAddress = user.ShippingAddress,
                Amount = -1
            };
            db.OrderInvoiceDetails.Add(orderInvoice);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (orderInvoiceDetailExists(orderInvoice.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            // get all books from cart
            var books = db.Carts.Where(c => c.UId == user.UId)
                .Select( b => new { b.BId, b.Count }).ToList();

            // insert all books into order items with order id
            // generated from order invoice
            foreach (var book in books)
            {
                OrderItem orderItem = new OrderItem()
                {
                    OrderId = orderid.ToString(),
                    BId = book.BId,
                    COUNT = book.Count
                };
                db.OrderItems.Add(orderItem);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (orderItemExists(orderInvoice.OrderId,book.BId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // return all books from order items
            List<OrderItem> placedOrders 
                = db.OrderItems.Where(o => o.OrderId == orderid.ToString())
                .ToList();

            return Ok(orderid.ToString()); 
        }

        // return percentage of total value to be reduced
        // and adds its to the coupon validation table
        private decimal applyCoupon(string couponid, int userid)
        {
            Coupon_Validation coupon 
                = db.Coupon_Validation.SingleOrDefault
                (c => c.CouponId == couponid && c.UId == userid);
            
            // if coupon already applied
            if (coupon != null)
            {
                return 0; //raise exception if time permits
            }
            // insert applied coupon details into coupon
            // validation table so that the user wont use it again
            coupon = new Coupon_Validation()
            {
                CouponId = couponid,
                UId = userid,
                CouponValid = true //ambiguous
            };
            db.Coupon_Validation.Add(coupon);
            db.SaveChanges();
            // task add to coupon validation
            Coupon applied_coupon = db.Coupons
                .SingleOrDefault(c => c.CouponId == couponid);
            return applied_coupon.Discount;
        }

        // calculate total amount from given orderid
        private decimal CalcTotalAmount(string orderid)
        {
            var orderItems = db.OrderItems
                .Where(o => o.OrderId == orderid).ToList();
            decimal Amount = 0;
            foreach(var item in orderItems)
            {
                Book book = db.Books.Find(item.BId);
                Amount += (decimal)(book.BPrice * item.COUNT);
            }

            return Amount;
        }

        // remove from cart once order is confirmed
        void RemoveFromCart(int uid)
        {
            List<Cart> cartitems = db.Carts
                .Where(c => c.UId == uid).ToList();
            db.Carts.RemoveRange(cartitems);
            db.SaveChanges();
        }

        // remove from wishlist once order is confirmed
        void RemoveFromWishlist(int userid,string orderid)
        {
            // inner join orderitems from orderid
            // and wishlist having userid and remove them
            List<OrderItem> orderitem = db.OrderItems
                .Where(o => o.OrderId==orderid).ToList();
            foreach(var item in orderitem)
            {
                Wishlist wishlistitem 
                    = db.Wishlists.SingleOrDefault
                    (w => w.UId == userid && w.BId == item.BId);
                if (wishlistitem!=null)
                {
                    db.Wishlists.Remove(wishlistitem);
                    db.SaveChanges();
                }
            }
        }

        // POST : api/ConfirmOrder
        // Confirm order by user
        [HttpPost]
        [Route("api/ConfirmOrder")]
        public IHttpActionResult ConfirmOrder
            (string orderid, string shId, string couponid = "")
        {   
            // get user
            var identity = User.Identity.Name;
            User_Account_Info user 
                = db.User_Account_Info.Find(int.Parse(identity));
            // if order id does not exist
            if(user == null || db.OrderInvoiceDetails.Find(orderid)== null)
            {
                return BadRequest();
            }

            // check validity of applied coupon if any
            decimal discount = 0;
            if (couponid != "")
            {
                discount = applyCoupon(couponid,user.UId);
            }
            
            // calculate total amount
            decimal total_amount = CalcTotalAmount(orderid);
            // reduce discount if any
            total_amount = total_amount - ((discount / 100) * total_amount);

            // update total amount in order invoice details
            OrderInvoiceDetail order = db.OrderInvoiceDetails.Find(orderid);
            order.Amount = total_amount;
            order.ShippingAddress = shId;
            db.Entry(order).Property(b => b.Amount).IsModified = true;
            db.SaveChanges();

            // remove order items from cart and wishlist
            RemoveFromCart(user.UId);
            RemoveFromWishlist(user.UId, order.OrderId); 

            // Returns order id to reroute to order post
            return Ok(orderid); 
        }

        // POST : api/CancelOrder
        // cancel order
        // order id as paramter
        [HttpPost]
        [Route("api/CancelOrder")]
        public IHttpActionResult CancelOrder(string orderid)
        {
            var identity = (ClaimsIdentity)User.Identity;

            OrderInvoiceDetail order = db.OrderInvoiceDetails.Find(orderid);

            if (identity == null || order ==null)
            {
                return BadRequest("Operation Not permitted");
            }
           
            if(identity.RoleClaimType=="User" 
                && order.UId!= int.Parse(identity.Name))
            {
                return BadRequest("Unauthorised");
            }

            // remove order id from order items
            List<OrderItem> orderItems = db.OrderItems
                .Where(o => o.OrderId == orderid).ToList();
            db.OrderItems.RemoveRange(orderItems);
            db.SaveChanges();

            // remove order id from order invoice details
            db.OrderInvoiceDetails.Remove(order);
            db.SaveChanges();

            return Ok();
        }

        // GET : api/AllOrder
        // Return All orders of a user
        [HttpGet]
        [Route("api/AllOrders")]
        public IHttpActionResult AllOrders()
        {
            var identity = (ClaimsIdentity)User.Identity;
            int uid = int.Parse(
                         identity.Claims.Where(c => c.Type == "UId")
                         .Select(c => c.Value).FirstOrDefault()
                     );
            var orders = db.OrderInvoiceDetails.ToList().AsQueryable();
            if(uid != 0)
            {
                // Returns only confirmed orders
                // Orders with amount = -1 are
                // those which were not confirmed by user
                orders = orders.Where(o => o.UId == uid && o.Amount != -1);
            }

            return Ok(orders);
        }

        // POST : api/OrderDetails
        // Returns order list details of an order
        // Take in order id and returns the details of the order.
        [HttpGet]
        [Route("api/OrderDetails/")]
        public IHttpActionResult OrderDetails(string orderid)
        {
            var identity = (ClaimsIdentity)User.Identity;
            int uid = int.Parse(
                         identity.Claims.Where(c => c.Type == "UId")
                         .Select(c => c.Value).FirstOrDefault()
                     );
            var order = db.OrderInvoiceDetails.Include("OrderItems.Book")
                .Where(o => o.OrderId == orderid).ToList().AsQueryable();
            if(uid != 0)
            {
                order = order.Where(o => o.UId == uid);
            }
            if (identity == null || order == null)
            {
                return BadRequest("Operation Not permitted");
            }

            return Ok(order);
        }

        private bool orderInvoiceDetailExists(string id)
        {
            return db.OrderInvoiceDetails.Count(e => e.OrderId == id) > 0;
        }
        private bool orderItemExists(string orderid,int bid)
        {
            return db.OrderItems
                .Count(e => e.OrderId == orderid&& e.BId==bid) > 0;
        }
    }
}
