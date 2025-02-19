using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.CONTRACTS.Job
{
    public  class OnJobProgress
    {
        public OnJobProgress(Guid jobID)
        {
            JobID = jobID;
       
        }

        public Guid JobID { get; init; }
        
    }
}
