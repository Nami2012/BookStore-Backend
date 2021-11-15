using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BookStore.Models;


namespace BookStore.Controllers
{   
    public class CategoriesController : ApiController
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: api/Categories 
        // Sorted according to position of category
        // Returns only active categories
        [HttpGet]
        [Route("api/Categories")]
        public IHttpActionResult GetCategories()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var role = identity.Name;
            if (role == "admin")
                return Ok(db.usp_get_categories());


            return Ok(db.usp_get_active_categories());
        }

        // GET: api/Category/Names
        // Returns List of Active Catgeory Names
        [HttpGet]
        [Route("api/Category/Names")]
        public IQueryable<string> GetCategoryNames()
        {
            List<string> CategoryNames = new List<string>();
            foreach (var category in db.Categories.Where(c => c.CStatus == true))
            {
                CategoryNames.Add(category.CName);
            }
            return CategoryNames.AsQueryable();
        }

        // GET: api/category/5 
        // Returns information related to category with given CId
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


        // PUT: api/Category/Edit/ActiveStatus/1
        // Change the ActiveStatus for a category
        // Activate and deactivate category
        // Admin Only
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("api/Category/Edit/ActiveStatus/{cid}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(int cid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Category category = db.Categories.Find(cid);
            if (category == null)
            {
                return BadRequest();
            }
            category.CStatus = !category.CStatus;
            db.Entry(category).Property(c => c.CStatus).IsModified = true;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(cid))
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

        // PUT: api/Category/edit
        // Edit Category
        // Admin Only
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

        // DELETE: api/category/delete/1
        // Delete category with given CId
        // Admin Only
        [HttpDelete]
        [Authorize(Roles = "Admin")]
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


        // GET: api/image/category/1
        // Get the Category Image with given CId
        [Route("api/image/category/{cid}")]
        public HttpResponseMessage GetCategoryImage(int cid)
        {
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);

                // Get the image filename from DB
                string imageName = db.Categories
                    .Where(c => c.CId == cid)
                    .Select(c => c.CImage)
                    .FirstOrDefault();

                // Create the full file path and extension
                var filePath = HttpContext.Current
                    .Server.MapPath("~/Images/" + imageName);
                var ext = System.IO.Path.GetExtension(filePath);

                // Fetch the file
                var contents = System.IO.File.ReadAllBytes(filePath);
                System.IO.MemoryStream ms 
                    = new System.IO.MemoryStream(contents);

                // Setup response
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



        // POST : api/uploadImage
        // Upload category and book image to the server
        // Admin Only
        [Authorize(Roles = " Admin")]
        [Route("api/uploadImage")]
        public IHttpActionResult PostImage()
        {
            string result = "";
            try
            {
                string imageName = null;

                // Get the image file from request
                var httpRequest = HttpContext.Current.Request;
                var postedImage = httpRequest.Files["ImageToUpload"];

                // Save the image to the server
                if (postedImage != null)
                {

                    // Create unique filename
                    imageName = new String(Path.GetFileNameWithoutExtension(postedImage.FileName)
                        .Take(10)
                        .ToArray())
                        .Replace(" ", "-");
                    imageName = imageName
                        + DateTime.Now.ToString("yymmssfff")
                        + Path.GetExtension(postedImage.FileName);

                    // Saving image to Images folder
                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                    postedImage.SaveAs(filePath);

                    // Image name is returned
                    result = imageName;
                } else
                {
                    result = "Image upload failed";
                    return InternalServerError(new Exception(result));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok(result);
        }


        // GET: api/Categories/Admin
        // Returns all (active and non active) categories
        // Admin only
        [Authorize(Roles = "Admin")]
        [Route("api/Categories/Admin")]
        public IHttpActionResult GetCategoriesForAdmin()
        {
            return Ok(db.usp_get_categories());
        }     
        

        // POST: api/Categories
        // Create new category
        // Admin Only
        [Authorize(Roles = "Admin")]
        [Route("api/categories")]
        public IHttpActionResult PostCategory(Category category)
        {
            int newCPosition = db.Categories.Max(c => c.CPosition) + 1;
            
            DateTime CCreatedAt = DateTime.Now;

            db.Categories.Add(category);

            try
            {
                db.usp_insert_category
                    (category.CName, category.CDescription, 
                    category.CImage, newCPosition, CCreatedAt);
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok("Category Added");
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
