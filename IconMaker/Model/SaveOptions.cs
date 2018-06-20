using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IconMaker.Model
{
    public class SaveOptions
    {
        public SaveOptions()
        { }

        public SaveOptions(SaveOptions other)
        {
            if (other == null)
                return;

            TargetDirectory = other.TargetDirectory;
        }

        public string TargetDirectory { get; set; }
    }
}
