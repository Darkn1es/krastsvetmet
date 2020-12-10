using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public class NomenclatureClass : ICloneable
    {
        public uint Id { get; }
        public string Name { get; }
        public NomenclatureClass( uint id, string name )
        {
            if ( string.IsNullOrEmpty( name ) )
            {
                throw new ArgumentException( "Имя номенклатуры не может быть пустым" );
            }
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{{\nId : {Id},\nName: {Name} }}";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
