
using System.Text;

namespace ConnectionsAPI.Features.Version.Get
{
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

            responseBuilder.AppendFormat("v{0}-{1}+{2};",
                                         GitVersionInformation.MajorMinorPatch,
                                         GitVersionInformation.ShortSha,
                                         GitVersionInformation.EscapedBranchName);

            await SendStringAsync(responseBuilder.ToString(), cancellation: ct);
        }
    }
}
