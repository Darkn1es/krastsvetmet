using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public class ProductInfo
    {
        public BatchClass Batch { get; }
        public MachineClass Machine { get; }
        public uint StartTime { get; }
        public uint EndTime { get; }

        public ProductInfo( BatchClass batch, MachineClass machine, uint startTime, uint endTime )
        {
            Batch = batch;
            Machine = machine;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
