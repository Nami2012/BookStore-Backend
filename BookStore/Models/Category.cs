//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Books = new HashSet<Book>();
        }
        
        public int CId { get; set; }
        public string CName { get; set; }
        public string CDescription { get; set; }
        public string CImage { get; set; }
        public bool CStatus { get; set; }
        public int CPosition { get; set; }
        public System.DateTime CCreatedAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public  ICollection<Book> Books { get; set; }
    }
}
