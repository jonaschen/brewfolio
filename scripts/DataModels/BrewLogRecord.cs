// scripts/DataModels/BrewLogRecord.cs
using Godot;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

// Helper classes for Firestore REST API interaction.
// These define the specific JSON structure Firestore expects.
public class FirestoreDocument
{
    // Example: "projects/YOUR_PROJECT_ID/databases/(default)/documents/users/USER_ID/recipes/RECIPE_ID"
    public string name { get; set; }
    public Fields fields { get; set; }

    // Extracts the document ID from the full 'name' path.
    public string GetId() => name?.Split('/').LastOrDefault();
}

public class Fields
{
    // Each property of our BrewLogRecord is mapped to a Firestore field object.
    public StringValue Id { get; set; }
    public StringValue RecipeName { get; set; }
    public StringValue BeanName { get; set; }
    public StringValue Notes { get; set; }
    public DoubleValue Dose { get; set; }
    public DoubleValue Yield { get; set; }
    public DoubleValue BrewTime { get; set; }
}

// Firestore wrapper for string values.
public class StringValue
{
    public string stringValue { get; set; }
}

// Firestore wrapper for double values.
public class DoubleValue
{
    public double doubleValue { get; set; }
}


public partial class BrewLogRecord
{
    // --- Properties ---
    public string Id { get; set; }

    // Objective Parameters
    public float Dose { get; set; }
    public float Yield { get; set; }
    public float BrewTime { get; set; }

    // Subjective Notes
    public string RecipeName { get; set; }
    public string BeanName { get; set; }
    public string Notes { get; set; }

    /// <summary>
    /// Creates a BrewLogRecord from a Firestore document structure.
    /// </summary>
    public static BrewLogRecord FromFirestoreDocument(FirestoreDocument doc)
    {
        return new BrewLogRecord
        {
            Id = doc.GetId(),
            RecipeName = doc.fields.RecipeName?.stringValue,
            BeanName = doc.fields.BeanName?.stringValue,
            Notes = doc.fields.Notes?.stringValue,
            Dose = (float)(doc.fields.Dose?.doubleValue ?? 0.0),
            Yield = (float)(doc.fields.Yield?.doubleValue ?? 0.0),
            BrewTime = (float)(doc.fields.BrewTime?.doubleValue ?? 0.0),
        };
    }

    /// <summary>
    /// Converts this BrewLogRecord into a format suitable for the Firestore REST API.
    /// </summary>
    public object ToFirestoreDocument()
    {
        var fields = new Fields
        {
            Id = new StringValue { stringValue = this.Id },
            RecipeName = new StringValue { stringValue = this.RecipeName },
            BeanName = new StringValue { stringValue = this.BeanName },
            Notes = new StringValue { stringValue = this.Notes },
            Dose = new DoubleValue { doubleValue = this.Dose },
            Yield = new DoubleValue { doubleValue = this.Yield },
            BrewTime = new DoubleValue { doubleValue = this.BrewTime },
        };
        return new { fields };
    }

    /// <summary>
    /// Converts the object to a Godot-friendly JSON string.
    /// </summary>
    public string ToJson()
    {
        var dict = new Godot.Collections.Dictionary
        {
            { "id", Id },
            { "dose", Dose },
            { "yield", Yield },
            { "brewTime", BrewTime },
            { "recipeName", RecipeName },
            { "beanName", BeanName },
            { "notes", Notes }
        };
        return Json.Stringify(dict, "  ");
    }
}
