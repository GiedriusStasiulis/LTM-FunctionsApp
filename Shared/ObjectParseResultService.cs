using Newtonsoft.Json;
using System;

namespace LTM_FunctionsApp.Shared
{
    public class ObjectParseResultService<A> : IObjectParseResultService<A> where A : class
    {
        public Tuple<A, bool, string> TryParseObject(string jsonString)
        {
            A deserializedObj = null;

            if (!string.IsNullOrEmpty(jsonString) && !string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    deserializedObj = JsonConvert.DeserializeObject<A>(jsonString);
                    return Tuple.Create(deserializedObj, true, "Parse OK");
                }
                catch(Exception ex)
                {
                    return Tuple.Create(deserializedObj, false, $"Exception caught: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                return Tuple.Create(deserializedObj, false, $"Empty, null or whitespace string? String content: '{jsonString}'");
            }
        }
    }
}
