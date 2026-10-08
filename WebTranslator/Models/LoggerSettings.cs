namespace WebTranslator.Models;

public class LoggerSettings
{
   public int DefaultRetentionDaysInfo { get; set; } = 7;
   public int DefaultRetentionDaysWarning { get; set; } = 30;
   public int CleanupInterval { get; set; } = 24; // interval in hours
}