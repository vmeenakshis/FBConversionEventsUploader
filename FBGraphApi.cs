using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FBGrapher
{
    public class FBGraphApi
	{
        #region Public Props
        public List<OfflineEvent> fbOfflineEvents { get; set; } = new List<OfflineEvent>();
        public string fbGraphOfflineEventsApiUrl { get; set; } = "https://graph.facebook.com/v13.0/{0}/events";
        public string CsvFilePath { set { ReadCsvFile(value); } }
        #endregion

        #region Public Methods 
        public List<string> startUploading(string EventID, string AccessToken, int BatchSize=1000)
        {
            List<string> ResponseMsgStr = new List<string>();
            SplitIntoBatches(fbOfflineEvents, BatchSize).ForEach(batch =>
            {
                try
                {
                    ResponseMsgStr.Add(simplePost(AccessToken, batch, EventID).ToString());
                }
                catch (Exception e1)
                {
                    ResponseMsgStr.Add(e1.ToString());
                }
            });

            return ResponseMsgStr;
        }
        public void prepareTestEvents()
        {
            OfflineEvent oe = new OfflineEvent
            {
                match_keys = new MatchKeys()
                {
                    email = new List<string>() { GenerateSHA256("twissnn_89@hotmail.com") },
                    phone = new List<string>() { GenerateSHA256("23556373") },
                    fn = GenerateSHA256("Lasse"),
                    ln = GenerateSHA256("Twatmann bildtrup"),
                    zip = GenerateSHA256("5000"),
                    ct = GenerateSHA256("Haarby"),
                    country = GenerateSHA256("DK")
                },
                currency = "DKK",
                custom_data = new CustomData() { event_source = "-test-" },
                event_name = "Purchase",
                event_time = 1651332550,
                value = 158.90,
                contents = new List<Content>()
            };

            fbOfflineEvents.Add(oe);

            oe = new OfflineEvent
            {
                match_keys = new MatchKeys()
                {
                    email = new List<string>() { GenerateSHA256("test_89@hotmail.com") },
                    phone = new List<string>() { GenerateSHA256("23556373") },
                    fn = GenerateSHA256("tesrer"),
                    ln = GenerateSHA256("dataere"),
                    zip = GenerateSHA256("5000"),
                    ct = GenerateSHA256("Haarby"),
                    country = GenerateSHA256("DK")
                },
                currency = "DKK",
                custom_data = new CustomData() { event_source = "-test-" },
                event_name = "Purchase",
                event_time = 1651332555,
                value = 200.90
            };

            fbOfflineEvents.Add(oe);
        }
        #endregion

        #region Private Methods
        private void ReadCsvFile(string FilePath)
        {
            StreamReader sr = new StreamReader(FilePath);
            fbOfflineEvents = new List<OfflineEvent>();
            string line;
            string[] row = new string[5];
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(',');
                if (String.Equals(row[0], "email")) continue;

                fbOfflineEvents.Add(new OfflineEvent
                {
                    match_keys = new MatchKeys()
                    {
                        email = new List<string>() { GenerateSHA256(row[0]) },
                        phone = new List<string>() { GenerateSHA256(row[1]) },
                        fn = GenerateSHA256(row[2]),
                        ln = GenerateSHA256(row[3]),
                        zip = GenerateSHA256(row[4]),
                        ct = GenerateSHA256(row[5]),
                        country = GenerateSHA256(row[6])
                    },
                    event_name = row[7],
                    event_time = int.Parse(row[8]),
                    value = Double.Parse(row[9]),
                    currency = row[10],
                    contents = new List<Content>(),
                    custom_data = new CustomData() { event_source = "-test-" }
                });
            }
        }
        private List<List<OfflineEvent>> SplitIntoBatches(List<OfflineEvent> lstEvents, int batchSize)
		{
			List<List<OfflineEvent>> lstStudentsInBatches = new List<List<OfflineEvent>>();

			// Determine how many lists are required
			int numberOfLists = (lstEvents.Count / batchSize);
			int remainder = (lstEvents.Count % batchSize);

			if (remainder > 0)
				numberOfLists += 1;

			for (int i = 0; i < numberOfLists; i++)
			{
				List<OfflineEvent> newList = lstEvents.Skip(i * batchSize).Take(batchSize).ToList();
				lstStudentsInBatches.Add(newList);
			}
			return lstStudentsInBatches;
		}
        private static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        private string simplePost(string token, List<OfflineEvent> oData, string eventID)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                var Parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("access_token", token),
                        new KeyValuePair<string, string>("upload_tag", "AutoUpload"),
                        new KeyValuePair<string, string>("data", JsonSerializer.Serialize(oData)),
                    };

                var Request = new HttpRequestMessage(HttpMethod.Post, string.Format(fbGraphOfflineEventsApiUrl, eventID))
                {
                    Content = new FormUrlEncodedContent(Parameters)
                };

                return client.SendAsync(Request).Result.Content.ReadAsStringAsync().Result;
            }
        }
        #endregion
    }
}
