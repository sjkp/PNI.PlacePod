using System;
using System.Collections.Generic;
using System.Linq;

namespace PNI.PlacePod
{
    public class Decoder
    {
        /* 
         * Data Chan Hex Description Data Type Data Resolution
         * 1 0x01 Recalibrate Response 1
         * 2 0x02 Temperature 103 0.1 °C
         * 3 0x03 Battery 2 0.01 Volt
         * 5 0x05 PNI Internal* 0
         * 6 0x06 PNI Internal* 0
         * 21 0x15 Parking Status 102 0(Vacant) or 1(Occupied)
         * 28 0x1C Deactivate Response 1
         * 33 0x21 Vehicle Count 0 1 count for 1 vehicle
         * 55 0x37 Keep-Alive 102 or 0
         * 63 0x3F Reboot Response 1
        */

        /*
         *  Data Type Hex Description Data Size Data Resolution
         *  0 0x00 Digital Input 1 1
         *  1 0x01 Digital Output 1 1
         *  2 0x02 Analog Input 2 0.01 Signed
         *  3 0x03 Analog Output 2 0.01 Signed
         *  102 0x66 Presence Sensor 1 1
         *  103 0x67 Temperature Sensor 2 0.1 °C Signed MSB
         */
        public static IEnumerable<Data> Decode(string hex)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes.Add(byte.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
            }

            return Decode(bytes.ToArray());
        }

        public static IEnumerable<Data> Decode(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length;)
            {
                Data decoded = new Data();
                switch (bytes[i])
                {
                    case 0x01:
                        decoded.Recalibrated = (bytes[i+2] & 0x01) == 0x01;
                        i += 3;
                        break;
                    case 0x02:
                        var x = ((Int16)(bytes[i+2] & 0x7F) << 8) + bytes[i+3];
                        //MSB 1 then negate
                        if ((0x80 & bytes[i+2]) == 0x80)
                            x = -x;
                        decoded.Temperature = x / 10.0f;
                        i += 4;
                        break;
                    case 0x03:
                        decoded.Battery = (((Int16)(bytes[i+2]) << 8) + bytes[i+3]) / 100.0f;
                        i += 4;
                        break;
                    case 0x15:
                        ParkingStatus(bytes[i + 2], decoded);
                        i += 3;
                        break;
                    case 0x1C:
                        decoded.Deactivated = (bytes[i+2] & 0x01) == 0x01;
                        i += 3;
                        break;
                    case 0x21:
                        VehicleCount(bytes[i + 2], decoded);
                        i += 3;
                        break;
                    case 0x37:
                        //Keep alive
                        if ((bytes[1] & 0x66) == 0x66)
                        {
                            ParkingStatus(bytes[i + 2], decoded);
                        }
                        else
                        {
                            VehicleCount(bytes[i + 2], decoded);
                        }
                        i += 3;
                        break;
                    case 0x3F:
                        decoded.RebootSucceed = (bytes[2] & 0x01) == 0x01;
                        i += 3;
                        break;
                }
                yield return decoded;
            }
            
        }

        private static void ParkingStatus(byte @byte, Data decoded)
        {
            decoded.Occupied = (@byte & 0x01) == 0x01;
        }

        private static void VehicleCount(byte @byte, Data decoded)
        {
            if ((@byte & 0x80) == 0x80)
                decoded.RecalibrationOrReboot = true;
            else
                decoded.VehicleCount = (int)@byte;
        }
    }
}
