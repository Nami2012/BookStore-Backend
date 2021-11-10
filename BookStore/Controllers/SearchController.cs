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
        //Search books 
        [HttpGet]
        [Route("api/Search/Books/{searchterm}/{cid:int=0}")]
        public IHttpActionResult SearchBooks(int cid, string searchterm)
        {
            if (cid == 0)
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
