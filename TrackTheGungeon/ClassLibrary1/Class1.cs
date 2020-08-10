using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Policy;

namespace TrackTheGungeon
{
    public class Class1 : ETGModule
    {
		private string baseUrl;

		// Initializes things internal to my mod
		public override void Init()
		{
			Console.WriteLine($"TEST TrackTheGungeon.init()");
			// query is ?user
			// hook DoQuickRestart
			// hook DoMainMenu
			baseUrl = "http://127.0.0.1:8000/track";
			//client.Headers.Add("");
			// string s = Encoding.ASCII.GetString(client.UploadData(baseUrl, "GET", ""));
			GetRequest(baseUrl);
		}

		public static void GetRequest(string url)
		{
			System.Net.WebClient client = new System.Net.WebClient();
			// System.Uri uri = new System.Uri(url + "?user=GUNGEON");
			var response = client.DownloadString(url + "?user=GUNGEON") ;
		}

		// Called after mods are initialized. Allows interaction between mods
		public override void Start()
		{
			Console.WriteLine($"TEST TrackTheGungeon.Start()");
		}

		// Called when game exits
		public override void Exit()
		{
			Console.WriteLine($"TEST TrackTheGungeon.Exit()");
		}
	}
}
