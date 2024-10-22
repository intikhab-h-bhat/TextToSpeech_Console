//using Microsoft.Identity.Client;
//using Newtonsoft.Json;
//using System;
//using System.IO;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//class Program
//{
//    private const string TENANT_ID = "";
//    private const string CLIENT_ID = "";
//    private const string CLIENT_SECRET = "";
//    private const string RESOURCE = "https://management.azure.com/.default";
//    private const string IMAGE_PATH = "C:\\Users\\lenovo\\Pictures\\kill.png"; // Set your image path here  
//    private const string QUESTION = "How is the weather today?"; // Set your question here  
//    private const string ENDPOINT = "https://ai-intikhabhai065690971993.openai.azure.com/openai/deployments/gpt-4/chat/completions?api-version=2024-02-15-preview";

//    static async Task Main()
//    {
//        var encodedImage = Convert.ToBase64String(File.ReadAllBytes(IMAGE_PATH));

//        // Acquire a token from Entra ID (Azure AD)  
//        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(CLIENT_ID)
//            .WithClientSecret(CLIENT_SECRET)
//            .WithAuthority(new Uri($"https://login.microsoftonline.com/{TENANT_ID}"))
//            .Build();

//        string[] scopes = new string[] { RESOURCE };
//        AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
//        string token = result.AccessToken;

//        using (var httpClient = new HttpClient())
//        {
//            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

//            var payload = new
//            {
//                // Add your payload structure here  
//                messages = new object[]
//                {
//                    new
//                    {
//                        role = "system",
//                        content = new object[]
//                        {
//                            new
//                            {
//                                type = "text",
//                                text = "You are an AI assistant that helps people find information."
//                            }
//                        }
//                    },
//                    new
//                    {
//                        role = "user",
//                        content = new object[]
//                        {
//                            new
//                            {
//                                type = "image_url",
//                                image_url = new
//                                {
//                                    url = $"data:image/jpeg;base64,{encodedImage}"
//                                }
//                            },
//                            new
//                            {
//                                type = "text",
//                                text = QUESTION
//                            }
//                        }
//                    }
//                },
//                temperature = 0.7,
//                top_p = 0.95,
//                max_tokens = 800,
//                stream = false
//            };

//            var response = await httpClient.PostAsync(ENDPOINT, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
//            if (response.IsSuccessStatusCode)
//            {
//                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
//                Console.WriteLine(responseData);
//            }
//            else
//            {
//                Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}");
//            }
//        }
//    }
//}


using TextToSpeech_Console;


   // PronunciationEvaluvation objPronun= new PronunciationEvaluvation();
    // Call the pronunciation assessment method
   // await PronunciationEvaluvation.PronunciationAssessmentContinuousWithFile();

    await RealTimeSpeechToText.RecogniseSpeech();

    // Keep the console window open
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();





//using Microsoft.CognitiveServices.Speech;
//using Microsoft.CognitiveServices.Speech.Audio;
//using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
//using DiffPlex;
//using DiffPlex.DiffBuilder;
//using DiffPlex.DiffBuilder.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;


//namespace PronunciationAssessmentApp
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            // Call the pronunciation assessment method
//            await PronunciationAssessmentContinuousWithFile();

//            // Keep the console window open
//            Console.WriteLine("Press any key to exit...");
//            Console.ReadKey();
//        }

//        public static async Task PronunciationAssessmentContinuousWithFile()
//        {
//            // Creates an instance of a speech config with specified subscription key and service region.
//            // Replace with your own subscription key and service region (e.g., "westus").
//            var config = SpeechConfig.FromSubscription("your resource key", "eastus");

//            // Creates a speech recognizer using file as audio input. 
//            // provide a WAV file as an example. Replace it with your own.
//            using (var audioInput = AudioConfig.FromWavFileInput(@"D:\c#-crash-course\TextToSpeech_Console\Test.wav"))
//            {
//                var language = "en-US";

//                using (var recognizer = new SpeechRecognizer(config, language, audioInput))
//                {
//                    var referenceText = "It took me a long time to learn where he came from. The little prince, who asked me so many questions, never seemed to hear the ones I asked him. It was from words dropped by chance that, little by little, everything was revealed to me.";
//                    var enableMiscue = true;

