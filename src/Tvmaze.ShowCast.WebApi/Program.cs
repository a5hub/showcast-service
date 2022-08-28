using Serilog;
using Tvmaze.ShowCast.ApiClient.ApiPolicies;
using Tvmaze.ShowCast.ApiClient.Extensions;
using Tvmaze.ShowCast.ApiClient.Options;
using Tvmaze.ShowCast.Core.Bll.Services;
using Tvmaze.ShowCast.Core.Dal.Repository;
using Tvmaze.ShowCast.Core.Extensions;
using Tvmaze.ShowCast.Core.Options;
using Tvmaze.ShowCast.WebApi.ApiServices;
using Tvmaze.ShowCast.WebApi.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Options
builder.Services.Configure<TvmazeClientOptions>(builder.Configuration.GetSection(TvmazeClientOptions.Key));
builder.Services.Configure<ApiPolicyOptions>(builder.Configuration.GetSection(ApiPolicyOptions.Key));
builder.Services.Configure<ParallelismOptions>(builder.Configuration.GetSection(ParallelismOptions.Key));

// Api clients
builder.Services.AddTransient<IApiPolicies, ApiPolicies>();
builder.Services.AddTvmazeApiServices();
        
// Services
builder.Services.AddTransient<IShowCastApiService, ShowCastApiService>();
builder.Services.AddTransient<IShowCastService, ShowCastService>();
builder.Services.AddTransient<IShowCastRepository, ShowCastRepository>();
        
// Database
builder.Services.AddCosmosDbService(builder.Configuration.GetSection(CosmosDbOptions.Key).Bind);
        
// Serilog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration, "Serilog")
    .CreateLogger();
builder.Logging.AddSerilog(logger);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();