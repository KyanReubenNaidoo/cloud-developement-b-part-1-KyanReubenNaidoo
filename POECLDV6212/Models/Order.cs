using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace POECLDV6212.Models
{
    public class Order : ITableEntity
    {
        [Key]
        public int Order_ID { get; set; }
        //ITableEntity
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        [Required(ErrorMessage = "Please select a Customer")]
        public int Customer_ID { get; set; }

        [Required(ErrorMessage = "Please select a Product")]
        public int Product_ID { get; set; }

        [Required(ErrorMessage = "Please enter your order description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter your order date")]
        public DateTime OrderDate { get; set; }
    }
}
