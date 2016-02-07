using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace EnglishHelper.Core
{
    class JsonObject
    {
        public int code = 0;
        public string language = string.Empty;
        public List<string> text = new List<string>();
    }

    public class JsonHelper
    {
        public static string Parse(string jsonString)
        {
            JsonObject jsonObject = JsonMapper.ToObject<JsonObject>(jsonString);
            return jsonObject.text[0];
        }
    }
}
