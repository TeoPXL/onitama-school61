using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Onitama.Api.Util;

public class TwoDimensionalArrayJsonConverter<T> : JsonConverter<T[,]>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsArray && typeToConvert.GetArrayRank() == 2 && typeToConvert.GetElementType() == typeof(T); ;
    }

    public override T[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        List<T[]> rows = new List<T[]>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            T[] row = JsonSerializer.Deserialize<T[]>(ref reader, options);
            rows.Add(row);
        }

        int numberOfRows = rows.Count;

        if (numberOfRows == 0)
        {
            return new T[0, 0];
        }

        int numberOfColumns = rows[0].Length;

        // Validate that all rows have the same number of columns
        foreach (var row in rows)
        {
            if (row.Length != numberOfColumns)
            {
                throw new JsonException("All rows must have the same number of columns.");
            }
        }

        T[,] array = new T[numberOfRows, numberOfColumns];

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                array[i, j] = rows[i][j];
            }
        }

        return array;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        int numberOfRows = value.GetLength(0);
        int numberOfColumns = value.GetLength(1);

        writer.WriteStartArray();
        for (int i = 0; i < numberOfRows; i++)
        {
            writer.WriteStartArray();
            for (int j = 0; j < numberOfColumns; j++)
            {
                string valueAsJson = JsonSerializer.Serialize(value.GetValue(i, j));
                writer.WriteRawValue(valueAsJson);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}