using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BookStore.Models;

// Available Endpoints
// Get all active categories : GetCategories : GET api/Categories
// Get all categories (admin only) : GetCategoriesForAdmin() : GET api/Categories/Admin
// Create new Category : PostCategory(Category category) : POST: api/Categories

namespace BookStore.Controllers
{   
    public class CategoriesController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/Categories 
        // sorted according to position
        // return only active categories
        public IHttpActionResult GetCategories()
        {
            var categories = db.Categories.Where(category => category.CStatus == true)
                .Select(c => new { 
                    c.CId, 
                    c.CName, 
                    c.CDescription, 
                    c.CImage, 
                    c.CStatus, 
                    c.CPosition, 
                    c.CCreatedAt 
                });
            categories = categories.OrderBy(category => category.CPosition);
            return Ok(categories);
        }
        // Return includes books details too
        /*public IQueryable<Category> GetCategories() 
        {
            IQueryable<Category> categories = db.Categories.Where(category => category.CStatus == true);
            categories = categories.OrderBy(category => category.CPosition);
            return categories;
        }*/




        //GET: api/Categories/Admin
        // Return includes books details too
        // TODO: Remove the Book Details from GetCategories
        [Authorize(Roles = "Admin")]
        [Route("api/Categories/Admin")]
        public IQueryable<Category> GetCategoriesForAdmin() //return all categories
        {
            IQueryable<Category> categories = db.Categories;

            categories = categories.OrderBy(category => category.CPosition);

            return categories;
        }



        /*// GET: api/Categories/5
        // Commented out since currenly we aren't using this endpoint
        [ResponseType(typeof(Category))]
        public IHttpActionResult GetCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }*/


        // Working perfectly
        // Commented out since currenly we aren't using this endpoint
        /*[Route("api/Category/Names")]
        public IQueryable<string> GetCategoryNamesAndID() //id
        {
            List<string> CategoryNames = new List<string>();
            foreach(var category in db.Categories)
            {
                CategoryNames.Add(category.CName);
            }
            return CategoryNames.AsQueryable();
        }*/

        // Frontend isn't ready for this endpoint
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        [HttpPut] // Test whether this will work without [HttpPut]
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

        // Frontend isn't ready for this endpoint
        // PUT: api/Categories/5
        [ResponseType(typeof(void))]    
        //[Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            category.CPosition = 1;
            category.CImage = "https://static.wikia.nocookie.net/harrypotter/images/d/d4/LibraryPottermore.png";
            category.CStatus = true;
            category.CCreatedAt = DateTime.Now;

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

        //[Authorize(Roles = "Admin")]
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