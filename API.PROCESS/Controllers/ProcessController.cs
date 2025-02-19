using API.PROCESS.Entities;
using API.PROCESS.Query;
using API.PROCESS.Query.Process;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // For logging
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.PROCESS.Command;

namespace API.PROCESS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessController> _logger; // Logger instance

        public ProcessController(IMediator mediator, ILogger<ProcessController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns list of all processes for a given client
        /// </summary>
        /// <param name="client_id">Client ID</param>
        /// <returns>List of processes for the client</returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProcessEntity>>> GetProcessesList([FromQuery] int client_id)
        {
            _logger.LogInformation("In GetProcessesList: Fetching processes list for client ID {ClientID}.", client_id);
            try
            {
                var processes = await _mediator.Send(new GetProcessesForClientQuery(client_id));
                _logger.LogInformation("In GetProcessesList: Successfully fetched processes for client ID {ClientID}. Count: {Count}", client_id, processes?.Count() ?? 0);
                return Ok(processes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetProcessesList: An error occurred while fetching processes for client ID {ClientID}.", client_id);
                return StatusCode(500, "An error occurred while retriving process list.");
            }
        }

        [HttpPut()]
        public async Task<ActionResult> AddProcesses([FromQuery] ProcessEntity process)
        {
            _logger.LogInformation($"In AddProcesses: Adding new process {process.ProcessName} for client {process.ClientID}.");
            try
            {
                var processes = await _mediator.Send(new AddProcessesForClientCommand(process));
                _logger.LogInformation("In AddProcesses: Successfully added into database");
                return Ok(processes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"In AddProcesses: \"An error occurred while inserting processor client ID {process.PorcessID}.");
                return StatusCode(500, "An error occurred while inserting process.");
            }
        }
    }
}