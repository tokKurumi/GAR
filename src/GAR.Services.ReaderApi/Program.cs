using Asp.Versioning;
using GAR.ServiceDefaults;
using GAR.Services.ReaderApi.Data;
using GAR.Services.ReaderApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ZipXmlReaderService>();
builder.Services.AddSingleton<ZipXmlParserService>();
builder.Services.AddSingleton<XmlTypesCopyHelpers>();
builder.Services.AddSingleton<XmlDataCopyHelpers>();

builder.AddNpgsqlDataSource("gar-database");
builder.Services.AddSingleton<DatabaseInitializerService>();
builder.Services.AddSingleton<SqlCopyHelpers>();
builder.Services.AddSingleton<DataWriterService>();

builder.Services.AddSingleton<DataTransferService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ReaderApi",
        Description = "An ASP.NET Core Web API for reading GAR data and provide it into PostgreSQL database",
        Contact = new OpenApiContact
        {
            Name = "Yudashkin Oleg",
            Url = new Uri(@"https://github.com/tokKurumi"),
            Email = @"tokkurumi901@gmail.com",
        },
    });
});
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc().AddApiExplorer(config =>
{
    config.GroupNameFormat = "'v'V";
    config.SubstituteApiVersionInUrl = true;
});

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    config.EnablePersistAuthorization();
    config.DisplayRequestDuration();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
