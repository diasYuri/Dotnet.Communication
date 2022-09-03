using Dotnet.Communication.gRPC.Client.Model;
using Dotnet.Communication.gRPC.Server;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Communication.gRPC.Client.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly Auth.AuthClient _authClient;
    
    public AuthController(ILogger<AuthController> logger, Auth.AuthClient authClient)
    {
        _logger = logger;
        _authClient = authClient;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(AuthRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[INICIO] api signup");
        var grpcRequest = new SignUpRequest
        {
            Email = request.Email,
            Password = request.Password
        };

        var reply = await _authClient.SignUpAsync(grpcRequest, cancellationToken: cancellationToken);

        if (reply.Success)
        {
            _logger.LogInformation("[TERMINO] api signup, sucesso");
            return Ok(reply);
        }

        _logger.LogError("[TERMINO] api signup, erro");
        return BadRequest("Não foi possível cadastrar o usuário");
    }
}