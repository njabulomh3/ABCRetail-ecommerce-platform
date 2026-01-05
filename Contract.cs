
using Azure;
using Azure.Data.Tables;
using System;

namespace ABCRetail_Cloud_.Models
{
    public class Contract : ITableEntity
    {
        // Existing properties
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        // Calculated property for display size
        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                    return $"{Size / 1024 / 1024} MB";
                if (Size >= 1024)
                    return $"{Size / 1024} KB";
                return $"{Size} Bytes";
            }
        }

        // ITableEntity implementation
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }


    }
}

