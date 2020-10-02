using PaderConference.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PaderConference.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace PaderConference.Infrastructure.Tests.Auth
{
    public class JwtFactoryUnitTests
    {
        [Fact]
        public async void GenerateEncodedToken_GivenValidInputs_ReturnsExpectedTokenData()
        {
            // arrange
            var token = Guid.NewGuid().ToString();
            var id = Guid.NewGuid().ToString();
            var jwtIssuerOptions = new JwtIssuerOptions
            {
                Issuer = "",
                Audience = "",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secret_key")), SecurityAlgorithms.HmacSha256)
            };

            var mockJwtTokenHandler = new Mock<IJwtHandler>();
            mockJwtTokenHandler.Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>())).Returns(token);

            var jwtFactory = new JwtFactory(mockJwtTokenHandler.Object, Options.Create(jwtIssuerOptions));

            // act
            var result = await jwtFactory.GenerateEncodedToken(id, "userName");

            // assert
            Assert.Equal(token, result);
        }
    }
}
