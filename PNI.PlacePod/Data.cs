using System.Collections.Generic;

namespace PNI.PlacePod
{
    public class Data
    {
        public float? Temperature { get; set; }
        public bool? Recalibrated { get; set; }
        public float? Battery { get; set; }
        public bool? Occupied { get; set; }
        public bool? Deactivated { get; set; }
        public bool? RecalibrationOrReboot { get; set; }
        public int? VehicleCount { get;  set; }
        public bool? RebootSucceed { get; set; }
    }
}