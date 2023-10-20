namespace Microsoft.Lonsum.Services.ANPR.API.ViewModels
{
    public record CreateRecognizenEventDTO
    {
        public string OriginalLicensePlate { get; set; }
        public string PlateNumber { get; set; }
        public string PlateColor { get; set; }
        public string VehicleType { get; set; }
        public string VehicleColor { get; set; }
        public DateTime CaptureDate { get; set; }
        public IFormFile File { get; set; }
    }
}
