using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public class JwtTokenBuilder
    {
        
        public JwtTokenBuilder()
        {
            _algorithm = "HS256";
        }

        private string _algorithm;

        private Dictionary<string, object> _claims;

        private string _username;

        private string _user_id;

        private DateTime? _expiration;
        private string _secret;

        public JwtTokenBuilder AddExpiration(DateTime expiration)
        {
            _expiration = expiration;
            return this;
        }

        public JwtTokenBuilder AddUsername(string username)
        {
            this._username = username;
            return this;
        }
        public JwtTokenBuilder AddClaim(string key, object value)
        {
            return this;
        }

        public JwtTokenBuilder AddUserId(string userId)
        {
            this._user_id = userId;
            return this;
        }

        public JwtTokenBuilder AddSecret(string secret)
        {
            this._secret = secret;
            return this;
        }

        public string Build()
        {
            var jwtHeader = new JwtHeader()
            {
                typ = "JWT",
                alg = _algorithm
            };
            var jwtData = new JwtData()
            {
                Username = _username,
                Iat = _expiration,
                Exp = _expiration,
                UserId = _user_id
            };
            var jwtToken = new JwtToken()
            {
                jwtHeader = jwtHeader,
                jwtData = jwtData
            };
            var jwtStrategy = JwtSignatureFactory.Create("HS256", _secret);
            return jwtStrategy.Encode(jwtToken);
        }
    }
}
