using System.Net;
using Grpc.Core;

namespace Dotnet.Communication.gRPC.Server.Services;

public class AuthService : Auth.AuthBase
{
    private readonly ILogger<AuthService> _logger;
    private static readonly Dictionary<string, string> _cadastros = new();
    public AuthService(ILogger<AuthService> logger)
    {
        _logger = logger;
        
    }
    public override Task<SignUpReply> SignUp(SignUpRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[INICIO] signup");
        if (_cadastros.ContainsKey(request.Email))
        {
            _logger.LogError("[TERMINO] signup com erro");
            return Task.FromResult(new SignUpReply { Email = request.Email, Success = false });
        }
        
        _cadastros.Add(request.Email, request.Password);
        _logger.LogInformation("[TERMINO] signup com sucesso");
        return Task.FromResult(new SignUpReply { Email = request.Email, Success = true });
    }
}