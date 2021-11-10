using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using BookStore.Models;
using System.Security.Claims;

namespace BookStore.Controllers
{   
    public class CategoriesController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/Categories 
        // sorted according to position
        // return only active categories
        [HttpGet]
        [Route("api/Categories")]
        public IHttpActionResult GetCategories()
        {   
            var identity= (ClaimsIdentity)User.Identity;
         
            var role = identity.Name;
                if (role == "admin")
                    return Ok(db.usp_get_categories());
           

            return Ok(db.usp_get_active_categories());
        }

        // GET: api/Categories/5 
        [HttpGet]
        [Route("api/category/{id}")]
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

        //GET: api/Category/Names
        [HttpGet]
        [Route("api/Category/Names")]
        public IQueryable<string> GetCategoryNamesAndID() //id
        {
            List<string> CategoryNames = new List<string>();
            foreach (var category in db.Categories)
            {
                CategoryNames.Add(category.CName);
            }
            return CategoryNames.AsQueryable();
        }

        //PUT: api/Category/Edit/ActiveStatus/{bid}
        [Authorize(Roles = "Admin")]
        [HttpPut] 
        [Route("api/Category/Edit/ActiveStatus/{bid}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(int bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Category category = db.Categories.Find(bid);
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
                if (!CategoryExists(bid))
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
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("api/Category/edit")]
        [ResponseType(typeof(Category))]    
        public IHttpActionResult PutCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CId))
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
        [HttpPost]
        [Route("api/Categories")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult PostCategory(Category category)
        {
            category.CId = db.Categories.Max(c => c.CId) + 1;
            category.CPosition = db.Categories.Max(c => c.CPosition) + 1;
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
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("api/category/delete")]
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