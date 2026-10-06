namespace Afutrans.Core.Models.Enums;

public enum SourceType
{
   /// <summary>
   /// Represents a source that is a file.
   /// </summary>
   File = 0,

   /// <summary>
   /// Represents a source that is a directory.
   /// </summary>
   Directory = 1,

   /// <summary>
   /// Represents a source that is an FTP server.
   /// </summary>
   Ftp = 2,

   /// <summary>
   /// Represents a source that is a URL.
   /// </summary>
   Url = 3,

   /// <summary>
   /// Represents a source that is a SQL Server database.
   /// </summary>
   SqlServer = 4,

   /// <summary>
   /// Represents a source that is a MySQL database.
   /// </summary>
   MySql = 5,

   /// <summary>
   /// Represents a source that is a PostgreSQL database.
   /// </summary>
   PostgreSql = 6,

   /// <summary>
   /// Represents a source that is a SQLite database.
   /// </summary>
   Sqlite = 7,

   MongoDb = 8,
}