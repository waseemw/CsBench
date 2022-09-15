using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.SetMinimumLevel(LogLevel.Trace);
// //builder.Logging.ClearProviders();
// // Services:
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(o => o.BufferBody = false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromSeconds(60));
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});
var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run("http://localhost:80");