using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech_Console
{
    class GroundedDetection
    {
        // Replace with your Azure Content Safety API endpoint and key
        private static readonly string endpoint = "https://textguardian.cognitiveservices.azure.com/";
        private static readonly string apiKey = "8cedaabd384d43f3af735f1e1d66a5ac";

       
        //static async Task Main(string[] args)
        //{
        //    string groundednessResult = await DetectGroundednessAsync();
        //    Console.WriteLine(groundednessResult);
        //}

        public static async Task<string> DetectGroundednessAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //payload 
               var requestContent = new JObject
               {
                   ["domain"] = "Medical",
                   ["task"] = "Summarization",
                   ["text"] = "Ms Johnson has been in the hospital after experiencing a heart attack.",
                   ["groundingSources"] = new JArray
                   {
                        "Our patient, Ms. Johnson, presented with persistent fatigue, unexplained weight loss, and frequent night sweats. After a series of tests, she was diagnosed with Hodgkin’s lymphoma, a type of cancer that affects the lymphatic system. The diagnosis was confirmed through a lymph node biopsy revealing the presence of Reed-Sternberg cells, a characteristic of this disease. She was further staged using PET-CT scans. Her treatment plan includes chemotherapy and possibly radiation therapy, depending on her response to treatment. The medical team remains optimistic about her prognosis given the high cure rate of Hodgkin’s lymphoma."
                   },
                   ["reasoning"] = false
               };

                // JSON payload as defined in your  example
                // Payload object

                // var requestContent = new
                // {
                //     domain = "Generic",
                //     task = "QnA",
                //     qna = new
                //     {
                //         query = "How much does she currently get paid per hour at the bank?"
                //     },
                //     text = "12/hour",
                //     groundingSources = new[]
                //{
                //         "I'm 21 years old and I need to make a decision about the next two years of my life. Within a week. I currently work for a bank that requires strict sales goals to meet. IF they aren't met three times (three months) you're canned. They pay me 10/hour and it's not unheard of to get a raise in 6ish months. The issue is, **I'm not a salesperson**. That's not my personality. I'm amazing at customer service, I have the most positive customer service \"reports\" done about me in the short time I've worked here. A coworker asked \"do you ask for people to fill these out? you have a ton\". That being said, I have a job opportunity at Chase Bank as a part time teller. What makes this decision so hard is that at my current job, I get 40 hours and Chase could only offer me 20 hours/week. Drive time to my current job is also 21 miles **one way** while Chase is literally 1.8 miles from my house, allowing me to go home for lunch. I do have an apartment and an awesome roommate that I know wont be late on his portion of rent, so paying bills with 20hours a week isn't the issue. It's the spending money and being broke all the time.\n\nI previously worked at Wal-Mart and took home just about 400 dollars every other week. So I know i can survive on this income. I just don't know whether I should go for Chase as I could definitely see myself having a career there. I'm a math major likely going to become an actuary, so Chase could provide excellent opportunities for me **eventually**."
                //     },
                //     reasoning = false
                // };



                HttpContent content = new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/contentsafety/text:detectGroundedness?api-version=2024-09-15-preview", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
        }
    }
}
