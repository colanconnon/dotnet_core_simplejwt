using System;
using Xunit;
using dotnet_core_simple_jwt;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace tests
{
    public class JwtTokenTests
    {

        [Fact]
        public void TestJwtTokenWithWrongSecretDoesntValidate()
        {
            var secret = "blahblah";
            var secret2 = "blahblah2";
            var username = "test";
            var Id = "123456";
            IJwtSignatureStrategy jwtStrategy = new HmacSignatureStrategy(secret);
            IJwtSignatureStrategy jwtStrategy2 = new HmacSignatureStrategy(secret2);

            var jwtData = new JwtData()
            {
                Username = username,
                UserId = Id
            };
            var jwtHeader = new JwtHeader()
            {
                alg = "HS256",
                typ  = "JWT" 
            };
            var jwtToken = new JwtToken()
            {   
                jwtData  = jwtData,
                jwtHeader = jwtHeader
            };
            var encodedJwt = jwtStrategy.Encode(jwtToken);

            Assert.Throws<InvalidJwtException>(() => {jwtStrategy2.Decode(encodedJwt);});
        }

        [Fact]
        public void TestJwtTokenCanBeCreatedWithBuilder()
        {
            var secret = "blahblah";
            var username = "test";
            var Id = "123456";
            var jwtToken = new JwtTokenBuilder().AddSecret(secret)
                                    .AddUsername(username)
                                    .AddUserId(Id)
                                    .AddExpiration(DateTime.UtcNow.AddDays(-7))
                                    .Build();    
            IJwtSignatureStrategy jwtStrategy = new HmacSignatureStrategy(secret);
            var decodedJwt  = jwtStrategy.Decode(jwtToken);
            Assert.Equal(Id, decodedJwt.jwtData.UserId);
            Assert.Equal(username, decodedJwt.jwtData.Username);
        }

        [Fact]
        public void TestJwtTokenCanBeEncodedAndDecodedWithRightSecret()
        {
            var secret = "blahblah";
            var username = "test";
            var Id = "123456";
            IJwtSignatureStrategy jwtStrategy = new HmacSignatureStrategy(secret);
            var jwtData = new JwtData()
            {
                Username = username,
                UserId = Id
            };
            var jwtHeader = new JwtHeader()
            {
                alg = "HS256",
                typ  = "JWT" 
            };
            var jwtToken = new JwtToken()
            {   
                jwtData  = jwtData,
                jwtHeader = jwtHeader
            };
            var encodedJwt = jwtStrategy.Encode(jwtToken);

            var decodedJwt  = jwtStrategy.Decode(encodedJwt);

            Assert.Equal(jwtToken.jwtData.UserId, decodedJwt.jwtData.UserId);
            Assert.Equal(jwtToken.jwtData.Username, decodedJwt.jwtData.Username);

        }

        [Fact]
        public void TestValidJwtTokenCanBeVerified()
        {
            var secret = "blahblah";
            var username = "test";
            var Id = "123456";
            var email = "test@test.com";
            var jwtOptions = new JwtMiddlewareOptions()
            {
                secret  = secret
            };
            var jwtToken = new JwtTokenBuilder().AddSecret(secret)
                                    .AddUsername(username)
                                    .AddUserId(Id)
                                    .AddExpiration(DateTime.UtcNow.AddDays(-7))
                                    .Build();
            jwtToken = $"Bearer: {jwtToken}";
            var user = new IdentityUser()
            {
                UserName = username,
                Id = Id,
                Email = email
            };
            var mock = new Mock<HttpContext>();
            var jwtVerify= new JwtTokenVerificaion<IdentityUser>(jwtOptions.secret, jwtOptions);
            mock.Setup(item => item.Request.Headers.ContainsKey("Authorization"))
                .Returns(true);
            mock.Setup(item => item.Request.Headers["Authorization"])
                .Returns(jwtToken);
            var _user = jwtVerify.HandleTokenVerificationRequest(mock.Object);
            Assert.Equal(_user.Id, Id);
            Assert.Equal(_user.UserName, username);
        }
    }
}
