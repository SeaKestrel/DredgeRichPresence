using System;
using System.Collections.Generic;
using DiscordRPC;
using UnityEngine;
using Winch.Core;

namespace DredgeRichPresence
{
	public class DredgeRichPresence : MonoBehaviour
	{

		public static DredgeRichPresence Instance { get; private set; }

		[NonSerialized]
		private static DiscordRpcClient client;

		[NonSerialized]
		public Dictionary<string, RichPresence> richPresences = [];

		[NonSerialized]
		private static Timestamps timeStarted;

		[NonSerialized]
		private ZoneEnum CurrentZone;

		[NonSerialized]
		public Action<ZoneEnum> OnZoneChanged;

		[NonSerialized]
		private bool InGame;
		private bool DayTime;

		public void Awake()
		{
			Instance = this;

			GameManager.Instance.OnGameStarted += GameStarted;

			InGame = false;

			// Creating the instance with the app id from discord
			client = new DiscordRpcClient("1232072940418371614");
			WinchCore.Log.Info("Client created.");

			// Calling the init function
			client.Initialize();
			WinchCore.Log.Info("Client initialized.");

			client.OnReady += (sender, msg) =>
			{
				//Create some events so we know things are happening
				Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
			};

			// Saving the timestamp when the game started
			timeStarted = Timestamps.Now;

			// Initialize presences
			InitializePresences();
			WinchCore.Log.Info("Presences initialized.");

			// Setting the presence for the main menu
			client.SetPresence(richPresences["main_menu"]);

			OnZoneChanged += ZoneChangedHandler;

			WinchCore.Log.Debug($"{nameof(DredgeRichPresence)} has loaded!");
		}

		public void Update()
		{
			if (Instance == null) return;
			if (!InGame) return;

			ParseLocation();
		}

