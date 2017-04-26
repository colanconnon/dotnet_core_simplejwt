using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace dotnet_core_simple_jwt
{
    public interface IJwtVerificationStrategy
    {
        IdentityUser verify(string jwtToken);

        IdentityUser HandleTokenVerificationRequest(HttpContext context);
    }
}
