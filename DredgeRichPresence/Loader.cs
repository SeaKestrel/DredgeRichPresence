using DredgeRichPresence.Libs;
using HarmonyLib;
using UnityEngine;

namespace DredgeRichPresence
{
	public class Loader
	{
		/// <summary>
		/// This method is run by Winch to initialize your mod
		/// </summary>
		public static void Initialize()
		{
			var gameObject = new GameObject(nameof(DredgeRichPresence));
			gameObject.AddComponent<DredgeRichPresence>();
			gameObject.AddComponent<CustomSceneManager>();
			GameObject.DontDestroyOnLoad(gameObject);
            new Harmony(nameof(DredgeRichPresence)).PatchAll();
        }
	}
}