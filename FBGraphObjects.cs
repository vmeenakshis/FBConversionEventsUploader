using System.Collections.Generic;

namespace FBGrapher
{
    public class PostOfflineEventsResponse
	{
		public PostOfflineEventsResponse() { }
		public long Count { get; set; }
	}
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
}
