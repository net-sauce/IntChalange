using System.Text.Json.Serialization;

/// <summary>
/// Do not look here 
/// </summary>
public class RecordsModel
{
    public List<Record> Records { get; set; }


}

public class Record
{
    [JsonPropertyName("s3")]
    public S3 S3 { get; set; }
    [JsonPropertyName("source")]
    public Source Source { get; set; }

}
public class S3
{
    [JsonPropertyName("bucket")]
    public Bucket Bucket { get; set; }
    [JsonPropertyName("object")]
    public S3Object Object { get; set; }
}

public class Bucket
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("arn")]
    public string Arn { get; set; }
}


public class S3Object
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    [JsonPropertyName("eTag")]
    public string ETag { get; set; }
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }
    [JsonPropertyName("sequencer")]

    public string Sequencer { get; set; }
}


public class Source
{
    [JsonPropertyName("host")]
    public string Host { get; set; }
    [JsonPropertyName("port")]
    public string Port { get; set; }
    [JsonPropertyName("userAgent")]
    public string UserAgent { get; set; }
}