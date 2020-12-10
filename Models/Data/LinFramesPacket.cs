using Newtonsoft.Json;
using System.Collections.Generic;

namespace LTM_FunctionsApp.Models.Data
{
    [JsonObject(ItemRequired = Required.Always)]
    public class LinFramesPacket
    {
        public int PCKNO { get; set; }

        public string DEVID { get; set; }

        public List<LinFrame> FRAMES { get; set; }

    }
}
