using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace POECLDV6212.Models
{
    public class Product : ITableEntity
    {
        [Key]
        public int Product_ID { get; set; }
        public string? Product_Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }

        //ITableEntity
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}