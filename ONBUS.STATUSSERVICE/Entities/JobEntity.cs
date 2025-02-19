using System.Collections.Generic;
using System;
using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


public enum JobStatus
{
    Pending,
    InProgress,
    Completed
}
[BsonNoId]
[BsonIgnoreExtraElements]
public class JobEntity : SagaStateMachineInstance, ISagaVersion
{
    [BsonRepresentation(BsonType.String)]
    public Guid JobID { get; set; }
    public int ClientID { get; set; }
    public List<string> FilesRequired { get; set; }
    public List<string> FilesUploaded { get; set; }
    public int Status { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid CorrelationId
    {
        get => JobID;
        set
        {
            JobID = value;
        }
    }

    public int Version { get; set; }
}


