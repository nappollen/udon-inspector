using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Nappollen.UdonInspector.Editor {
	public static class DownloadAllVariables {
		[MenuItem("Tools/Udon Inspector/Download All Variables")]
		public static void Execute() {
			Debug.Log("Downloading All Variables");

			var dir = Path.Combine(Application.dataPath, "UdonVariables");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var behaviours = Object.FindObjectsOfType<UdonBehaviour>();
			foreach (var behaviour in behaviours) {
				IUdonProgram program = behaviour?.GetProgram();
				program ??= behaviour?.GetSerializedProgramAsset()?.ReadSerializedProgram();
				if (program == null) continue;

				var symbols = program.SymbolTable.GetSymbols();
				var csv     = "Symbol,Type,Value\n";
				foreach (var symbol in symbols) {
					var address = program.SymbolTable.GetAddressFromSymbol(symbol);
					var type    = program.Heap.GetHeapVariableType(address);
					var value   = program.Heap.GetHeapVariable(address);

					var escapedSymbol = symbol.Replace(",", ";");
					var escapedType   = (type?.FullName ?? "null").Replace(",", ";");
					var escapedValue  = (value?.ToString() ?? "null").Replace(",", ";");
					csv += $"{escapedSymbol},{escapedType},{escapedValue}\n";
				}

				var fileName = $"{behaviour.GetInstanceID()}_{Utils.EscapeName(behaviour.name)}.csv";
				var path     = Path.Combine(dir, fileName);
				File.WriteAllText(path, csv);
				Debug.Log($"Variables for {behaviour.name} saved to {path}");
			}

			Debug.Log("All variables downloaded");
		}
	}
}
