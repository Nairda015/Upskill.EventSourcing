using Amazon;
using Amazon.Runtime;
using Contracts.Constants;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;

namespace Upskill.Tests;

public static class ConnectionHelper
{
    private static readonly Uri Endpoint = new("https://97wo1p8o3k53pmune5q7.eu-central-1.aoss.amazonaws.com");

    private static readonly AwsSigV4HttpConnection Connection = new(new AssumeRoleAWSCredentials(
            new BasicAWSCredentials(
                "ASIA46ZQGKPVWSLNTBZV",
                "ie9+pXDfTS3qtLLKsea+db8QuXORj7JJODlFi9G1"
                ),
            "arn:aws:iam::890769921003:user/afranczak",
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