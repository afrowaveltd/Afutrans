namespace Afutrans.Core.Models.Enums;

public enum SourceData
{
   /// <summary>
   /// Represents a source that is a text file.
   /// </summary>
   PlainText = 0,

   /// <summary>
   /// Represents a source that is a JSON file.
   /// </summary>
   JSON = 1,

   /// <summary>
   /// Represents a source that is an AJIS file.
   /// </summary>
   AJIS = 2,

   /// <summary>
   /// Represents a source that is a Markdown file.
   /// </summary>
   MD = 3,

   /// <summary>
   /// Represents a source that is an HTML file.
   /// </summary>
   HTML = 4,
}