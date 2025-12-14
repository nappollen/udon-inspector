using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Nappollen.UdonInspector.Editor {
	public class AssemblerExtractor {
		public static bool Extract(IUdonProgram program, out string result) {
			var str = new StringBuilder();
			var entryPoints = program.EntryPoints.GetSymbols()
				.Select(e => (e, program.EntryPoints.GetAddressFromSymbol(e)))
				.ToArray();

			str.Append("----------: // Extracted with Nappollen's Udon Inspector\n");
			str.AppendLine($"----------: // Program: {program.InstructionSetIdentifier} v{program.InstructionSetVersion} \n");
			str.AppendLine("----------: .data_start");

			var symbols = program.SymbolTable.GetSymbols()
				.ToArray()
				.Select(e => (e, program.SymbolTable.GetAddressFromSymbol(e)))
				.OrderBy(e => e.Item2)
				.ToArray();

			foreach (var sym in symbols) {
				var type  = program.Heap.GetHeapVariableType(sym.Item2);
				var value = program.Heap.GetHeapVariable(sym.Item2);
				str.AppendLine($"0x{sym.Item2:X8}:   {sym.Item1}: {FormatValue(type)}, {FormatValue(value)}");
			}

			str.AppendLine("----------: .data_end\n");
			str.AppendLine("----------: .code_start");

			for (var i = 0; i < program.ByteCode.Length; i += 4) {
				// check has entry point
				var entryPoint = entryPoints.FirstOrDefault(e => e.Item2 == i);
				if (entryPoint != default) {
					str.AppendLine($"0x{i:X8}:   .export {entryPoint.Item1}");
					str.AppendLine($"0x{i:X8}:   {entryPoint.Item1}:");
				}

				// read opcode
				var op = FromBytes(program.ByteCode, i);
				str.Append($"0x{i:X8}:     ");
				object symbol;

				var address = 0u;
				switch (op) {
					case 0x00:
						str.AppendLine("NOP");
						break;
					case 0x01:
						address = FromBytes(program.ByteCode, i += 4);
						try {
							symbol = program.SymbolTable.GetSymbolFromAddress(address);
						} catch {
							symbol = program.Heap.GetHeapVariable(address);
						}

						str.AppendLine($"PUSH, {symbol ?? "0x" + address.ToString("X8")}");
						break;
					case 0x02:
						str.AppendLine("POP");
						break;
					case 0x04:
						str.AppendLine($"JUMP_IF_FALSE, {FormatValue(FromBytes(program.ByteCode, i += 4))}");
						break;
					case 0x05:
						str.AppendLine($"JUMP, {FormatValue(FromBytes(program.ByteCode, i += 4))}");
						break;
					case 0x06:
						address = FromBytes(program.ByteCode, i += 4);
						symbol  = program.Heap.GetHeapVariable(address);
						str.AppendLine($"EXTERN, {FormatValue(symbol ?? address, false)}");
						break;
					case 0x07:
						str.AppendLine($"ANNOTATION, {FormatValue(FromBytes(program.ByteCode, i += 4))}");
						break;
					case 0x08:
						address = FromBytes(program.ByteCode, i += 4);
						symbol  = program.SymbolTable.GetSymbolFromAddress(address);
						str.AppendLine($"JUMP_INDIRECT, {symbol ?? "0x" + address.ToString("X8")}");
						break;
					case 0x09:
						str.AppendLine("COPY");
						break;
					default:
						str.AppendLine($"UNKNOWN {FormatValue(op)}");
						break;
				}
			}

			str.AppendLine("----------: .code_end\n");
			str.Append("----------: // End of extraction");
			result = str.ToString().TrimEnd();

			return true;
		}

		private static string FormatValue(object value, bool minimize = true)
			=> value switch {
				Type t => $"%{t.FullName?.Replace(".", "")}",
				// to base64
				string { Length: > 64 } s when minimize
					=> $"base64:\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(s))}\"",
				string s  => $"\"{s}\"",
				bool b    => b ? "true" : "false",
				null      => "null",
				uint u    => $"0x{u:X8}",
				int i     => $"0x{i:X8}",
				long l    => $"0x{l:X8}",
				ulong ul  => $"0x{ul:X8}",
				ushort us => $"0x{us:X4}",
				short ss  => $"0x{ss:X4}",
				byte b    => $"0x{b:X8}",
				sbyte sb  => $"0x{sb:X8}",
				_ when value.GetType().IsEnum
					=> $"{value.GetType().FullName?.Replace(".", "")}.{value}",
				_ when value.GetType().IsArray
					=> $"[{string.Join(", ", (value as Array ?? Array.Empty<object>()).Cast<object>().Select(e => FormatValue(e, minimize)))}]",
				_ => value.ToString()
			};

		private static uint FromBytes(byte[] bytes, int startIndex)
			=> (uint)(bytes[startIndex + 3]
				| (startIndex          + 2 < bytes.Length ? bytes[startIndex + 1] & 0xFF << 8 : 0)
				| (startIndex          + 1 < bytes.Length ? bytes[startIndex + 2] & 0xFF << 16 : 0)
				| (startIndex          + 0 < bytes.Length ? bytes[startIndex + 3] & 0xFF << 24 : 0));

		[MenuItem("Tools/Udon Inspector/Download All Assemblies")]
		public static void DownloadAllAssemblies() {
			Debug.Log("Downloading All Assemblies");

			var dir = Path.Combine(Application.dataPath, "UdonAssemblies");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var behaviours = UnityEngine.Object.FindObjectsOfType<UdonBehaviour>();
			foreach (var behaviour in behaviours) {
				IUdonProgram program = behaviour?.GetProgram();
				program ??= behaviour?.GetSerializedProgramAsset()?.ReadSerializedProgram();
				if (program == null) continue;
				if (!Extract(program, out var str)) continue;
				var path = Path.Combine(dir, $"{behaviour.GetInstanceID()}_{behaviour.name}.udonasm");
				File.WriteAllText(path, str);
				Debug.Log($"Udon assembly for {behaviour.name} saved to {path}");
			}

			Debug.Log("All assemblies downloaded");
		}
	}
}