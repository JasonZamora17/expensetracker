using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        public string Borrower { get; set; }
        public string Lender { get; set; }

    }
}