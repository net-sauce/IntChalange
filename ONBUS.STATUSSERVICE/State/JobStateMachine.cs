using MassTransit;

using System.Collections.Generic;
using System;
using COMMON.CONTRACTS.Job;
using COMMON.CONTRACTS.File;
using MassTransit.Contracts.JobService;


public class JonStateMachine : MassTransitStateMachine<JobEntity>
{
    public State Pending { get; private set; }
    public State InProgress { get; private set; }
    public State Completed { get; private set; }
    public Event<OnCreateNewJob> JobStarted { get; private set; }
    public Event<OnFileUploaded> FileUploaded { get; private set; }

    public JonStateMachine()
    {
        Event(() => FileUploaded, x => x.CorrelateById(context => context.Message.JobID));
        Event(() => JobStarted, x => x.CorrelateById(context => context.Message.JobID));

        InstanceState(x => x.Status, Pending,InProgress,Completed);


        Initially(
            When(JobStarted)
                .Then(x =>
                {
                    Console.WriteLine($"Processing new job with id {x.Saga.JobID}");
                    x.Saga.FilesRequired = x.Message.FilesRequired;
                    x.Saga.FilesUploaded = new List<string>();
                    x.Saga.ClientID = x.Message.ClientID;
                    x.Send<OnJobCreated>(new OnJobCreated(x.Saga.JobID));
                })
                .TransitionTo(InProgress));


        During(InProgress,
            When(FileUploaded)
                .Then(x =>
                { 
                    Console.WriteLine($"{x.Message.FileName} uploaded for job {x.Saga.JobID}");

                   var uploadedFile = x.Message.FileName;
                    if (!x.Saga.FilesUploaded.Contains(uploadedFile))
                    {
                        x.Saga.FilesUploaded.Add(uploadedFile);                      
                    }

                })
                .IfElse(
                    x =>
                        (x.Saga.FilesUploaded?.Count ?? 0) == x.Saga.FilesRequired?.Count,
                    binder => binder
                    .TransitionTo(Completed)
                        .Publish(context => new OnJobCompleted(context.Saga.JobID)
                       ).Then(x =>
                       {
                           Console.WriteLine($"Job {x.Saga.JobID} completed");
                       }),
                    binder => binder // Still uploading
                        .Publish(context => new OnFileProgressUpdated
                        {
                            JobID = context.Saga.JobID,
                            UploadedFilesCount = context.Saga.FilesUploaded?.Count ?? 0,
                            TotalFilesCount = context.Saga.FilesRequired?.Count ?? 0
                        }).Then(x =>
                        {
                            var uploadedCount = x.Saga.FilesUploaded?.Count ?? 0;
                            var totalCount = x.Saga.FilesRequired?.Count ?? 1;
                            Console.WriteLine($"Job {(float)uploadedCount/(float)totalCount*100} completed");
                        })));

 

        During(Completed, Ignore(FileUploaded));
    }
}