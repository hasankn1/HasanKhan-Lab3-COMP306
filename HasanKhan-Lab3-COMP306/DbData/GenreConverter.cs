using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using HasanKhan_Lab3_COMP306.Models;
using System;

public class GenreConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object value)
    {
        if (value == null)
            return new DynamoDBNull();

        // Convert enum to string
        return new Primitive(value.ToString().Replace("_", " ")); // Replaces underscores with spaces
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry == null || entry is DynamoDBNull)
            return null;

        string entryValue = entry.AsString().Replace(" ", ""); // Replace spaces with empty string or underscore, if needed

        // Try parsing with a fallback
        if (Enum.TryParse(typeof(Genre), entryValue, true, out object result))
            return result;
        else
            return Genre.UNKNOWN; // Return a default value if parsing fails
    }
}

