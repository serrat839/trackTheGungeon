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
			// Create new webclient ONCE for use throughout game session
			client = new System.Net.WebClient();
			client.Headers[HttpRequestHeader.ContentType] = "application/json";

			// Set the base uri/url for the webserver
			baseUrl = "http://127.0.0.1:8000";
			
			// Hook the gameover function.
			// Maybe it would be better to hook the OpenAmmonomicon function, this way victory is handled as well
			Hook hook = new Hook(
				typeof(GameManager).GetMethod("DoGameOver", BindingFlags.Public | BindingFlags.Instance),
				typeof(Class1).GetMethod("DoGameOverData", BindingFlags.Public | BindingFlags.Instance),
				this
			);

			
		}

		// Func<gameMangager, string, string(?)>
		// ^ dont use func, use Action if your return is void!!!
		// Replacement function for DoGameOver that sends user's run data to webserver
		// orig - Original DoGameOver function, automatically passed by hook
		// GameManager self - original object this method is from
		// String gameOverSource - source of game over
		public void DoGameOverData(Action<GameManager, string> orig, GameManager self, string gameOverSource = "")
		{
			// for some reason, the crosshair clock winding is not winding completely/for a long time
			// when using this mod. Does the crosshair wind longer when the session is longer????
			orig(self, gameOverSource);

			ETGModConsole.Log("Sorting my run data...");
			var x = GameManager.Instance.PrimaryPlayer;
			string items = "{";

			// I know this is kinda garbage code but I am about to go to work so i couldnt be bothered to fix it
			items += "\"passive\": [";
			if (x.passiveItems.Count > 0)
			{ 
				items += String.Format("\"{0}\"", x.passiveItems[0].EncounterNameOrDisplayName);
				for (int i = 1; i < x.passiveItems.Count; i++)
				{
					items += ", ";
					items += String.Format("\"{0}\"", x.passiveItems[i].EncounterNameOrDisplayName);
				}
			}
			items += "],";

			ETGModConsole.Log("Still vibing1");

			items += "\"active\": [";
			if (x.activeItems.Count > 0)
			{
				items += String.Format("\"{0}\"", x.activeItems[0].EncounterNameOrDisplayName);
				for (int i = 1; i < x.activeItems.Count; i++)
				{
					items += ", ";
					items += String.Format("\"{0}\"", x.activeItems[i].EncounterNameOrDisplayName);
				}
			}
			items += "],";

			ETGModConsole.Log("Still vibing2");

			items += "\"guns\": [";
			if (x.inventory.AllGuns.Count > 0)
			{
				items += String.Format("\"{0}\"", x.inventory.AllGuns[0].EncounterNameOrDisplayName);
				for (int i = 1; i < x.inventory.AllGuns.Count; i++)
				{
					items += ", ";
					items += String.Format("\"{0}\"", x.inventory.AllGuns[i].EncounterNameOrDisplayName);
				}
			}
            items += "]";

            items += "}";


			ETGModConsole.Log("Storing Run Data...");
			ETGModConsole.Log(items);


			// some kind of processing here of user inventory
			// Send a string formatted as JSON to webclient
			client.UploadStringAsync(
				new System.Uri(baseUrl + "/runEnd", uriKind: UriKind.Absolute),
				items);
		}


		// Function to test GET requests
		// url - string containing url of webserver
		// user - string containing user information
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
