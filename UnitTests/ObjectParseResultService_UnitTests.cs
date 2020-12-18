using LTM_FunctionsApp.Models.Data;
using LTM_FunctionsApp.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LTM_FunctionsApp.UnitTests
{
    [TestClass]
    public class ObjectParseResultService_UnitTests
    {
        public LinFrame _linFrame1;
        public LinFrame _linFrame2;
        public List<LinFrame> _linFrames;
        public LinFramesPacket _linFramesPacket;

        public string linFramePacketJsonOK = "{\"PCKNO\":1,\"DEVID\":\"ESP32SIM1_1607594372\",\"FRAMES\":[{\"FNO\":1,\"FDATA\":\"123456789123456789\"},{\"FNO\":2,\"FDATA\":\"987654321987654321\"}]}";
        public string linFramePacketJsonBAD_IncorrectPCKNOName = "{\"PACKNumber\":1,\"DEVID\":\"ESP32SIM1_1607594372\",\"FRAMES\":[{\"FNO\":1,\"FDATA\":\"123456789123456789\"},{\"FNO\":2,\"FDATA\":\"987654321987654321\"}]}";
        public string linFramePacketJsonBAD_InvalidPCKNOType = "{\"PCKNO\":\"1\",\"DEVID\":\"ESP32SIM1_1607594372\",\"FRAMES\":[{\"FNO\":1,\"FDATA\":\"123456789123456789\"},{\"FNO\":2,\"FDATA\":\"987654321987654321\"}]}";
        public string linFramePacketJsonBAD_EmptyPCKNO = "{\"PCKNO\":,\"DEVID\":\"ESP32SIM1_1607594372\",\"FRAMES\":[{\"FNO\":1,\"FDATA\":\"123456789123456789\"},{\"FNO\":2,\"FDATA\":\"987654321987654321\"}]}";
        public string linFramePacketJsonBAD_MissingPCKNO = "{\"DEVID\":\"ESP32SIM1_1607594372\",\"FRAMES\":[{\"FNO\":1,\"FDATA\":\"123456789123456789\"},{\"FNO\":2,\"FDATA\":\"987654321987654321\"}]}";
        public string linFramesPacketJsonBAD_Empty = "";
        public string linFramesPacketJsonBAD_WhiteSpace = " ";

        public string userSettingsItemJsonOK = "";
        public string userSettingsItemJsonBAD = "";

        readonly IObjectParseResultService<LinFramesPacket> _testServiceForLinFramePacket 
            = new ObjectParseResultService<LinFramesPacket>();

        [TestInitialize]
        public void Setup()
        {
            _linFrame1 = new LinFrame() { FNO = 1, FDATA = "123456789123456789" };
            _linFrame2 = new LinFrame() { FNO = 2, FDATA = "987654321987654321" };

            _linFrames = new List<LinFrame>
            {
                _linFrame1, _linFrame2
            };

            _linFramesPacket = new LinFramesPacket() { PCKNO = 1, DEVID = "ESP32SIM1_1607594372" , 
                FRAMES = _linFrames };
        }

        [TestMethod]
        public void IsLinFramePacketJsonFormatOK_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramePacketJsonOK);

            Assert.AreEqual(JsonConvert.SerializeObject(result.Item1),JsonConvert.SerializeObject(_linFramesPacket));            
            Assert.IsTrue(result.Item2);
            Assert.IsTrue(result.Item3.Equals("Parse OK"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonIncorrectPCKNOName_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramePacketJsonBAD_IncorrectPCKNOName);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Exception caught:"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonInvalidPCKNOType_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramePacketJsonBAD_IncorrectPCKNOName);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Exception caught:"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonEmptyPCKNO_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramePacketJsonBAD_IncorrectPCKNOName);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Exception caught:"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonMissingPCKNO_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramePacketJsonBAD_IncorrectPCKNOName);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Exception caught:"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonEmpty_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramesPacketJsonBAD_Empty);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Empty, null or whitespace string?"));
        }

        [TestMethod]
        public void IsLinFramePacketJsonWhiteSpace_ReturnTrue()
        {
            var result = _testServiceForLinFramePacket.TryParseObject(linFramesPacketJsonBAD_WhiteSpace);

            Assert.AreEqual(result.Item1, null);
            Assert.IsTrue(!result.Item2);
            Assert.IsTrue(result.Item3.Contains("Empty, null or whitespace string?"));
        }
    }
}
