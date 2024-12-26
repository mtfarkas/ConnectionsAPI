
using System.Text;

namespace ConnectionsAPI.Features.Version.Get;
public class GetVersionEndpoint : EndpointWithoutRequest<string>
{
    private static readonly int START_YEAR = 2024;

    public override void Configure()
    {
        Get("/version");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        StringBuilder responseBuilder = new();

        responseBuilder.Append("ConnectionsAPI ©");
        if (DateTime.UtcNow.Year != START_YEAR)
        {
            responseBuilder.AppendFormat("{0} - {1}; ", START_YEAR, DateTime.UtcNow.Year);
        }
        else
        {
            responseBuilder.AppendFormat("{0}; ", DateTime.UtcNow.Year);
        }

        responseBuilder.Append("by Mate Farkas; ");

        var currentAsm = System.Reflection.Assembly.GetExecutingAssembly();
        string asmVersion = currentAsm.GetName()?.Version?.ToString() ?? string.Empty;

        responseBuilder.AppendFormat("v{0};", asmVersion);

        await SendStringAsync(responseBuilder.ToString(), cancellation: ct);
    }
}
