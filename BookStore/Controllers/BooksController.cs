using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using BookStore.Models;



namespace BookStore.Controllers
{
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
           return Ok(db.usp_books_by_category(cid));         
        }



        //Search books 
        [Route("api/Search/Books/{searchterm}/{cid:int=0}")]
        [HttpGet]
        public IHttpActionResult SearchBooks(int cid, string searchterm)
        {
            if(cid == 0)
            {
                //can build where clause separately
                return Ok(db.usp_search_by_title($"%{searchterm}%"));
            }
            else
            {
                return Ok(db.usp_search_by_category($"%{searchterm}%", cid));
            }
        }

        // Search by Author
        [Route("api/Search/Author/{searchterm}")]
        [HttpGet]
        public IHttpActionResult SearchBookByAuthor(string searchterm)
        {
            return Ok(db.usp_search_by_author($"%{searchterm}%"));
        }

        [Route("api/Search/ISBN/{searchterm}")]
        [HttpGet]
        public IHttpActionResult SearchByISBN(string searchterm)
        {
            return Ok(db.usp_search_by_isbn($"%{searchterm}%"));
        }

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
        [Route("api/Book/edit/ActiveStatus/{id}")]
        [ResponseType(typeof(void))]
        // [Authorize(Roles = "Admin")]
        public IHttpActionResult ActiveStatus(int id)
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
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/

            book.BStatus = true;
            book.BPosition = db.Books.Max(b => b.BPosition) + 1;
            book.BImage = "https://harpercollins.co.in/PowerPoint_jpg/9789354223174.jpg";
            book.BId = db.Books.Max(b => b.BId) + 1;

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