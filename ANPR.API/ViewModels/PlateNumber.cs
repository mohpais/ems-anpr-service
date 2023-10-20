using Microsoft.Lonsum.Services.ANPR.API.Models;

namespace Microsoft.Lonsum.Services.ANPR.API.ViewModels
{
    public class PlateNumber
    {
        public EventNotificationAlert Data { get; set; }
        public Image Images { get; set; }
    }

    public class FileUpload
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Threshold { get; set; }
        public string Plate { get; set; }
        public IFormFile File { get; set; }
    }

    public class SendFileViaRabbitMq
    {
        public string PlateNumber { get; set; }
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Threshold { get; set; }
    }
}
