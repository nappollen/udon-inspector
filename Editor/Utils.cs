using System.Text.RegularExpressions;

namespace Nappollen.UdonInspector.Editor {
	public static class Utils {
		public static string EscapeName(string name)
			=> Regex.Replace(name ?? "", @"[^a-zA-Z0-9_]", "_");
	}
}
