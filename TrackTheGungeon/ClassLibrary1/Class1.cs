using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using MonoMod.RuntimeDetour;
using System.Collections;

namespace TrackTheGungeon
{
    public class Class1 : ETGModule
    {
		private string baseUrl;
		private System.Net.WebClient client;

		// Initializes things internal to my mod
		public override void Init()
		{
			Console.WriteLine($"TEST TrackTheGungeon.init()");
			// query is ?user
			// hook DoQuickRestart
			// hook DoMainMenu
			client = new System.Net.WebClient();
			client.Headers[HttpRequestHeader.ContentType] = "application/json";

			baseUrl = "http://127.0.0.1:8000";
			Hook hook = new Hook(
				typeof(GameManager).GetMethod("DoGameOver", BindingFlags.Public | BindingFlags.Instance),
				typeof(Class1).GetMethod("DoGameOverData", BindingFlags.Public | BindingFlags.Instance),
				this
			);

			
		}
		
		// Func<gameMangager, string, string(?)>
		// ^ dont use func, use Action if your return is void!!!
		public void DoGameOverData(Action<GameManager, string> orig, GameManager self, string gameOverSource = "")
		{
			client.UploadString(
				new System.Uri(baseUrl + "/runEnd", uriKind: UriKind.Absolute),
				"{\"guns\": \"GUNS\"}");
			orig(self, gameOverSource);
		}

		public void GetRequest(string url, string user)
		{
			// System.Uri uri = new System.Uri(url + "?user=GUNGEON");
			var response = client.DownloadString(url + "/track" + "?user=" + user) ;
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
