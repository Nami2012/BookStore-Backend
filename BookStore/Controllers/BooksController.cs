using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BookStore.Models;



namespace BookStore.Controllers
{
    public class BooksController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();


        // GET: api/Books - get all books
        [HttpGet]
        [Route("api/Books")]
        public IQueryable<Book> GetBooks()
        {
            return db.Books;
        }

        // GET: api/Books/5
        [HttpGet]
        [Route("api/Books/{id}")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        //GET Books With Category ID
        [HttpGet]
        [Route("api/BooksByCategory/{cid}")]
        [ResponseType(typeof(List<Book>))]
        public IHttpActionResult GetBooksByCategory(int cid)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var role = identity.Name;
            if (role == "admin")
                return Ok(db.usp_all_books_by_category(cid));


            return Ok(db.usp_books_by_category(cid));
        }



        // PUT: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("api/book/edit")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult PutBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Entry(book).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.BId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(book);
        }

        //PUT:api/Book/edit/ActiveStatus
        [HttpPut]
        [Route("api/Book/edit/ActiveStatus/{id}")]
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ActiveStatus(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return BadRequest();
            }
            book.BStatus = !book.BStatus;
            db.Entry(book).Property(b => b.BStatus).IsModified = true;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // GET: api/image/book/{cid} 
        [Route("api/image/book/{bid}")]
        public HttpResponseMessage GetCategoryImage(int bid)
        {
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);

                string imageName = db.Books
                    .Where( b => b.BId == bid )
                    .Select(b => b.BImage )
                    .FirstOrDefault();

                var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);

                var ext = System.IO.Path.GetExtension(filePath);

                var contents = System.IO.File.ReadAllBytes(filePath);

                System.IO.MemoryStream ms = new System.IO.MemoryStream(contents);

                response.Content = new StreamContent(ms);

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + ext);

                return response;
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }

        }

        // POST: api/Books
        [HttpPost]
        [Route("api/Books")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostBook(Book book)
        {
            int newPosition = db.Books.Max(b => b.BPosition) + 1;

            try
            {
                db.usp_insert_book(book.CId, book.BTitle, 
                    book.BAuthor, book.BISBN, book.BYEAR, 
                    book.BPrice, book.BDescription, newPosition, book.BImage);
            }
            catch (DbUpdateException)
            {
                if (BookExists(book.BId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(book.CId);
        }
        // DELETE: api/Books/5
        [HttpDelete]
        [ResponseType(typeof(Book))]
        [Route("api/book/delete")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            db.SaveChanges();

            return Ok(book);
        }

        [HttpGet]
        [Route("api/Books/GetTopBooks/{cid:int?}")]
        public IHttpActionResult GetFeaturedBooks(int? cid = null)
        {
            if (cid == null)
            {
                return Ok(db.usp_get_top_books(4));
            }
            else
            {
                return Ok(db.usp_get_top_books_by_category(4, cid));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BId == id) > 0;
        }
    }
}