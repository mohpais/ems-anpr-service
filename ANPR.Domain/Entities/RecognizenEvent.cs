using Microsoft.Lonsum.Services.ANPR.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Lonsum.Services.ANPR.Domain.Entities
{
    public sealed class RecognizenEvent : BaseEntity
    {
        public string EventType { get; private set; }
        public string OriginalLicensePlate { get; private set; }
        public string PlateNumber { get; private set; }
        public string PlateColor { get; private set; }
        public string VehicleType { get; private set; }
        public string VehicleColor { get; private set; }
        public string TransportationType { get; private set; }
        public string PlateImagePath { get; private set; }
        public string Location { get; private set; }
        public string OperatorId { get; private set; }
        public string OperatorName { get; private set; }
        public DateTime CaptureDate { get; private set; }
        public RecognizenEvent()
        { }

        public RecognizenEvent(
            string eventType,
            string originaLicensePlate,
            string plateNumber,
            string plateColor,
            string vehicleType,
            string vehicleColor,
            string transportationType,
            string plateImagePath,
            string location,
            string empCode,
            string createBy,
            DateTime captureDate) : this()
        {
            EventType = eventType;
            OriginalLicensePlate = originaLicensePlate;
            PlateNumber = plateNumber;
            PlateColor = plateColor;
            VehicleType = vehicleType;
            VehicleColor = vehicleColor;
            TransportationType = transportationType;
            PlateImagePath = plateImagePath;
            Location = location;
            OperatorId = empCode;
            OperatorName = createBy;
            CaptureDate = captureDate;
            CreateBy = createBy;
            CreateDate = DateTime.Now;
            LastUpdateBy = createBy;
            LastUpdateDate = DateTime.Now;
        }
    }
}
