using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech.Translation;

namespace TextToSpeech_Console
{
    public class SpeechTranslation
    {
        public static async Task SpeechTrans()
        {
            var config = SpeechTranslationConfig.FromSubscription("Your subscription key", "eastus");
            config.SpeechRecognitionLanguage = "en-US";
            config.AddTargetLanguage("fr");

            using (var recognizer = new TranslationRecognizer(config))
            {
                Console.WriteLine("Say something in English...");

                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.TranslatedSpeech)
                {
                    Console.WriteLine($"Recognized: {result.Text}");
                    foreach (var element in result.Translations)
                    {
                        Console.WriteLine($"Translated into '{element.Key}': {element.Value}");
                    }
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine("No speech could be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
        }

    }
}
