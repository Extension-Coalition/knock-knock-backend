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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
    int index = domainName.IndexOf(".");

    // TODO: Create a new logic that takes a name and adds a bunch of .endings
    // It should return an array of response objects
    if (index == -1)
    {
        return "We dont have this logic yet";
    }

    // There is a dot, so we are working with a domain
    if (index != -1)
    {
        // This runs with full domains 
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

    // Lamda function needs to always have a return
    // its not smarth enoght to notiwe our conditions on top
    // will always return something... or not?
    return "how did you get here";
})
.WithName("GetDomainInfo");

app.Run();

