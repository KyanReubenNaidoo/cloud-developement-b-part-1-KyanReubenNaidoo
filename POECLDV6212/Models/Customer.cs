using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace POECLDV6212.Models
{
    public class Customer : ITableEntity
    {
        [Key]
        public int Customer_ID { get; set; }
        public string? Full_Name { get; set; }
        public required string Address { get; set; }
        public string? Phone_Number { get; set; }
        public string? Email { get; set; }

        //ITableEntity
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
