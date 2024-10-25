using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech_Console
{
    public class LanguageLearning
    {
        static readonly string speechKey = "speech key";
        static readonly string serviceRegion = "eastus";
        static readonly string aoaiResourceName = "name";
        static readonly string aoaiDeploymentName = "model";
        static readonly string aoaiApiVersion = "vesrion";
        static readonly string aoaiApiKey = "oai key";

        static async Task Main(string[] args)
        {
            string[] inputFiles = { "resources/chat_input_1.wav", "resources/chat_input_2.wav" };
            string topic = "describe working dogs";
            string referenceText = "";

            // Generate response from Azure OpenAI API
            foreach (var file in inputFiles)
            {
                string userText = await SpeechToTextAsync(file);
                referenceText += userText + " ";
                Console.WriteLine("User: " + userText);

                string gptResponse = await CallGPTAsync(userText);
                Console.WriteLine("GPT: " + gptResponse);

                await TextToSpeechAsync(gptResponse, $"output/gpt_output_{Path.GetFileNameWithoutExtension(file)}.wav");
            }

            Console.WriteLine("Generating the final report...");
            await PronunciationAssessmentAsync(inputFiles, referenceText, topic);
        }

        static async Task<string> SpeechToTextAsync(string filePath)
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, serviceRegion);
            var audioConfig = AudioConfig.FromWavFileInput(filePath);
            var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            return result.Text;
        }

        static async Task<string> CallGPTAsync(string userText)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://{aoaiResourceName}.openai.azure.com/openai/deployments/{aoaiDeploymentName}/chat/completions?api-version={aoaiApiVersion}";
                client.DefaultRequestHeaders.Add("api-key", aoaiApiKey);

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    messages = new[]
                    {
                        new { role = "system", content = "You are a voice assistant, and when you answer questions, your response should not exceed 25 words." },
                        new { role = "user", content = userText }
                    }
                }), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(await response.Content.ReadAsStringAsync());
                return result["choices"][0]["message"]["content"];
            }
        }

        static async Task TextToSpeechAsync(string text, string outputPath)
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, serviceRegion);
            var audioConfig = AudioConfig.FromWavFileOutput(outputPath);
            var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig);

            var ssmlText = $@"
                <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                    <voice name='en-US-JennyNeural'>{text}</voice>
                </speak>";
            await synthesizer.SpeakSsmlAsync(ssmlText);
        }

        static async Task PronunciationAssessmentAsync(string[] inputFiles, string referenceText, string topic)
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, serviceRegion);
            var streamFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);

            using (var audioStream = AudioInputStream.CreatePushStream(streamFormat))
            using (var audioConfig = AudioConfig.FromStreamInput(audioStream))
            using (var recognizer = new SpeechRecognizer(speechConfig, "en-US", audioConfig))
            {
                foreach (var file in inputFiles)
                {
                    byte[] audioData = File.ReadAllBytes(file);
                    audioStream.Write(audioData);
                    Thread.Sleep(100);  // Adjust sleep for real-time streaming

                    var result = await recognizer.RecognizeOnceAsync();
                    Console.WriteLine($"Recognized Text: {result.Text}");

                    var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(referenceText, GradingSystem.HundredMark, Granularity.Phoneme);
                    pronunciationAssessmentConfig.ApplyTo(recognizer);
                }
            }

            // Generate your report based on results; you may need to customize this further for prosody and fluency.
            Console.WriteLine("Pronunciation assessment completed.");
        }
    }
}
