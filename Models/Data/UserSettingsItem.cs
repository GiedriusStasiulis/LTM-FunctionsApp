using Newtonsoft.Json;

namespace LTM_FunctionsApp.Models.Data
{
    public class UserSettingsItem
    {
        //To upsert in cosmosdb, use lowercase 'id'
        [JsonProperty(PropertyName = "id")]
        public string SettingsItemID { get; set; }

        public string UserID { get; set; }

        public string PIDHexValue { get; set; }

        public string PIDName { get; set; }

        public string Payload0Name { get; set; }

        public string Payload1Name { get; set; }

        public string Payload2Name { get; set; }

        public string Payload3Name { get; set; }

        public string Payload4Name { get; set; }

        public string Payload5Name { get; set; }

        public string Payload6Name { get; set; }

        public string Payload7Name { get; set; }
    }
}
