﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using BookStore.filters;
using BookStore.Models;
//using System.Security.Claims;

namespace BookStore.Controllers
{
    public class BooksController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();


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
        [Route("api/Category/Books/")]
        public IQueryable<Book> GetBooksByCategory(int cid)
        {   
            
            IQueryable<Book> Books = db.Books.Where(b => b.CId == cid).AsQueryable();
             return Books;
            
            
        }

        //Search books 
        [Route("api/Search/Books/")]
        [HttpGet]
        public IQueryable<Book> SearchBooks(string searchterm)
        {   
            
            //can build where clause separately
            IQueryable<Book> Books = db.Books.Where(b => b.BTitle.Contains(searchterm)).AsQueryable();
            return Books;


        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
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
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        [AuthenticationFilter]
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