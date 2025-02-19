using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.CONTRACTS.File
{
   public class OnFileUploaded
    {
        public Guid JobID { get; set; }
        public required string FileName { get; set; }
        public string ClientID { get; set; }
    }
}
