namespace FileUploaderAPI.Models
{
    public class UploadURLToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public bool IsExpired { get; set; }
    }
}
