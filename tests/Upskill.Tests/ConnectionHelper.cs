using Amazon;
using Amazon.Runtime;
using Contracts.Constants;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;

namespace Upskill.Tests;

public static class ConnectionHelper
{
    private static readonly Uri Endpoint = new("localhost:9200");

    private static readonly AwsSigV4HttpConnection Connection = new(new AssumeRoleAWSCredentials(
            new BasicAWSCredentials(
                "aaaa",
                "bbbbb"
                ),
            "cccccc",
            "test"
        ),
        RegionEndpoint.EUCentral1);

    // private static ConnectionSettings config = new(endpoint, connection);
    private static readonly ConnectionSettings ConnectionSettings = new ConnectionSettings(Endpoint, Connection)
        .DefaultIndex(Constants.ProductsIndexName)
        .PrettyJson()
        .DefaultFieldNameInferrer(x => x.ToLower());

    public static IOpenSearchLowLevelClient GetWritClient() => new OpenSearchLowLevelClient(ConnectionSettings);
    public static IOpenSearchClient GetReadClient() => new OpenSearchClient(ConnectionSettings);
}