//                    // create pronunciation assessment config, set grading system, granularity and if enable miscue based on your requirement.
//                    var pronConfig = new PronunciationAssessmentConfig(referenceText, GradingSystem.HundredMark, Granularity.Phoneme, enableMiscue);
//                    pronConfig.EnableProsodyAssessment();

//                    pronConfig.ApplyTo(recognizer);

//                    var recognizedWords = new List<string>();
//                    var pronWords = new List<Word>();
//                    var finalWords = new List<Word>();
//                    var fluency_scores = new List<double>();
//                    var prosody_scores = new List<double>();
//                    var durations = new List<int>();
//                    var done = false;

//                    recognizer.SessionStopped += (s, e) =>
//                    {
//                        Console.WriteLine("ClOSING on {0}", e);
//                        done = true;
//                    };

//                    recognizer.Canceled += (s, e) =>
//                    {
//                        Console.WriteLine("ClOSING on {0}", e);
//                        done = true;
//                    };

//                    recognizer.Recognized += (s, e) =>
//                    {
//                        Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
//                        var pronResult = PronunciationAssessmentResult.FromResult(e.Result);
//                        Console.WriteLine($"    Accuracy score: {pronResult.AccuracyScore}, pronunciation score: {pronResult.PronunciationScore}, completeness score: {pronResult.CompletenessScore}, fluency score: {pronResult.FluencyScore}, prosody score: {pronResult.ProsodyScore}");

//                        fluency_scores.Add(pronResult.FluencyScore);
//                        prosody_scores.Add(pronResult.ProsodyScore);

//                        foreach (var word in pronResult.Words)
//                        {
//                            var newWord = new Word(word.Word, word.ErrorType, word.AccuracyScore);
//                            pronWords.Add(newWord);
//                        }

//                        foreach (var result in e.Result.Best())
//                        {
//                            durations.Add(result.Words.Sum(item => item.Duration));
//                            recognizedWords.AddRange(result.Words.Select(item => item.Word).ToList());

//                        }
//                    };

//                    // Starts continuous recognition.
//                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

//                    while (!done)
//                    {
//                        // Allow the program to run and process results continuously.
//                        await Task.Delay(1000); // Adjust the delay as needed.
//                    }

//                    // Waits for completion.
//                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

//                    // For continuous pronunciation assessment mode, the service won't return the words with `Insertion` or `Omission`
//                    // even if miscue is enabled.
//                    // We need to compare with the reference text after received all recognized words to get these error words.
//                    string[] referenceWords = referenceText.ToLower().Split(' ');
//                    for (int j = 0; j < referenceWords.Length; j++)
//                    {
//                        referenceWords[j] = Regex.Replace(referenceWords[j], "^[\\p{P}\\s]+|[\\p{P}\\s]+$", "");
//                    }

//                    if (enableMiscue)
//                    {
//                        var differ = new Differ();
//                        var inlineBuilder = new InlineDiffBuilder(differ);
//                        var diffModel = inlineBuilder.BuildDiffModel(string.Join("\n", referenceWords), string.Join("\n", recognizedWords));

//                        int currentIdx = 0;

//                        foreach (var delta in diffModel.Lines)
//                        {
//                            if (delta.Type == ChangeType.Unchanged)
//                            {
//                                finalWords.Add(pronWords[currentIdx]);

//                                currentIdx += 1;
//                            }

//                            if (delta.Type == ChangeType.Deleted || delta.Type == ChangeType.Modified)
//                            {
//                                var word = new Word(delta.Text, "Omission");
//                                finalWords.Add(word);
//                            }

//                            if (delta.Type == ChangeType.Inserted || delta.Type == ChangeType.Modified)
//                            {
//                                Word w = pronWords[currentIdx];
//                                if (w.ErrorType == "None")
//                                {
//                                    w.ErrorType = "Insertion";
//                                    finalWords.Add(w);
//                                }

//                                currentIdx += 1;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        finalWords = pronWords;
//                    }

