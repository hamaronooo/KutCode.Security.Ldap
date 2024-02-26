using FastEndpoints;
using KutCode.Security.Ldap.Http;
using KutCode.Security.Ldap.Rpc;
using Serilog;

namespace KutCode.Security.Ldap.WebApi.Rpc.Handlers;

public sealed class AuthHandler : ICommandHandler<LdapAuthCommand, LdapAuthenticationResponse>
{
	private readonly LdapService _ldapService;
	public AuthHandler(LdapService ldapService)
	{
		_ldapService = ldapService;
	}
	public async Task<LdapAuthenticationResponse> ExecuteAsync(LdapAuthCommand command, CancellationToken ct)
	{
		Log.Information("RPC: Sending LDAP authorization for {User}", command.Login);
		return await Task.Run(() => _ldapService.Authenticate(command.Login, command.Password), ct);
	}
}