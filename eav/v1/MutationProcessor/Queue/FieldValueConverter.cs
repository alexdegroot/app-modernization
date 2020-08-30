using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MutationProcessor.Queue
{
    public class FieldValueConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            // When there's a value, this value is not empty and doesn't start with a zero, try to make it a number
            if (!string.IsNullOrEmpty(value) && IsNumeric(value) && value[0] != '0')
            {
                return long.Parse(value!);
            }
            
            // TODO: Consider booleans and other field types

            return string.IsNullOrEmpty(value) ? null : value.Trim();
        }

        public override void Write(
            Utf8JsonWriter writer,
            object dateTimeValue,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private static bool IsNumeric(string value) => value.All(char.IsNumber);
    }
}