//                    //We can calculate whole accuracy by averaging
//                    var filteredWords = finalWords.Where(item => item.ErrorType != "Insertion");
//                    var accuracyScore = filteredWords.Sum(item => item.AccuracyScore) / filteredWords.Count();
//                    var prosodyScore = (prosody_scores).Sum() / prosody_scores.Count();

//                    //Re-calculate fluency score
//                    var fluencyScore = fluency_scores.Zip(durations, (x, y) => x * y).Sum() / durations.Sum();

//                    //Calculate whole completeness score
//                    var completenessScore = (double)pronWords.Count(item => item.ErrorType == "None") / referenceWords.Length * 100;
//                    completenessScore = completenessScore <= 100 ? completenessScore : 100;

//                    var pronScore = accuracyScore * 0.4 + prosodyScore * 0.2 + fluencyScore * 0.2 + completenessScore * 0.2;

//                    Console.WriteLine("Paragraph pronunciation score: {0}, accuracy score: {1}, completeness score: {2}, fluency score: {3}, prosody score: {4}", pronScore, accuracyScore, completenessScore, fluencyScore, prosodyScore);

//                    for (int idx = 0; idx < finalWords.Count(); idx++)
//                    {
//                        Word word = finalWords[idx];
//                        Console.WriteLine("{0}: word: {1}\taccuracy score: {2}\terror type: {3}",
//                            idx + 1, word.WordText, word.AccuracyScore, word.ErrorType);
//                    }
//                }
//            }
//        }

//    }
//    public class Word
//    {
//        public string WordText { get; set; }
//        public string ErrorType { get; set; }
//        public double AccuracyScore { get; set; }

//        // Constructor
//        public Word(string wordText, string errorType, double accuracyScore = 0)
//        {
//            WordText = wordText;
//            ErrorType = errorType;
//            AccuracyScore = accuracyScore;
//        }
//    }
//}


//************************************************************
//using System;
//using Microsoft.CognitiveServices.Speech;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        var config = SpeechConfig.FromSubscription("Your Resource Key", "eastus");

//        using (var recognizer = new SpeechRecognizer(config))
//        {
//            Console.WriteLine("Say Something................");

//            var result = await recognizer.RecognizeOnceAsync();

//            if (result.Reason == ResultReason.RecognizedSpeech)
//            {
//                Console.WriteLine($"Recognized: {result.Text}");
//            }
//            else if (result.Reason == ResultReason.NoMatch)
//            {
//                Console.WriteLine("No speech could be recognized.");
//            }
//            else if (result.Reason == ResultReason.Canceled)
//            {
//                var cancellation = CancellationDetails.FromResult(result);
//                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

//                if (cancellation.Reason == CancellationReason.Error)
//                {
//                    Console.WriteLine($"ErrorDetails={cancellation.ErrorDetails}");
//                }
//            }
//        }
//    }
//}


// *************** Transalation


//using System;
//using Microsoft.CognitiveServices.Speech;
//using Microsoft.CognitiveServices.Speech.Translation;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        var config = SpeechTranslationConfig.FromSubscription("Your Resource Key", "eastus");
//        config.SpeechRecognitionLanguage = "en-US";
//        config.AddTargetLanguage("fr");

//        using (var recognizer = new TranslationRecognizer(config))
//        {
//            Console.WriteLine("Say something in English...");

//            var result = await recognizer.RecognizeOnceAsync();

//            if (result.Reason == ResultReason.TranslatedSpeech)
//            {
//                Console.WriteLine($"Recognized: {result.Text}");
//                foreach (var element in result.Translations)
//                {
//                    Console.WriteLine($"Translated into '{element.Key}': {element.Value}");
//                }
//            }
//            else if (result.Reason == ResultReason.NoMatch)
//            {
//                Console.WriteLine("No speech could be recognized.");
//            }
//            else if (result.Reason == ResultReason.Canceled)
//            {
//                var cancellation = CancellationDetails.FromResult(result);
//                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

//                if (cancellation.Reason == CancellationReason.Error)
//                {
//                    Console.WriteLine($"ErrorDetails={cancellation.ErrorDetails}");
//                }
//            }
//        }
//    }
//}

