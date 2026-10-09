using WebTranslator.Models;

namespace WebTranslator.Logging;

/// <summary>
/// Holds the resolved file-system paths used for log storage and the associated retention and cleanup configuration.
/// </summary>
/// <remarks>
/// Two independent retention periods are supported:
/// <list type="bullet">
///   <item><description><see cref="RetentionDaysInfo"/> – applied to the JSON text log directory (Information and below).</description></item>
///   <item><description><see cref="RetentionDaysWarning"/> – applied to the SQLite database directory (Warning and above).</description></item>
/// </list>
/// Retention and cleanup defaults are read from the <c>LoggerSettings</c> section of <c>appsettings.json</c>.
/// </remarks>
public class LogStoragePaths
{
   public const int FallbackRetentionDaysInfo = 7;
   public const int FallbackRetentionDaysWarning = 30;
   public const int FallbackCleanupIntervalHours = 24;

   public LogStoragePaths(IConfiguration configuration, IHostEnvironment environment)
   {
      ArgumentNullException.ThrowIfNull(configuration);
      ArgumentNullException.ThrowIfNull(environment);

      LoggerSettings settings = configuration.GetSection("LoggerSettings").Get<LoggerSettings>() ?? new LoggerSettings();
      LogStoragePaths resolvedPaths = Create(environment.ContentRootPath, settings: settings);

      RootDirectory = resolvedPaths.RootDirectory;
      TextDirectory = resolvedPaths.TextDirectory;
      DatabaseDirectory = resolvedPaths.DatabaseDirectory;
      RetentionDaysInfo = resolvedPaths.RetentionDaysInfo;
      RetentionDaysWarning = resolvedPaths.RetentionDaysWarning;
      CleanupInterval = resolvedPaths.CleanupInterval;
   }

   /// <summary>Gets the root directory that contains all log subdirectories.</summary>
   public string RootDirectory { get; }

   /// <summary>Gets the directory where JSON text log files are written.</summary>
   public string TextDirectory { get; }

   /// <summary>Gets the directory where SQLite log database files are written.</summary>
   public string DatabaseDirectory { get; }

   /// <summary>
   /// Gets the number of days JSON text log files (Information and below) are retained before being deleted.
   /// Configured via <c>LoggerSettings:DefaultRetentionDaysInfo</c> (in <c>appsettings.json</c>).
   /// </summary>
   public int RetentionDaysInfo { get; }

   /// <summary>
   /// Gets the number of days SQLite database log files (Warning and above) are retained before being deleted.
   /// Configured via <c>LoggerSettings:DefaultRetentionDaysWarning</c> (in <c>appsettings.json</c>).
   /// </summary>
   public int RetentionDaysWarning { get; }

   /// <summary>The interval between consecutive cleanup passes run by <see cref="LogCleanupService"/>.</summary>
   public TimeSpan CleanupInterval { get; }

