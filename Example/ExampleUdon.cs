using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nappollen.UdonInspector.Example {
	public class ExampleUdon : UdonSharpBehaviour {
		private void Start() {
			Debug.Log("ExampleUdon start");
			if (Networking.LocalPlayer != null) {
				Debug.Log("ExampleUdon local player is " + Networking.LocalPlayer.displayName);
			} else Debug.Log("ExampleUdon local player is null");
		}
	}
}