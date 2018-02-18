using System;
using Newtonsoft.Json.Linq;

namespace LedMusic2.VstInterop
{

    public class VstChannel
    {

        public Guid Id { get; }
        public string Name { get; private set; }
        public double Value { get; private set; }
        public int Note { get; private set; }

        public VstChannel(Guid id)
        {
            Id = id;
        }

        public void ExecuteMessage(JObject message)
        {

            try
            {

                string type = message.Value<string>("type");
                
                if (type == "name")
                {
                    Name = message.Value<string>("name");
                } else if (type == "value")
                {
                    Value = message.Value<double>("value");
                } else if (type == "midi" && message.Value<string>("event") == "noteOn")
                {
                    Value = message.Value<double>("velocity");
                    Note = message.Value<int>("note");
                }

            } catch (Exception) { }

        }

    }

}
