using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/healt", () =>
{
    return "ok";
})
.WithName("GetHealth");

app.MapGet("/domain-info/{domainName}", (string domainName) =>
{

    // Check if we get a domain with a .ending or just a name
    // for now lest asume that we have a clean input from the FE

    // This will return a -1 if not dot found
    int indexOfDot = domainName.IndexOf(".");

    // takes a name and adds a bunch of .endings
    // returns an array of response objects
    if (indexOfDot == -1)
    {
        String[] endings = new string[] {"com","org","net","xyz","io"};

        // keep the array length in sync witht he domain endings
        // or learn how to use lists.
        object[] results = new object[5];

        int iterationIndex = 0;

        foreach (String ending in endings)
        {          
            results[iterationIndex] = findDomain(domainName + '.' + ending);
            iterationIndex = iterationIndex+1;
        }

        return results;
    }

    // There is a dot, so we are working with a domain
    if (indexOfDot != -1)
    {
        var result = findDomain(domainName);
        return result;
    }

    // Lamda function needs to always have a return
    // its not smarth enoght to notiwe our conditions on top
    // will always return something... or not?
    return "how did you get here";
})
.WithName("GetDomainInfo");

app.Run();


object findDomain(String domainName)
{
    try
    {
        IPHostEntry host = Dns.GetHostEntry(domainName);
        object response = new { hostName = host.HostName, inUse = host.AddressList.Length > 0 };
        return response;
    }
    // Dns.GetHostEntry failed to retrive information on this domain name
    // This (should) means the domain is available
    catch (SocketException e)
    {
        app.Logger.LogError("SocketException.", e);
        object response = new { hostName = domainName, inUse = false };
        return response;
    }
    catch
    {
        app.Logger.LogError("Something went wrong");
        return "something went wrong";
    }

}