using System;

namespace LTM_FunctionsApp.Shared
{
    public interface IObjectParseResultService<A> where A : class
    {
        Tuple<A, bool, string> TryParseObject(string jsonString);
    }
}
