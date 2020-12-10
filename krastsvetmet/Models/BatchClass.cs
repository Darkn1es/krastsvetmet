using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public class BatchClass
    {
        public uint Id { get; }
        public NomenclatureClass Nomenclature { get; }

        public BatchClass( uint id, NomenclatureClass nomenclature )
        {
            Id = id;
            Nomenclature = ( NomenclatureClass )nomenclature.Clone();
        }

        public override string ToString()
        {
            return $"{{\nID : {Id},\nNomenclature: {Nomenclature} }}";
        }
    }
}
