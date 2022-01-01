namespace FileUploaderAPI.Models
{
    public class UploadedFileMetadata
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }   
        public long Size { get; set; }
    }
}
