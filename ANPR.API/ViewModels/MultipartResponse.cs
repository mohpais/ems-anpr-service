namespace Microsoft.Lonsum.Services.ANPR.API.ViewModels
{
    public class MultipartResponse
    {
        public string XmlData { get; set; }
        public Image Images { get; set; }
    }

    public class Image
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
    }
}
