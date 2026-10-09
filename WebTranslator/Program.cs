using Microsoft.AspNetCore.Http.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AspNetCore.App.SignalR.Extensions;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebTranslator.Logging;
using WebTranslator.Startup;

const string ConsoleOutputTemplate = """
┌──────────────────────────────────────────────────────────────────────────────
│ {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]
│ Source: {SourceContext}
│ Message: {Message:lj}
│ Properties: {Properties:j}
{Exception}└──────────────────────────────────────────────────────────────────────────────

""";

#if DEBUG
const LogEventLevel BootstrapMinimumLevel = LogEventLevel.Verbose;
const LogEventLevel ApplicationMinimumLevel = LogEventLevel.Verbose;
const LogEventLevel FrameworkMinimumLevel = LogEventLevel.Debug;
const LogEventLevel AspNetCoreMinimumLevel = LogEventLevel.Debug;
const LogEventLevel RequestSuccessLevel = LogEventLevel.Debug;
const LogEventLevel JsonFileMinimumLevel = LogEventLevel.Debug;
const double SlowRequestThresholdMs = 500;
#else
const LogEventLevel BootstrapMinimumLevel = LogEventLevel.Information;
const LogEventLevel ApplicationMinimumLevel = LogEventLevel.Information;
const LogEventLevel FrameworkMinimumLevel = LogEventLevel.Warning;
const LogEventLevel AspNetCoreMinimumLevel = LogEventLevel.Warning;
const LogEventLevel RequestSuccessLevel = LogEventLevel.Information;
const LogEventLevel JsonFileMinimumLevel = LogEventLevel.Information;
const double SlowRequestThresholdMs = 1000;
#endif

// Bootstrap logger used before DI container is fully built.
Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Is(BootstrapMinimumLevel)
   .MinimumLevel.Override("Microsoft", FrameworkMinimumLevel)
   .MinimumLevel.Override("Microsoft.AspNetCore", AspNetCoreMinimumLevel)
   .MinimumLevel.Override("System", FrameworkMinimumLevel)
   .Enrich.FromLogContext()
#if DEBUG
   .WriteTo.Console(
      restrictedToMinimumLevel: LogEventLevel.Verbose,
      theme: AnsiConsoleTheme.Code,
      outputTemplate: ConsoleOutputTemplate)
#endif
   .CreateBootstrapLogger();

try
{
   Log.Information("Starting web application");
   WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

   // Configure Serilog as the logging provider.
   int retentionDaysInfo = builder.Configuration.GetValue<int>("LoggerSettings:DefaultRetentionDaysInfo", LogStoragePaths.FallbackRetentionDaysInfo);
   int retentionDaysWarning = builder.Configuration.GetValue<int>("LoggerSettings:DefaultRetentionDaysWarning", LogStoragePaths.FallbackRetentionDaysWarning);
   LogStoragePaths logStoragePaths = LogStoragePaths.Create(
      contentRootPath: builder.Environment.ContentRootPath,
      retentionDaysInfo: retentionDaysInfo,
      retentionDaysWarning: retentionDaysWarning);

   logStoragePaths.EnsureDirectories();
   LogStorageMaintenance.CleanupExpiredFiles(logStoragePaths);

   builder.Logging.ClearProviders();
   builder.Services.AddSingleton(logStoragePaths);
   builder.Services.AddSingleton<JsonArrayFileSink>();
   builder.Services.AddSingleton<SqliteLogSink>();
   builder.Services.AddDefaultSerilogHub();
   builder.Services.AddHostedService<LogCleanupService>();
   builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
   .MinimumLevel.Is(ApplicationMinimumLevel)
   .MinimumLevel.Override("Microsoft", FrameworkMinimumLevel)
   .MinimumLevel.Override("Microsoft.AspNetCore", AspNetCoreMinimumLevel)
   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
   .MinimumLevel.Override("System", FrameworkMinimumLevel)
   .Enrich.FromLogContext()
   .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
   .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
#if DEBUG
   .WriteTo.Console(
      restrictedToMinimumLevel: LogEventLevel.Verbose,
      theme: AnsiConsoleTheme.Literate,
      outputTemplate: ConsoleOutputTemplate)
#endif
   .WriteTo.Sink(services.GetRequiredService<JsonArrayFileSink>(), restrictedToMinimumLevel: JsonFileMinimumLevel)
   .WriteTo.Sink(services.GetRequiredService<SqliteLogSink>(), restrictedToMinimumLevel: LogEventLevel.Warning)
   .WriteTo.SignalR(services, "ReceiveEvent"));

   // Serialization
   builder.Services.Configure<JsonOptions>(options =>
   {
      options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
      options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
   });

   builder.Services.Configure<ForwardedHeadersOptions>(options =>
   {
      options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                 Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
   });

   // Adding Conrollers
   builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
         options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
         options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      })
      .AddXmlDataContractSerializerFormatters();

   // Adding OpenAPI/Swagger
   builder.Services.AddOpenApi();

   // Add services to the container.
   builder.Services.AddRazorPages();

   // Add SignalR with JSON serialization options
   builder.Services.AddSignalR()
      .AddJsonProtocol(o =>
      {
         o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
         o.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
         o.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

   builder.Services.AddHttpClient();
   builder.Services.AddHttpContextAccessor();
   builder.Services.AddAntiforgery(options =>
   {
      options.HeaderName = "X-XSRF-TOKEN";
      options.Cookie.Name = "XSRF-TOKEN";
      options.Cookie.SecurePolicy = CookieSecurePolicy.None;
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.HttpOnly = true;
   });
   builder.Services.AddDistributedMemoryCache();
   builder.Services.AddLocalization();

   var app = builder.Build();

   app.UseSerilogRequestLogging(options =>
   {
      options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
      options.GetLevel = (httpContext, elapsed, exception) =>
         ProgramPipelineHelpers.GetRequestLogLevel(httpContext, elapsed, exception, SlowRequestThresholdMs, RequestSuccessLevel);
      options.EnrichDiagnosticContext = ProgramPipelineHelpers.EnrichRequestDiagnosticContext;
   });

   // Configure the HTTP request pipeline.
   if(!app.Environment.IsDevelopment())
   {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
   }

   app.UseHttpsRedirection();

   app.UseRouting();
   /* */

   app.UseAuthorization();

   app.MapStaticAssets();
   app.MapRazorPages()
      .WithStaticAssets();

   app.Run();
}
catch(Exception exception)
{
   Log.Fatal(exception, "Application terminated unexpectedly");
   throw;
}
finally
{
   Log.CloseAndFlush();
}