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
    try
    {
        IPHostEntry host = Dns.GetHostEntry(domainName);
        app.Logger.LogInformation("Dns call was made");
        // TODO: hacer una validacion mejor con hots.AddressList

        app.Logger.LogInformation("Domain address " + host.AddressList);
        app.Logger.LogInformation("IPHosto to string:" + host.HostName.ToString());
        object response = new { hostName = host.HostName, inUse = host.AddressList.Length > 0 };
        return response;
    }
    // cuando se da un dominio invalido o no existe tira un error
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


})
.WithName("GetDomainInfo");

app.Run();

