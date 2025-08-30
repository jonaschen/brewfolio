using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class BrewLogRecord
{
    public string Id { get; set; }
    public string RecipeName { get; set; }
    public string BeanName { get; set; }
    public string Notes { get; set; }
    public float Dose { get; set; }
    public float Yield { get; set; }
    public float BrewTime { get; set; }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public FirestoreDocument ToFirestoreDocument()
    {
        var doc = new FirestoreDocument();
        doc.fields["recipeName"] = new FirestoreValue { stringValue = this.RecipeName };
        doc.fields["beanName"] = new FirestoreValue { stringValue = this.BeanName };
        doc.fields["notes"] = new FirestoreValue { stringValue = this.Notes };
        doc.fields["dose"] = new FirestoreValue { doubleValue = this.Dose };
        doc.fields["yield"] = new FirestoreValue { doubleValue = this.Yield };
        doc.fields["brewTime"] = new FirestoreValue { doubleValue = this.BrewTime };
        // ID is part of the document path, not the fields
        return doc;
    }

    public static BrewLogRecord FromFirestoreDocument(FirestoreDocument doc)
    {
        if (doc == null || doc.fields == null) return null;

        var record = new BrewLogRecord();
        // The document name format is "projects/{projectId}/databases/(default)/documents/{collectionId}/{documentId}"
        string[] nameParts = doc.name.Split('/');
        record.Id = nameParts[nameParts.Length - 1];

        record.RecipeName = doc.fields.ContainsKey("recipeName") ? doc.fields["recipeName"].stringValue : "";
        record.BeanName = doc.fields.ContainsKey("beanName") ? doc.fields["beanName"].stringValue : "";
        record.Notes = doc.fields.ContainsKey("notes") ? doc.fields["notes"].stringValue : "";
        record.Dose = doc.fields.ContainsKey("dose") ? (float)doc.fields["dose"].doubleValue : 0f;
        record.Yield = doc.fields.ContainsKey("yield") ? (float)doc.fields["yield"].doubleValue : 0f;
        record.BrewTime = doc.fields.ContainsKey("brewTime") ? (float)doc.fields["brewTime"].doubleValue : 0f;

        return record;
    }
}

// Firestore REST API 的輔助結構
public class FirestoreDocument
{
    public string name { get; set; }
    public Dictionary<string, FirestoreValue> fields { get; set; } = new Dictionary<string, FirestoreValue>();
}

public class FirestoreValue
{
    public string stringValue { get; set; }
    public double doubleValue { get; set; }
    // 可以根據需要添加 integerValue, booleanValue 等
}

