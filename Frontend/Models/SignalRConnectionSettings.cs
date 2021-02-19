using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class SignalRConnectionSettings : ISignalRConnectionSettings
    {
        public string ConnectionString { get; set; }
    }

    public interface ISignalRConnectionSettings
    {
        string ConnectionString { get; set; }
    }
}
