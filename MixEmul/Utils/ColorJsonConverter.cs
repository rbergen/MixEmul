using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MixGui.Utils
{
	class ColorJsonConverter : JsonConverter<Color>
	{
		public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> Color.FromArgb(reader.GetInt32());
		public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
			=> writer.WriteNumberValue(value.ToArgb());
	}
}
