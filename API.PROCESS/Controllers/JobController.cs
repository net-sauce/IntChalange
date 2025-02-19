using API.PROCESS.Entities;
using API.PROCESS.Query.Job;
using COMMON.CONTRACTS.Job;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PROCESS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IMediator _mediator;
        private readonly ILogger<JobController> _logger; 

        public JobController(IBus bus, IMediator mediator, ILogger<JobController> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns all jobs for a given client
        /// </summary>
        /// <param name="client_id">Client ID</param>
        /// <returns>List of jobs for the client</returns>
        [HttpGet("all/{client_id}")]
        public async Task<ActionResult<IEnumerable<JobEnity>>> GetAllJobs(int client_id)
        {
            _logger.LogInformation("In GetAllJobs: Fetching all jobs for client ID {ClientID}.", client_id);
            try
            {
                var jobs = (await _mediator.Send(new GetAllJobsQuery(client_id)));
                _logger.LogInformation(
                    "In GetAllJobs: Successfully fetched jobs for client ID {ClientID}. Count: {Count}", client_id,
                    jobs?.Count() ?? 0);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetAllJobs: An error occurred while fetching jobs for client ID {ClientID}.",
                    client_id);
                return StatusCode(500, "An error occurred while retriving jobs.");
            }
        }

        /// <summary>
        /// Returns job status for a given job ID
        /// </summary>
        /// <param name="job_id">Job ID</param>
        /// <returns>Status of the job</returns>
        [HttpGet()]
        public async Task<ActionResult<JobEnity>> GetJobStatus([FromQuery] Guid job_id)
        {
            _logger.LogInformation("In GetJobStatus: Fetching status for job ID {JobID}.", job_id);
            try
            {
                var jobStatus = await _mediator.Send(new GetJobStatusQuery(job_id));
                if (jobStatus == null)
                {
                    _logger.LogWarning("In GetJobStatus: No job found with ID {JobID}.", job_id);
                    return NotFound($"Job with ID {job_id} not found.");
                }

                _logger.LogInformation("In GetJobStatus: Successfully fetched status for job ID {JobID}.", job_id);
                return Ok(jobStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetJobStatus: An error occurred while fetching status for job ID {JobID}.",
                    job_id);
                return StatusCode(500, "An error occurred, culd not retrive job status.");
            }
        }

        /// <summary>
        /// Starts a new job
        /// </summary>
        /// <param name="process">Process entity containing job details</param>
        /// <returns>Newly created job entity</returns>
        [HttpPut()]
        public async Task<ActionResult<NewJobEntity>> StartNewJob([FromBody] ProcessEntity process)
        {
            _logger.LogInformation("In StartNewJob: Starting a new job.");
            try
            {
                NewJobEntity newJob = new NewJobEntity(process);
                await _bus.Publish(new OnCreateNewJob() { JobID = newJob.JobID, ClientID = newJob.ClientID });

                _logger.LogInformation("In StartNewJob: Successfully created and published new job with ID {JobID}.",
                    newJob.JobID);
                return Ok(newJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In StartNewJob: An error occurred while starting a new job.");
                return StatusCode(500, "An error occurred while starting new job.");
            }
        }
    }
}