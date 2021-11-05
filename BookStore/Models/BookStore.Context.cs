﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookStore.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BookStoreDBEntities : DbContext
    {
        public BookStoreDBEntities()
            : base("name=BookStoreDBEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Coupon_Validation> Coupon_Validation { get; set; }
        public virtual DbSet<OrderInvoiceDetail> OrderInvoiceDetails { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<User_Account_Info> User_Account_Info { get; set; }
        public virtual DbSet<User_Credentials> User_Credentials { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
    
        public virtual ObjectResult<usp_books_by_category_Result> usp_books_by_category(Nullable<int> cId)
        {
            var cIdParameter = cId.HasValue ?
                new ObjectParameter("CId", cId) :
                new ObjectParameter("CId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_books_by_category_Result>("usp_books_by_category", cIdParameter);
        }
    
        public virtual ObjectResult<usp_get_active_categories_Result> usp_get_active_categories()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_get_active_categories_Result>("usp_get_active_categories");
        }
    
        public virtual ObjectResult<usp_get_categories_Result> usp_get_categories()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_get_categories_Result>("usp_get_categories");
        }
    
        public virtual ObjectResult<usp_get_top_books_Result> usp_get_top_books(Nullable<int> rowCount)
        {
            var rowCountParameter = rowCount.HasValue ?
                new ObjectParameter("rowCount", rowCount) :
                new ObjectParameter("rowCount", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_get_top_books_Result>("usp_get_top_books", rowCountParameter);
        }
    
        public virtual ObjectResult<usp_get_top_books_by_category_Result> usp_get_top_books_by_category(Nullable<int> rowCount, Nullable<int> cId)
        {
            var rowCountParameter = rowCount.HasValue ?
                new ObjectParameter("rowCount", rowCount) :
                new ObjectParameter("rowCount", typeof(int));
    
            var cIdParameter = cId.HasValue ?
                new ObjectParameter("CId", cId) :
                new ObjectParameter("CId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_get_top_books_by_category_Result>("usp_get_top_books_by_category", rowCountParameter, cIdParameter);
        }
    
        public virtual ObjectResult<usp_get_user_details_Result> usp_get_user_details(Nullable<int> userid)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("userid", userid) :
                new ObjectParameter("userid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_get_user_details_Result>("usp_get_user_details", useridParameter);
        }
    }
}
