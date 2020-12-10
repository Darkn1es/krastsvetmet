using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public class MachineParameterClass
    {
        public uint OperationTime { get; }
        public uint NomenclatureId { get; }

        public MachineParameterClass( uint nomenclatureId, uint operationTime )
        {
            NomenclatureId = nomenclatureId;
            OperationTime = operationTime;
        }

        public override string ToString()
        {
            return $"{{\nOperationTime : {OperationTime},\nNomenclatureId: {NomenclatureId} }}";
        }
    }
}
