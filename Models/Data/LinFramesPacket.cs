using System.Collections.Generic;

namespace LTM_AzureFunctionsApp.Models.Data
{
    public class LinFramesPacket
    {
        public int PCKNO { get; set; }

        public string DEVID { get; set; }

        public List<LinFrame> FRAMES { get; set; }
    }
}
