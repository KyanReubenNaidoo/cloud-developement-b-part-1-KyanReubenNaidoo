namespace POECLDV6212.Models
{
    public class FileModel
    {
        public required string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                {
                    return $"{Size / 1024 / 1024} MB";
                }
                else if (Size >= 1024)
                {
                    return $"{Size / 1024} KB";
                }
                return $"{Size} Bytes";
            }
        }
    }
}