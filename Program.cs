// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

// Setup application and configuration
var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configurationRoot = builder.Build();

var openAiSettings = configurationRoot.GetSection("OpenAiSettings").Get<OpenAiSettings>();
if (openAiSettings is null)
{
    Console.WriteLine("Configuration Error: No Open AI Key is defined");
    return;
}

// Call Open AI
using (var client = new HttpClient())
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiSettings.ApiKey}");

    var content = new StringContent("{\"prompt\":\"Write a blog post about the architecture of GPT-3\"}");

    var response = await client.PostAsync("engines/davinci/jobs", content);

    if (response.IsSuccessStatusCode)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
    }
}