namespace N06_LibrarySystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.LoanDetail = new HashSet<LoanDetail>();
        }
    
        public int BookID { get; set; }
        public string Title { get; set; }
        public int CategoryID { get; set; }
        public int AuthorID { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual Author Author { get; set; }
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoanDetail> LoanDetail { get; set; }
    }
}
