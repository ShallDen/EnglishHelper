using System.Collections.Generic;
using LitJson;

namespace EnglishHelper.Core
{
    sealed class JsonObject
    {
        public int code = 0;
        public string language = string.Empty;
        public List<string> text = new List<string>();
    }

    public static class JsonHelper
    {
        public static string Parse(string jsonString)
        {
            JsonObject jsonObject = JsonMapper.ToObject<JsonObject>(jsonString);
            return jsonObject.text[0];
        }
    }
}
