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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

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