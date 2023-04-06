using Contracts.Messages;
using Contracts.Projections;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace Projections.Handlers;

public static class LoggerExtensions
{
    public static void LogVersion(this ILogger logger, GetResponse<ProductProjection> response, EventMetadata metadata)
    {
        if ((ulong)response.Version == metadata.Number)
        {
            logger.LogVersionMatch(metadata.Number);
        }
        else
        {
            logger.LogVersionMismatch(response.Version, metadata.Number);
        }
    }
    
    private static void LogVersionMismatch(this ILogger logger, long responseVersion, ulong messageVersion) =>
        logger.LogError(
            """
            Response version does not match message version, skipping update.
            Response version: {ResponseVersion}, message version: {MessageVersion}.
            """,
            responseVersion,
            messageVersion);

    private static void LogVersionMatch(this ILogger logger, ulong responseVersion) =>
        logger.LogInformation("Response version matches message version, applying update {Version}", responseVersion);
}