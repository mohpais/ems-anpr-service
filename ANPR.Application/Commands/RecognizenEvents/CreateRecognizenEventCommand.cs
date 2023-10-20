using MediatR;
using System.Runtime.Serialization;

namespace Microsoft.Lonsum.Services.ANPR.Application.Commands
{
    [DataContract]
    public class CreateRecognizenEventCommand
        : IRequest<bool>
    {
        [DataMember]
        public string OriginalLicensePlate { get; set; }
        [DataMember]
        public string PlateNumber { get; set; }
        [DataMember]
        public string PlateColor { get; set; }
        [DataMember]
        public string VehicleType { get; set; }
        [DataMember]
        public string VehicleColor { get; set; }
        [DataMember]
        public string PlateImagePath { get; set; }
        [DataMember]
        public string EmpCode { get; set; }
        [DataMember]
        public DateTime CaptureDate { get; set; }
        public CreateRecognizenEventCommand()
        { }
        public CreateRecognizenEventCommand(string originalPlate, string plateNumber, string plateColor, string vehicleType, string vehicleColor, string plateImagePath, string empCode, DateTime captureDate)
        {
            OriginalLicensePlate = originalPlate;
            PlateNumber = plateNumber;
            PlateColor = plateColor;
            VehicleType = vehicleType;
            VehicleColor = vehicleColor;
            PlateImagePath = plateImagePath;
            EmpCode = empCode;
            CaptureDate = captureDate;
        }
    }
}
