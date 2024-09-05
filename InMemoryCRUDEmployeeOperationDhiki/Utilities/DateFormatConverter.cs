using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace InMemoryCRUDEmployeeOperationDhiki.Utilities;

//Formater DateTime menjadi "dd-MMM-yyyy"
public class DateFormatConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd-MMM-yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        return DateTime.ParseExact(dateString, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}