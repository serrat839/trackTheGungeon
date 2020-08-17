using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using MonoMod.RuntimeDetour;

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

		/**
		 * Replacement function for DoGameOver that sends user's run data to webserver
         * Action<GameManager, string> orig - Original DoGameOver function, automatically passed by hook. Action is
         *  used since return is void
         * GameManager self - original object this method is from
         * String gameOverSource - source of game over
		 */
		public void DoGameOverData(Action<GameManager, string> orig, GameManager self, string gameOverSource = "")
		{
			// Begin gameover sequence
			orig(self, gameOverSource);



			// Parse player data and send it to the server
			var player = GameManager.Instance.PrimaryPlayer;
			var stats = GameStatsManager.Instance;
			string schema = "v1.0";

			string items = "{";
			items += String.Format("\"{0}\":  \"{1}\", ", "schema", schema);
			items += String.Format("\"{0}\":  {1}, ", "metadata", GameMetaJSON(
				player.characterIdentity.ToString(),
				stats.GetSessionStatValue(TrackedStats.ENEMIES_KILLED).ToString(),
				GameManager.Instance.Dungeon.DungeonShortName,
				player.carriedConsumables.Currency.ToString(),
				stats.GetSessionStatValue(TrackedStats.TOTAL_MONEY_COLLECTED).ToString(),
				stats.IsRainbowRun.ToString(),
				player.CharacterUsesRandomGuns.ToString(),
				stats.isTurboMode.ToString(),
				ChallengeManager.CHALLENGE_MODE_ACTIVE.ToString()
			));

			items += Jsonify("passive", player.passiveItems.ConvertAll(obj => (PickupObject)obj));
			items += Jsonify("active", player.activeItems.ConvertAll(obj => (PickupObject)obj));
			items += Jsonify("guns", player.inventory.AllGuns.ConvertAll(obj => (PickupObject)obj), true);
            items += "}";
						

			// Send user data to server
			client.UploadStringAsync(
				new System.Uri(baseUrl + "/runEnd", uriKind: UriKind.Absolute),
				items);
		}

		/**
		 * Helper function to organize and JSONIFY run data
		 * string gungeoneer - The gungeoneer being used
		 * string kills - number of enemies killed
		 * string carried_money - carried money
		 * string total_money - total money acquired throughout run
		 * string rainbow - rainbow run boolean
		 * string blessed - blessed run boolean
		 * string turbo - turbo run boolean
		 * string challenge - challenge run boolean 
		 */
		private string GameMetaJSON(string gungeoneer, string kills, string floor, string carried_money, string total_money, string rainbow, string blessed, string turbo, string challenge)
        {
			string jsonData = "{";
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "gungeoneer", gungeoneer);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "kills", kills);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "floor", floor);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "carried_money", carried_money);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "total_money", total_money);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "rainbow", rainbow);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "blessed", blessed);
			jsonData += String.Format("\"{0}\":  \"{1}\", ", "turbo", turbo);
			jsonData += String.Format("\"{0}\":  \"{1}\"", "challenge", challenge);
			jsonData += "}";
			return jsonData;
        }

		/**
		 * Helper function to convert lists into json formatted strings
		 * string category - The key that the JSON will use
		 * List<PickupObject> items - A list of PickupObjects that we want to convert to a JSON string
		 * Boolean last - Controls wether or not our data should be followed by a comma
		 */
		private string Jsonify(string category, List<PickupObject> items, Boolean last = false)
        {
			// set the key
			string jsonString = String.Format("\"{0}\": [", category);

			// if our list actually exists
			if (items.Count > 0)
			{

				// fencepost the first item then cycle through rest of list
				jsonString += String.Format("\"{0}\"", items[0].EncounterNameOrDisplayName);
				for (int i = 1; i < items.Count; i++)
				{
					jsonString += ", ";
					jsonString += String.Format("\"{0}\"", items[i].EncounterNameOrDisplayName);
				}
			}
			jsonString += "]";

			// if not last element of json...
			if (!last)
            {
				jsonString += ",";
            }
			return jsonString;
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
