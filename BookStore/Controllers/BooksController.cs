using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Security;
using BookStore.Models;
//using System.Security.Claims;


// Available Endpoints :
// Get All Books : GetBooks() : GET api/Books
// Get Book using BId : GetBook(int id) : GET api/Books/{id}
// Get Books using CId : GetBooksByCategory(int cid) : GET api/Book
// Insert Book : PostBook(Book book) : POST api/BooksByCategory/{cid}
// Get Featured / Top books of each category
//          : GetFeaturedBooks(int? cid) : GET GetFeaturedBooks(int? cid)


namespace BookStore.Controllers
{
    //[EnableCors("*", "*", "*")]
    public class BooksController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();


        // GET: api/Books
        public IQueryable<Book> GetBooks()
        {
            return db.Books;
        }

        // GET: api/Books/5
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
        [Route("api/BooksByCategory/{cid}")]
        [ResponseType(typeof(List<Book>))]
        public IHttpActionResult GetBooksByCategory(int cid)
        {

            IQueryable<Book> Books = db.Books
                .Where(b => b.CId == cid).AsQueryable();
             return Ok(Books);
           // return Ok(db.usp_books_by_category(cid));            
            
        }

       

        /*//Search books 
        [Route("api/Search/Books/")]
        [HttpGet]
        public IQueryable<Book> SearchBooks(int id,string searchterm)
        {   
            
            //can build where clause separately
            IQueryable<Book> Books = db.Books.Where(b => b.BTitle.Contains(searchterm)).AsQueryable();
            return Books;


        }*/

        /*// PUT: api/Books/5
        [ResponseType(typeof(void))]
        // [Authorize(Roles = "Admin")]
        public IHttpActionResult PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BId)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

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
        }*/

        /*//PUT:api/Book/edit/ActiveStatus
        // But why?
        // Why PutCategory inside BooksController ?
        [Route("api/Book/edit/ActiveStatus/{id}")]
        [ResponseType(typeof(void))]
        // [Authorize(Roles = "Admin")]
        public IHttpActionResult PutBookStatus(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Book book = db.Books.Find(id);
            if(book == null)   
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
        }*/

        // POST: api/Books
        [ResponseType(typeof(Book))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book.BStatus = true;
            book.BPosition = 1;
            book.BImage = "https://harpercollins.co.in/PowerPoint_jpg/9789354223174.jpg";

            db.Books.Add(book);

            try
            {
                db.SaveChanges();
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

            return CreatedAtRoute("DefaultApi", new { id = book.BId }, book);
        }

        /*// DELETE: api/Books/5
        [ResponseType(typeof(Book))]

       // [Authorize(Roles = "Admin")]
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
        }*/

        [HttpGet]
        [Route("api/Books/GetTopBooks/{cid:int?}")]
        public IHttpActionResult GetFeaturedBooks(int? cid = null)
        {
            if(cid == null)
            {
                var books = db.Books.Where(b => b.BStatus == true)
                    .OrderBy(b => b.BPosition)
                    .Take(5)
                    .Select(b => new {
                        b.BId,
                        b.CId,
                        b.BTitle,
                        b.BAuthor,
                        b.BDescription,
                        b.BImage
                    });
                return Ok(books);
            }
            else
            {
                var books = db.Books
                    .Where(b => b.CId == cid && b.BStatus == true)
                    .OrderBy(b => b.BPosition)
                    .Take(5)
                    .Select(b => new {
                        b.BId,
                        b.CId,
                        b.BTitle,
                        b.BAuthor,
                        b.BDescription,
                        b.BImage
                    });
                return Ok(books);
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