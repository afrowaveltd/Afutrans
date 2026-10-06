namespace Afutrans.Core.Models.Enums;

public enum TranslationBackends
{
   /// <summary>
   /// Represents the Google Translate backend.
   /// </summary>
   GoogleTranslate = 0,

   /// <summary>
   /// Represents the Microsoft Translator backend.
   /// </summary>
   MicrosoftTranslator = 1,

   /// <summary>
   /// Represents the DeepL Translator backend.
   /// </summary>
   DeepLTranslator = 2,

   /// <summary>
   /// Represents the Amazon Translate backend.
   /// </summary>
   AmazonTranslate = 3,

   /// <summary>
   /// Represents the IBM Watson Language Translator backend.
   /// </summary>
   IBMWatsonLanguageTranslator = 4,

   /// <summary>
   /// Represents the LibreTranslate backend.
   /// </summary>
   LibreTranslate = 5,

   /// <summary>
   /// Represents the TranslateAI backend.
   /// </summary>
   TranslateAI = 6,

   /// <summary>
   /// Represents the LanguageDetectAI backend.
   /// </summary>

   LanguageDetectAI = 7,
}