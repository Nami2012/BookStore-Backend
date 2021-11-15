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


/// <summary>
/// Books Controller
/// Containes API end-points related to Books in the BookStore
/// </summary>
namespace BookStore.Controllers
{
    public class BooksController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();


        // GET: api/Books
        // Returns list of all activated and deactivated books
        // Admin Only
        [HttpGet]
        [Route("api/Books")]
        [Authorize(Roles = "Admin")]
        public IQueryable<Book> GetBooks()
        {
            return db.Books;
        }

        // GET: api/Books/5
        // Return Book with given Book Id
        [HttpGet]
        [Route("api/Books/{id}")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBook(int bid)
        {
            Book book = db.Books.Find(bid);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // GET: api/BooksByCategory/1
        // Returns list of books with given Category Id
        // Returns all books for Admin 
        // Returns active books else
        [HttpGet]
        [Route("api/BooksByCategory/{cid}")]
        [ResponseType(typeof(List<Book>))]
        public IHttpActionResult GetBooksByCategory(int cid)
        {
            // Get the role assosiated with the request
            var identity = (ClaimsIdentity)User.Identity;
            var role = identity.Name;

            // Return all books for admin
            if (role == "admin")
                return Ok(db.usp_all_books_by_category(cid));

            // Returns active books for others
            return Ok(db.usp_books_by_category(cid));
        }



        // PUT: api/book/edit
        // Book object is retrived from body
        // Edit book details
        // Admin only
        [HttpPut]
        [Authorize(Roles = "Admin")]
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

        // PUT: api/Book/edit/ActiveStatus/1
        // Active / Deactivate a book
        // Changes the active status of book with given BId
        // Admin only
        [HttpPut]
        [Route("api/Book/edit/ActiveStatus/{bid}")]
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ActiveStatus(int bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book book = db.Books.Find(bid);

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
                if (!BookExists(bid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.OK);
        }

        // GET: api/image/book/1
        // Returns the cover image of the book with given BId
        [HttpGet]
        [Route("api/image/book/{bid}")]
        public HttpResponseMessage GetCoverImage(int bid)
        {
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);

                // Get the image filename from DB
                string imageName = db.Books
                    .Where( b => b.BId == bid )
                    .Select(b => b.BImage )
                    .FirstOrDefault();

                // Create the full file path and extension
                var filePath = HttpContext.Current
                    .Server.MapPath("~/Images/" + imageName);
                var ext = System.IO.Path.GetExtension(filePath);

                // Fetch the file
                var contents = System.IO.File.ReadAllBytes(filePath);
                System.IO.MemoryStream ms
                    = new System.IO.MemoryStream(contents);

                // Setup the response
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType 
                    = new System.Net.Http
                    .Headers.MediaTypeHeaderValue("image/" + ext);

                return response;
            }
            catch (Exception ex)
            {

                return Request
                    .CreateErrorResponse(HttpStatusCode.NotFound,
                    ex.Message);
            }

        }

        // POST: api/Books
        // Insert new book
        // Admin Only
        [HttpPost]
        [Route("api/Books")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostBook(Book book)
        {
            // Updating the position of newly added book
            int newPosition = db.Books.Max(b => b.BPosition) + 1;

            try
            {
                db.usp_insert_book(book.CId, book.BTitle, 
                    book.BAuthor, book.BISBN, book.BYEAR, 
                    book.BPrice, book.BDescription, 
                    newPosition, book.BImage);
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
        // Delete an existing book
        // Admin only
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


        // GET : api/Books/GetTopBooks/1
        // GET : api/Books/GetTopBooks
        // Returns the top books 
        // Returns the top books in category with given CId
        [HttpGet]
        [Route("api/Books/GetTopBooks/{cid:int?}")]
        public IHttpActionResult GetFeaturedBooks(int? cid = null)
        {
            // If no category is specified, return top books
            // If category is specified, return top books of that category
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