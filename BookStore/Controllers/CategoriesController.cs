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
using BookStore.filters;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class CategoriesController : ApiController
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: api/Categories
        public IQueryable<Category> GetCategories() //return only active categories
        {
            return db.Categories;
        }
        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult GetCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [Route("api/Category/Names")]
        public IQueryable<string> GetCategoryNames()
        {
            List<string> CategoryNames = new List<string>();
            foreach(var category in db.Categories)
            {
                CategoryNames.Add(category.CName);
            }
            return CategoryNames.AsQueryable();
        }

        [ResponseType(typeof(void))]
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        [Route("api/Category/Edit/ActiveStatus/{id}")]
        public IHttpActionResult PutCategory(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Category category = db.Categories.Find(id);
            if (category==null)
            {
                return BadRequest();
            }
            category.CStatus = !category.CStatus;
            db.Entry(category).Property(c=>c.CStatus).IsModified = true;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.CId)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(category);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CategoryExists(category.CId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = category.CId }, category);
        }

        // DELETE: api/Categories/5
        [AuthenticationFilter]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CId == id) > 0;
        }
    }
}