// See https://aka.ms/new-console-template for more information
using RestSharp;

Console.WriteLine("Hello, World!");
var options = new RestClientOptions("https://localhost:7108")
{
    Timeout = TimeSpan.FromSeconds(10),
};
var client = new RestClient(options);
var request = new RestRequest("/customer/123123/score/12", Method.Post);
RestResponse response = await client.ExecuteAsync(request);
Console.WriteLine(response.Content);