   /// <summary>
   /// Initializes a new instance of <see cref="LogStoragePaths"/> with explicitly supplied directory paths and retention periods.
   /// </summary>
   /// <param name="rootDirectory">The root directory that contains all log subdirectories.</param>
   /// <param name="textDirectory">The directory where JSON text log files are written.</param>
   /// <param name="databaseDirectory">The directory where SQLite log database files are written.</param>
   /// <param name="retentionDaysInfo">Days to retain JSON text log files (Information and below). Must be greater than zero.</param>
   /// <param name="retentionDaysWarning">Days to retain SQLite database files (Warning and above). Must be greater than zero.</param>
   /// <param name="cleanupInterval">The interval between cleanup passes.</param>
   public LogStoragePaths(
      string rootDirectory,
      string textDirectory,
      string databaseDirectory,
      int retentionDaysInfo,
      int retentionDaysWarning,
      TimeSpan? cleanupInterval = null)
   {
      if(string.IsNullOrWhiteSpace(rootDirectory))
      {
         throw new ArgumentException("The value cannot be null or whitespace.", nameof(rootDirectory));
      }

      if(string.IsNullOrWhiteSpace(textDirectory))
      {
         throw new ArgumentException("The value cannot be null or whitespace.", nameof(textDirectory));
      }

      if(string.IsNullOrWhiteSpace(databaseDirectory))
      {
         throw new ArgumentException("The value cannot be null or whitespace.", nameof(databaseDirectory));
      }

      if(retentionDaysInfo <= 0)
      {
         throw new ArgumentOutOfRangeException(nameof(retentionDaysInfo), retentionDaysInfo, "The retention period must be greater than zero.");
      }

      if(retentionDaysWarning <= 0)
      {
         throw new ArgumentOutOfRangeException(nameof(retentionDaysWarning), retentionDaysWarning, "The retention period must be greater than zero.");
      }

      TimeSpan resolvedCleanupInterval = cleanupInterval ?? TimeSpan.FromHours(FallbackCleanupIntervalHours);
      if(resolvedCleanupInterval <= TimeSpan.Zero)
      {
         throw new ArgumentOutOfRangeException(nameof(cleanupInterval), resolvedCleanupInterval, "The cleanup interval must be greater than zero.");
      }

      RootDirectory = rootDirectory;
      TextDirectory = textDirectory;
      DatabaseDirectory = databaseDirectory;
      RetentionDaysInfo = retentionDaysInfo;
      RetentionDaysWarning = retentionDaysWarning;
      CleanupInterval = resolvedCleanupInterval;
   }

   /// <summary>Creates the configured log directories if they do not already exist.</summary>
   public void EnsureDirectories()
   {
      Directory.CreateDirectory(RootDirectory);
      Directory.CreateDirectory(TextDirectory);
      Directory.CreateDirectory(DatabaseDirectory);
   }

   /// <summary>
   /// Creates a <see cref="LogStoragePaths"/> instance by deriving subdirectory paths from the application's content root.
   /// </summary>
   /// <param name="contentRootPath">The application content root path; the log directories are created beneath it.</param>
   /// <param name="retentionDaysInfo">Days to keep JSON text logs. Uses configured defaults when not supplied.</param>
   /// <param name="retentionDaysWarning">Days to keep SQLite database logs. Uses configured defaults when not supplied.</param>
   /// <param name="settings">Configuration-bound values used when overrides are not supplied.</param>
   /// <returns>A fully initialised <see cref="LogStoragePaths"/> instance.</returns>
   public static LogStoragePaths Create(
      string contentRootPath,
      int? retentionDaysInfo = null,
      int? retentionDaysWarning = null,
      LoggerSettings? settings = null)
   {
      if(string.IsNullOrWhiteSpace(contentRootPath))
      {
         throw new ArgumentException("The value cannot be null or whitespace.", nameof(contentRootPath));
      }

      settings ??= new LoggerSettings();

      int resolvedRetentionDaysInfo = retentionDaysInfo ?? (settings.DefaultRetentionDaysInfo > 0 ? settings.DefaultRetentionDaysInfo : FallbackRetentionDaysInfo);
      int resolvedRetentionDaysWarning = retentionDaysWarning ?? (settings.DefaultRetentionDaysWarning > 0 ? settings.DefaultRetentionDaysWarning : FallbackRetentionDaysWarning);
      int cleanupIntervalHours = settings.CleanupInterval > 0 ? settings.CleanupInterval : FallbackCleanupIntervalHours;

      string rootDirectory = Path.Combine(contentRootPath, "Logs");
      return new LogStoragePaths(
         rootDirectory,
         Path.Combine(rootDirectory, "Text"),
         Path.Combine(rootDirectory, "Database"),
         resolvedRetentionDaysInfo,
         resolvedRetentionDaysWarning,
         TimeSpan.FromHours(cleanupIntervalHours));
   }

   public string GetDatabasePath(DateTimeOffset timestamp)
   {
      DateTime localTimestamp = timestamp.ToLocalTime().Date;
      return Path.Combine(DatabaseDirectory, $"warnings-{localTimestamp:yyyyMMdd}.db");
   }
}