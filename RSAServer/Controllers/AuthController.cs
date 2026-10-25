using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static RSAGen.DTOs;
using RSAGen;
using RSAServer.Services;

namespace RSAServer.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ILoginService loginService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(EncryptedLoginDto request)
    {
        var result = loginService.Login(request);

        return result.Status switch
        {
            LoginStatus.Success => Ok(new LoginResponse(true, $"Welcome, {result.Login}!")),
            LoginStatus.InvalidCredentials => Unauthorized(new LoginResponse(false, "Invalid username or password")),
            LoginStatus.MalformedPayload => BadRequest(new LoginResponse(false, "Malformed payload")),
            _ => throw new UnreachableException()
        };
    }
}
