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

        /// GET: api/Categories 
        // sorted according to position
        // return only active categories
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


        // GET: api/image/category/{cid} 
        [Route("api/image/category/{cid}")]
        public HttpResponseMessage GetCategoryImage(int cid)
        {
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);

                string imageName = db.Categories
                    .Where(c => c.CId == cid)
                    .Select(c => c.CImage)
                    .FirstOrDefault();

                var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);

                var ext = System.IO.Path.GetExtension(filePath);

                var contents = System.IO.File.ReadAllBytes(filePath);

                System.IO.MemoryStream ms = new System.IO.MemoryStream(contents);

                response.Content = new StreamContent(ms);

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + ext);

                return response;
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
            
        }



        // POST : api/uploadImage
        // Accept image and store it into a folder
        /*[Authorize(Roles = " Admin")]*/
        [Route("api/uploadImage")]
        public IHttpActionResult PostImage()
        {
            string result = "";
            try
            {
                string imageName = null;
                var httpRequest = HttpContext.Current.Request;
                var postedImage = httpRequest.Files["ImageToUpload"];
                if (postedImage != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedImage.FileName)
                        .Take(10)
                        .ToArray())
                        .Replace(" ", "-");
                    imageName = imageName
                        + DateTime.Now.ToString("yymmssfff")
                        + Path.GetExtension(postedImage.FileName);

                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                    postedImage.SaveAs(filePath);
                    result = imageName;
                } else
                {
                    result = "Image upload failed";
                    return InternalServerError(new Exception(result));
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                throw;
            }

            return Ok(result);
        }


        //GET: api/Categories/Admin
        [Authorize(Roles = "Admin")]
        [Route("api/Categories/Admin")]
        public IHttpActionResult GetCategoriesForAdmin() //return all categories
        {
            return Ok(db.usp_get_categories());
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

        
        

        // POST: api/Categories
        // [Authorize(Roles = "Admin")]
        // need to test
        [ResponseType(typeof(Category))]
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
                // Write code to  retrive the created category id
                // return createdat()
            }
            catch (DbUpdateException)
            {
                throw;
            }
            // To return this find way to retrive the newly created category id
            // CreatedAtRoute("DefaultApi", new { id = category.CId }, category
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
