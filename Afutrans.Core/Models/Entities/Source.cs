using Afutrans.Core.Models.Enums;

namespace Afutrans.Core.Models.Entities;

public class Source
{
   /// <summary>
   /// Gets or sets the unique identifier for the source.
   /// </summary>
   public int Id { get; set; }

   /// <summary>
   /// Gets or sets the name of the source. This is a descriptive name that helps identify the source in the application.
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the path of the source. This can be a file path, directory path, FTP URL, SQL connection string, or any other relevant path depending on the source type.
   /// </summary>
   public string Path { get; set; } = string.Empty;

   public SourceType Type { get; set; }
   public SourceData DataType { get; set; }
   public bool NeedsAuthentication { get; set; } = false;
   public string? Username { get; set; }
   public string? Password { get; set; }
   public string? ConnectionString { get; set; }
   public bool IsActive { get; set; } = true;
   public bool Recursive { get; set; } = false;
   public string? SourceLanguage { get; set; }
}