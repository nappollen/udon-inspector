namespace Nappollen.UdonInspector.Editor {
	public static class ChatGptPrompt {
		private const string Template =
			"You are an expert in VRChat UdonSharp. Convert the following Udon assembly into clean, idiomatic UdonSharp C# code.\n\n" +
			"Requirements:\n" +
			"1. Output valid UdonSharp C# that compiles without errors.\n" +
			"2. Preserve all variable names from the .data_start section.\n" +
			"3. Reconstruct methods from exported entry points (.export directives).\n" +
			"4. Replace EXTERN calls with their equivalent UdonSharp / VRChat SDK API calls.\n" +
			"5. Replace JUMP_IF_FALSE and JUMP with structured if/else, loops, or early returns.\n" +
			"6. Use PUSH/POP sequences to reconstruct expression evaluation and method arguments.\n" +
			"7. Infer variable types from heap type annotations in the .data_start section.\n" +
			"8. Add the [UdonBehaviourSyncMode] attribute with an appropriate sync mode based on synced variables.\n" +
			"9. Do not include any assembly comments in the output; only emit clean C# code.\n" +
			"10. If something cannot be cleanly reconstructed, add a TODO comment with a brief explanation.\n\n" +
			"Assembly:\n" +
			"```\n" +
			"{assembly_code}\n" +
			"```";

		public static string Build(string assemblyCode)
			=> Template.Replace("{assembly_code}", assemblyCode);
	}
}
