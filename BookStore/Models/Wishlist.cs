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
    
    public partial class Wishlist
    {
        public int UId { get; set; }
        public int BId { get; set; }
        public Nullable<int> Count { get; set; }
    
        public  Book Book { get; set; }
        public  User_Credentials User_Credentials { get; set; }
    }
}
