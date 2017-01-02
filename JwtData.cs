using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace dotnet_core_simple_jwt
{
    public class JwtData
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public DateTime? Iat { get; set; }
        
        public DateTime? Exp { get; set; }

        public string Audience { get; set; }

        public JwtData()
        {

        }

        public static JwtData Create(string json)
        {
            var instance = new JwtData();
            JsonConvert.PopulateObject(json, instance);
            return instance;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
