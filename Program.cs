using FBGrapher;
using System;

namespace fbGraphConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to FB Graph Integration!");
            string _eventId = "<<FBOfflineEventSetID">>;
            string _aToken = "<<AdminSuperUserAccessToken>>";

            FBGraphApi _fbGraphApi = new();

            //===========> Option 1. Directly Provide your List of fbOfflineEvents here
            //_fbGraphApi.fbOfflineEvents = "Provide your list of offline events here";

            //===========> Option 2. Provide your csv path
            _fbGraphApi.CsvFilePath = @"d:\myevents.csv";

            //===========> Option 3. Use the Test events
            //_fbGraphApi.prepareTestEvents();

            //START Uploading
            _fbGraphApi.startUploading(_eventId, _aToken, 2).ForEach(res => Console.WriteLine(res));
        }
    }
}
