using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSAServer.Services;
using RSAGen;
using static RSAGen.DTOs;

namespace RSAServer.Controllers;

[ApiController]
[Route("api/keys")]
public sealed class KeysController(IRsaKeyProvider keys) : ControllerBase
{
    [HttpGet("public")]
    public ActionResult<PublicKeyDto> GetPublicKey() =>
        new PublicKeyDto(keys.PublicKey.E.ToBase64(), keys.PublicKey.N.ToBase64());
}