		/// <summary>
		/// This function initialize the presences and add them to the presence dictionnary
		/// </summary>
		private void InitializePresences()
		{
			richPresences.Add("main_menu", new RichPresence() { Details = "Main Menu", Assets = new Assets() { LargeImageKey = "main_menu", LargeImageText = "Main Menu", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("the_marrows", new RichPresence() { State = "The Marrows", Details = "Sailing", Assets = new Assets() { LargeImageKey = "the_marrows", LargeImageText = "at The Marrows", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("open_ocean", new RichPresence() { State = "The ocean", Details = "Sailing", Assets = new Assets() { LargeImageKey = "open_ocean", LargeImageText = "In the ocean", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("gale_cliffs", new RichPresence() { State = "Gale Cliffs", Details = "Sailing", Assets = new Assets() { LargeImageKey = "gale_cliffs", LargeImageText = "at Gale Cliffs", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("twisted_strand", new RichPresence() { State = "Twisted Strand", Details = "Sailing", Assets = new Assets() { LargeImageKey = "twisted_strand", LargeImageText = "at Twisted Strand", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("devils_spine", new RichPresence() { State = "Devil's Spine", Details = "Sailing", Assets = new Assets() { LargeImageKey = "devils_spine", LargeImageText = "at Devil's Spine", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("stellar_basin", new RichPresence() { State = "Stellar Basin", Details = "Sailing", Assets = new Assets() { LargeImageKey = "bassin_astral", LargeImageText = "at Stellar Basin", SmallImageKey = "main_menu" }, Timestamps = timeStarted });

			/*
			 * Docks
			 */

			// The marrows
			richPresences.Add("dock_greater_marrow", new RichPresence() { State = "Greater Marrow", Details = "Docked", Assets = new Assets() { LargeImageKey = "the_marrows", LargeImageText = "Docked at the Greater Marrow", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_little_marrow", new RichPresence() { State = "Little Marrow", Details = "Docked", Assets = new Assets() { LargeImageKey = "the_marrows", LargeImageText = "Docked at the Little Marrow", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_outcast_isle", new RichPresence() { State = "Blackstone Isle", Details = "Docked", Assets = new Assets() { LargeImageKey = "outcast_isle", LargeImageText = "Docked at the Blackstone isle", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_steel_point", new RichPresence() { State = "Steel Point", Details = "Docked", Assets = new Assets() { LargeImageKey = "the_marrows", LargeImageText = "Docked at Steel Point", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			//richPresences.Add("dock_greater_marrow", new RichPresence() { State = "Greater Marrow", Details = "Docked", Assets = new Assets() { LargeImageKey = "the_marrows", LargeImageText = "Docked at the Greater Marrow" } , Timestamps = timeStarted });

			// Gale Cliffs
			richPresences.Add("dock_old_mayor_gc", new RichPresence() { State = "Gale Cliffs' Old Camp", Details = "Docked", Assets = new Assets() { LargeImageKey = "gale_cliffs", LargeImageText = "Docked at the Gale Cliffs' Old Camp", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_pontoon_gc", new RichPresence() { State = "Dusty Pontoon", Details = "Docked", Assets = new Assets() { LargeImageKey = "gale_cliffs", LargeImageText = "Docked at the Gale Cliffs' Pontoon", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_gale_cliffs", new RichPresence() { State = "Ruins", Details = "Docked", Assets = new Assets() { LargeImageKey = "gale_cliffs", LargeImageText = "Docked at the Gale Cliffs' Ruins", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_ingfell", new RichPresence() { State = "Ingfell", Details = "Docked", Assets = new Assets() { LargeImageKey = "gale_cliffs", LargeImageText = "Docked at Ingfell", SmallImageKey = "main_menu" }, Timestamps = timeStarted });

			// Devils spine
			richPresences.Add("dock_ds_temple", new RichPresence() { State = "Ancient Temple", Details = "Docked", Assets = new Assets() { LargeImageKey = "devils_spine", LargeImageText = "Docked at the Ancient Temple", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_pontoon_ds", new RichPresence() { State = "Charred Pontoon", Details = "Docked", Assets = new Assets() { LargeImageKey = "devils_spine", LargeImageText = "Docked at the Charred Pontoon", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_ancient_ruins", new RichPresence() { State = "Ancient Ruins", Details = "Docked", Assets = new Assets() { LargeImageKey = "devils_spine", LargeImageText = "Docked at the Ancient Ruins", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_old_mayor_ds", new RichPresence() { State = "Devil's Spine Old Camp", Details = "Docked", Assets = new Assets() { LargeImageKey = "devils_spine", LargeImageText = "Docked at the Devil's Spine's Old Camp", SmallImageKey = "main_menu" }, Timestamps = timeStarted });

			// Twisted strand
			richPresences.Add("dock_old_mayor_ts", new RichPresence() { State = "Twisted Strand Old Camp", Details = "Docked", Assets = new Assets() { LargeImageKey = "twisted_strand", LargeImageText = "Docked at the Twisted Strand's Old Camp", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_pontoon_ts", new RichPresence() { State = "Rickety Pontoon", Details = "Docked", Assets = new Assets() { LargeImageKey = "twisted_strand", LargeImageText = "Docked at the Rickety Pontoon", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_soldier_camp", new RichPresence() { State = "Camp", Details = "Docked", Assets = new Assets() { LargeImageKey = "twisted_strand", LargeImageText = "Docked at the Soldier Camp", SmallImageKey = "main_menu" }, Timestamps = timeStarted });

			// Stellar Basin
			richPresences.Add("dock_old_mayor_sb", new RichPresence() { State = "Stellar Basin Old Camp", Details = "Docked", Assets = new Assets() { LargeImageKey = "bassin_astral", LargeImageText = "Docked at the Stellar Basin Old Camp", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_pontoon_sb", new RichPresence() { State = "Starlight Pontoon", Details = "Docked", Assets = new Assets() { LargeImageKey = "bassin_astral", LargeImageText = "Docked at the Starlight Pontoon", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_old_fort", new RichPresence() { State = "Old Fortress", Details = "Docked", Assets = new Assets() { LargeImageKey = "bassin_astral", LargeImageText = "Docked at the Old Fortress", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
			richPresences.Add("dock_research_pontoon", new RichPresence() { State = "Research Outpost", Details = "Docked", Assets = new Assets() { LargeImageKey = "bassin_astral", LargeImageText = "Docked at the Research Outpost", SmallImageKey = "main_menu" }, Timestamps = timeStarted });
		}

		/// <summary>
		/// This function parse the location and trigger the event
		/// </summary>
		private void ParseLocation()
		{
			if (CurrentZone == GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone()) return;
			// If the zone is different, change it and update
			CurrentZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
			OnZoneChanged?.Invoke(CurrentZone); // Call the event
		}

		/// <summary>
		/// This function handles the OnGameStarted event
		/// </summary>
		private void GameStarted()
		{
			CurrentZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone(); // Setting the current zone where the player is
			InGame = true; // saving the state of the game

			GameEvents.Instance.OnPlayerDockedToggled += PlayerDockedEvent; // Adding my handler to the original event

			PlayerDockedEvent(GameManager.Instance.Player.CurrentDock); // Calling the function to set the presence
		}

		/// <summary>
		/// This function handles the OnZoneChanged event
		/// </summary>
		/// <param name="zone">The current zone</param>
		private void ZoneChangedHandler(ZoneEnum zone)
		{
			//WinchCore.Log.Debug("ZoneChangedHandler()");
			SetRichPresence(zone.ToString().ToLower()); // Parsing the zone name (THE_MARROWS -> the_marrows)
		}

		/// <summary>
		/// This function handles the OnPlayerDockedToggled event
		/// </summary>
		/// <param name="dock">The dock the player is at</param>
		private void PlayerDockedEvent(Dock dock)
		{
			//WinchCore.Log.Debug("PlayerDockedEvent()");
			if (dock == null) // If the player isn't docked anymore
			{
				ZoneChangedHandler(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone());
				return;
			}
			string name = dock.dockData.id.Replace(".", "_").Replace("-", "_"); // parsing the dock's name (dock.greater-marrow -> dock_greater_marrow)
			client.SetPresence(richPresences[name]); // Setting the presence from the richPresences list
		}

		/// <summary>
		/// This function sets the rich presence from the zone id
		/// </summary>
		/// <param name="id">the id of the presence</param>
		private void SetRichPresence(string id)
		{
			client.SetPresence(richPresences[id]); // Set the presence
		}
	}
}
