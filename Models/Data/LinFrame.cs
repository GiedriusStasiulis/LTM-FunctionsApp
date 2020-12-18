using Newtonsoft.Json;
using System;

namespace LTM_FunctionsApp.Models.Data
{
    [Serializable]
    [JsonObject(ItemRequired = Required.Always)]
    public class LinFrame
    {
        public int FNO { get; set; }

        public string FDATA { get; set; }
    }
}