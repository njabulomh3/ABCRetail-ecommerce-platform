using System.ComponentModel.DataAnnotations;

namespace ABCRetail_Cloud_.Models
{
    public class CustomerSql
    {
        [Key]
        public int Id { get; set; }                      // PK
        [Required, MaxLength(150)]
        public string FullName { get; set; }
        [Required, MaxLength(256)]
        public string Email { get; set; }
        [MaxLength(30)]
        public string Phone { get; set; }
        public string Address { get; set; }

        // Optionally store reference to Table storage entity id if needed
        public string TablePartitionKey { get; set; }
        public string TableRowKey { get; set; }
    }
}

