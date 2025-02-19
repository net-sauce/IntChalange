using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.CONTRACTS.Job
{
    public class OnJobCompleted
    {
        public OnJobCompleted(Guid jobID, int client_id)
        {
            JobID = jobID;
            ClientID = client_id;
        }

        public Guid JobID { get; init; }
        public int ClientID { get; init; }
    }
}
