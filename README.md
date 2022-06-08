# FBConversionEventsUploader
Upload your offline conversion event data to facebook using graph Api.

YOU NEED TO SETUP YOUR OFFLINE CONVERSION SET ID AND ADMIN USER ACCESS TOKEN IN CODE.

C# Project that process your,
1. CSV file contains max of 2000 rows of events 
2. List<offlineevents> 
  
Upload them all to Facebook graph api using batches.

Reference: 
https://developers.facebook.com/docs/marketing-api/offline-conversions#upload-events

Required C# clasess

public class OfflineEvent
	{
		public OfflineEvent() { }
		public MatchKeys match_keys { get; set; }
		public string currency { get; set; }
		public double value { get; set; }
		public string event_name { get; set; }
		public int event_time { get; set; }
		public List<Content> contents { get; set; }
		public CustomData custom_data { get; set; }
	}
	
	public class MatchKeys
	{
		public string fn { get; set; }
		public string ln { get; set; }
		public string zip { get; set; }
		public string ct { get; set; }
		public string country { get; set; }
		public List<string> email { get; set; }
		public List<string> phone { get; set; }
	}
	public class Content
	{
		public string id { get; set; } = string.Empty;
		public int quantity { get; set; } = 0;
	}
	public class CustomData
	{
		public string event_source { get; set; }
	}

A Simple POST using HTTPCLIENT Does the majic.
	
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

From FB wiki, the actual curl:  https://developers.facebook.com/docs/marketing-api/offline-conversions#upload-events

curl \
  -F 'access_token=SYSTEM_USER_ACCESS_TOKEN' \
  -F 'upload_tag=store_data' \
  -F 'data=[ \
    { 
      match_keys: {"phone": ["HASH1","HASH2"], "email": ["HASH3","HASH4"]}, 
      currency: "USD", 
      value: 16,
      event_name: "Purchase",
      event_time: 1456870902,
      contents: [
        {id: "A", quantity: 1},
        {id: "B", quantity: 2},
        {id: "C", quantity: 1}
      ]
      custom_data: {
        event_source: "in_store"
      },
    }, 
    { 
      match_keys: {"lead_id": "12345"}, 
      event_name: "Lead",
      event_time: 1446336000,
      contents: [
        {id: "A", quantity: 1},
        {id: "B", quantity: 2},
        {id: "C", quantity: 1}
      ]
      custom_data: {
        event_source: "email",
        action_type: "sent_open_click",
        email_type: "email_type_code", 
        email_provider: "gmail_yahoo_hotmail",
      }
    }, 
  ]'
  https://graph.facebook.com/VERSION/OFFLINE_EVENT_SET_ID/events
