using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System;
using Microsoft.CognitiveServices.Speech;

namespace TextToSpeech_Console
{
    public class RealTimeSpeechToText
    {
        public static async Task RecogniseSpeech() {
            var config = SpeechConfig.FromSubscription("Your Resource Key", "eastus");

            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say Something................");

                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"Recognized: {result.Text}");
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

