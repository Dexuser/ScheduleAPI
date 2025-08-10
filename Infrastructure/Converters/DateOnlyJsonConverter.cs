using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure;

// So, ASP.NET Doesn't know how to convert a JSON Object to a DateOnly Object. 
// This code does that. You only need to register this class to the builder.Services
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "dd-MM-yyyy"; // El formato esperado del string JSON

    //  Método para leer un DateOnly desde un string en JSON
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString(); // Lee el valor como string, por ejemplo: "2000-05-15"
        return DateOnly.ParseExact(value!, Format); // Lo convierte a DateOnly usando el formato exacto
    }

    //  Método para escribir un DateOnly como string en JSON
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format)); // Convierte el DateOnly a string y lo escribe
    }
}