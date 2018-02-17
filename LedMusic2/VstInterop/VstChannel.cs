using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace LedMusic2.VstInterop
{

    public class VstChannel : VMBase
    {

        private string _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
            }
        }

        public Guid Id { get; set; }

        public VstChannel(Guid id)
        {
            Id = id;
        }

        public void ExecuteMessage(JObject message)
        {



        }

    }

}
