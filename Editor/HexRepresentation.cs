using System.IO;

namespace Nappollen.UdonInspector.Editor {
	public static class HexRepresentation {
		public static string Create(byte[] data) {
			using var writer = new StringWriter();
			var       total  = data.Length;
			if (total % 16 != 0)
				total += 16 - (total % 16);

			// write the first line
			writer.Write("--------: ");
			for (var j = 0; j < 8; j++)
				writer.Write($"{j:X2} ");
			writer.Write(" ");
			for (var j = 8; j < 16; j++)
				writer.Write($"{j:X2} ");
			writer.WriteLine(" ----------------");

			writer.WriteLine();

			// write each line
			for (var i = 0; i < total; i += 16) {
				// Address
				writer.Write($"{i:X8}: ");

				// Hex bytes
				for (var j = 0; j < 8; j++)
					writer.Write(i + j < data.Length ? $"{data[i + j]:X2} " : "-- ");

				writer.Write(" ");

				for (var j = 8; j < 16; j++)
					writer.Write(i + j < data.Length ? $"{data[i + j]:X2} " : "-- ");

				writer.Write(" ");

				// ASCII representation
				for (var j = 0; j < 16; j++) 
					if (i + j < data.Length) {
						var c = (char)data[i + j];
						writer.Write(char.IsControl(c) ? '.' : c);
					} else writer.Write('-');
				
				writer.WriteLine();
			}

			return writer.ToString();
		}
	}
}