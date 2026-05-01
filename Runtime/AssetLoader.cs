using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nappollen.UdonInspector.Runtime {
	public class AssetLoader : MonoBehaviour {
		[Tooltip("Path to the AssetBundle file. Surround with quotes if the path contains spaces.")]
		public string path;

		private void Start() {
			if (string.IsNullOrEmpty(path)) {
				Debug.LogWarning("[AssetLoader] No path specified.");
				return;
			}

			var bundle = AssetBundle.LoadFromFile(path);
			if (bundle == null) {
				Debug.LogError($"[AssetLoader] Failed to load AssetBundle from: {path}");
				return;
			}

			var scenePaths = bundle.GetAllScenePaths();
			if (scenePaths.Length > 0) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(scenePaths[0]);
				return;
			}

			var allAssets = bundle.LoadAllAssets<GameObject>();
			if (allAssets.Length > 0) {
				Instantiate(allAssets[0]);
				return;
			}

			Debug.LogWarning($"[AssetLoader] AssetBundle at {path} contains no scenes or prefabs.");
		}

		private void OnValidate() {
			if (!string.IsNullOrEmpty(path))
				path = path.Trim('"');
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Udon Inspector/Load AssetBundle")]
		public static void CreateAssetLoader() {
			var bundlePath = EditorUtility.OpenFilePanel("Select AssetBundle", "", "");
			if (string.IsNullOrEmpty(bundlePath)) return;

			var go     = new GameObject("AssetLoader");
			var loader = go.AddComponent<AssetLoader>();
			loader.path = bundlePath;
			EditorUtility.SetDirty(loader);
			Selection.activeGameObject = go;
			Undo.RegisterCreatedObjectUndo(go, "Create AssetLoader");
		}
#endif
	}
}
