using Contracts.Constants;
using OpenSearch.Client;
using OpenSearch.Net;

namespace Upskill.Tests;

public static class ConnectionHelper
{
    private static readonly Uri Endpoint = new("aaa");
    
    private static readonly ConnectionSettings ConnectionSettings = new ConnectionSettings(Endpoint)
        .DefaultIndex(Constants.ProductsIndexName)
//        .BasicAuthentication("root", "aaaa")
        .PrettyJson()
        .DefaultFieldNameInferrer(x => x.ToLower());

    public static IOpenSearchLowLevelClient GetWritClient() => new OpenSearchLowLevelClient(ConnectionSettings);
    public static IOpenSearchClient GetReadClient() => new OpenSearchClient(ConnectionSettings);
}