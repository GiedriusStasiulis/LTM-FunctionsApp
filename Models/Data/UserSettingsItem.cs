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
    }
}
