namespace Afutrans.Core.Models.Settings;

public class ApplicationSettings
{
   /// <summary>
   /// Gets or sets the default culture for the application. This is used when no specific culture is provided.
   /// </summary>
   public string ApplicationCulture { get; set; } = "en";

   /// <summary>
   /// Gets or sets the default source language for translations. This is used when no specific source language is provided.
   /// It helps to determine the language of the source text for translation purposes.
   /// </summary>
   public string DefaultSourceLanguage { get; set; } = "en";
}