using System;

public class OnFileProgressUpdated
{
    public Guid JobID { get; set; } 
    public int UploadedFilesCount { get; set; }
    public int TotalFilesCount { get; set; } 
}