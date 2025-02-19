using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.CONTRACTS.Job
{
  public  class OnCreateNewJob
    {
        public Guid JobID { get; init; }
        public int ClientID { get; init; }

        public List<string>? FilesRequired { get; set; }
    }
}
