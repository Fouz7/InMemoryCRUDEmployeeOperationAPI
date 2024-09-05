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
        if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        throw new JsonException("Invalid date format. Please use 'dd-MMM-yyyy'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}