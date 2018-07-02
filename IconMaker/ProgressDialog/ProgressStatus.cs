using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconMaker.ProgressDialog
{
    public class ProgressStatus
    {
        public string Message { get; set; }
        public string SubLabel { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Value { get; set; }
    }
}
