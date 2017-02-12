using Newtonsoft.Json;

namespace dotnet_core_simple_jwt
{
    public class JwtHeader
    {
        public string alg { get; set; }

        public string typ { get; set; }

       
        public static JwtHeader Create(string json)
        {
            var instance = new JwtHeader();
            JsonConvert.PopulateObject(json, instance);
            return instance;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}