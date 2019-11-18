using System;
using System.Linq;
using Xunit;

namespace PNI.PlacePod.Test
{
    public class DecoderTest
    {
        [Fact]
        public void TemperatureTest()
        {

            var res = Decoder.Decode("026700F0").First();

            Assert.Equal(24.0f, res.Temperature);

            var res2 = Decoder.Decode("026701F0").First();

            Assert.Equal(49.6f, res2.Temperature);

            var res3 = Decoder.Decode("026781F0").First();

            Assert.Equal(-49.6f, res3.Temperature);

        }

        [Fact]
        public void RecalibateTest()
        {
            var res = Decoder.Decode("010101").First();

            Assert.Equal(true, res.Recalibrated);

            var res2 = Decoder.Decode("010100").First();
            Assert.Equal(false, res2.Recalibrated);
        }

        [Fact]
        public void BatteryTest()
        {
            var res = Decoder.Decode("0302015E").First();

            Assert.Equal(3.5f, res.Battery);            
        }

        [Fact]
        public void OccupiedTest()
        {
            var res = Decoder.Decode("156601").First();

            Assert.Equal(true, res.Occupied);

            res = Decoder.Decode("156600").First();

            Assert.Equal(false, res.Occupied);
        }

        [Fact]
        public void DeactivateTest()
        {
            var res = Decoder.Decode("1C0101").First(); 

            Assert.Equal(true, res.Deactivated);          
        }

        [Fact]
        public void VehicleTest()
        {
            var res = Decoder.Decode("210020").First();

            Assert.Equal(32, res.VehicleCount);
            Assert.Null(res.RecalibrationOrReboot);

            res = Decoder.Decode("210080").First();
            Assert.Null(res.VehicleCount);
            Assert.Equal(true, res.RecalibrationOrReboot);
        }

        [Fact]
        public void KeepAliveTest()
        {
            var res = Decoder.Decode("376600").First();

            Assert.Equal(false, res.Occupied);
            Assert.Null(res.RecalibrationOrReboot);
            Assert.Null(res.VehicleCount);

            res = Decoder.Decode("376601").First();
            Assert.Equal(true, res.Occupied);
            Assert.Null(res.RecalibrationOrReboot);
            Assert.Null(res.VehicleCount);

            res = Decoder.Decode("370080").First();
            Assert.Null(res.VehicleCount);
            Assert.Equal(true, res.RecalibrationOrReboot);
            Assert.Null(res.Occupied);

            res = Decoder.Decode("370020").First(); 
            Assert.Equal(32, res.VehicleCount);
            Assert.Null(res.RecalibrationOrReboot);
            Assert.Null(res.Occupied);
        }

        [Fact]
        public void RebootTest()
        {
            var res = Decoder.Decode("3F0101").First();

            Assert.True(res.RebootSucceed);
        }

        [Fact]

        public void RealDataTest()
        {
            //parking status
            var res = Decoder.Decode("156600156601");

            Assert.False(res.First().Occupied);
            Assert.True(res.ElementAt(1).Occupied);

            res = Decoder.Decode("0302016c026700dc");

            Assert.Equal(3.64f, res.ElementAt(0).Battery);
            Assert.Equal(22f, res.ElementAt(1).Temperature);
        }

        [Fact]

        public void EmptyDecode()
        {
            var res = Decoder.Decode("");

            Assert.Empty(res);
        }

        [Fact]
        public void UnknownDecode()
        {
            var res = Decoder.Decode("05000506000e050005060010376601");

            Assert.Empty(res.ToList());
        }
    }
}
