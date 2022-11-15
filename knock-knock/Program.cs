using System.Net;

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
        // TODO: hacer una validacion mejor con hots.AddressList
        if (host != null)
        {
            app.Logger.LogInformation("Domain address "+ host.AddressList[0].ToString());
            return "The domain is in use";
        }
        else
        {
            return "The domain is available";
        }

    }
    // cuando se da un dominio invalido o no existe tira un error
    catch
    {
        return "The domain is available";
    }


})
.WithName("GetDomainInfo");

app.Run();

