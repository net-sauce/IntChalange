namespace API.PROCESS.Entities
{
    public record NewJobEntity : JobEnity
    {
        public NewJobEntity(ProcessEntity process)
        {
            this.JobID = Guid.NewGuid();
            this.Status = JobStatus.Pending;
            this.FilesRequired = process.FilesRequired;
            this.FillesUploaded = null;
            this.ClientID = ClientID;

        }

        protected NewJobEntity(JobEnity original) : base(original)
        {
        }
    }
}
