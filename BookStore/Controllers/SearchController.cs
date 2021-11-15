using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookStore.Controllers
{
    public class SearchController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();


        // GET : api/Search/Books/Harry
        // GET : api/Search/Books/Harry/1
        // Returns books that matches searchterm
        // Filter by categoryId if present
        [HttpGet]
        [Route("api/Search/Books/{searchterm}/{cid:int=0}")]
        public IHttpActionResult SearchBooks(int cid, string searchterm)
        {
            if (cid == 0)
            {
                return Ok(db.usp_search_by_title($"%{searchterm}%"));
            }
            else
            {
                return Ok(db.usp_search_by_category($"%{searchterm}%", cid));
            }
        }

        // GET : api/Search/Author/Dan
        // Search by Author
        [HttpGet]
        [Route("api/Search/Author/{searchterm}")]
        public IHttpActionResult SearchBookByAuthor(string searchterm)
        {
            return Ok(db.usp_search_by_author($"%{searchterm}%"));
        }

        // GET : api/Search/ISBN/9199
        // Search book by ISBN
        [HttpGet]
        [Route("api/Search/ISBN/{searchterm}")]
        public IHttpActionResult SearchByISBN(string searchterm)
        {
            return Ok(db.usp_search_by_isbn($"%{searchterm}%"));
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
