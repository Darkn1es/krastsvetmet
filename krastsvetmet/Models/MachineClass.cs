using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public class MachineClass
    {
        public uint Id { get; }
        public string Name { get; set; }
        public List<MachineParameterClass> Parameters { get; }
        public uint EndWorkingTime { get; private set; } = 0;

        private List<ProductInfo> _conveyor = new List<ProductInfo>();

        public MachineClass( uint id, string name, ICollection<MachineParameterClass> parameters )
        {
            if ( string.IsNullOrEmpty( name ) )
            {
                throw new ArgumentException( "Имя оборудования не может быть пустым" );
            }
            if ( parameters == null || parameters.Count == 0 )
            {
                throw new ArgumentException( "Параметры не могут быть пустые" );

            }
            Id = id;
            Name = name;
            Parameters = new List<MachineParameterClass>( parameters );
        }

        public void Process( BatchClass batch )
        {
            NomenclatureClass nomenclature = batch.Nomenclature;
            if ( !Parameters.Any( param => param.NomenclatureId == nomenclature.Id ) )
            {
                throw new ApplicationException( "Оборудование не может обработать данную номеклатуру" );
            }

            MachineParameterClass parameter = Parameters.First( param => param.NomenclatureId == nomenclature.Id );

            uint startTime = EndWorkingTime;
            uint endTime = startTime + parameter.OperationTime;

            EndWorkingTime = endTime;

            ProductInfo productInfo = new ProductInfo( batch, this, startTime, endTime );

            _conveyor.Add( productInfo );
        }

        public List<ProductInfo> GetReport()
        {
            return _conveyor.ToList();
        }

        public void Reset()
        {
            _conveyor.Clear();
            EndWorkingTime = 0;
        }
        public bool CanProcess( uint nomenclatureId )
        {
            return Parameters.Any( p => p.NomenclatureId == nomenclatureId );
        }

        public static void LoadMachines( ICollection<MachineClass> machines, ICollection<BatchClass> batches )
        {
            foreach ( var batch in batches )
            {
                var availableMachines = from m in machines
                                        where m.CanProcess( batch.Nomenclature.Id )
                                        select m;

                if ( availableMachines.Count() == 0 )
                {
                    throw new ApplicationException( $"Не получилось обработать партию {batch}" );
                }

                // choose less loaded machine
                var freeMachine = availableMachines.First();
                foreach ( var currentMachine in availableMachines )
                {
                    if ( currentMachine.EndWorkingTime < freeMachine.EndWorkingTime )
                    {
                        freeMachine = currentMachine;
                    }
                }

                freeMachine.Process( batch );
            }
        }

        public override string ToString()
        {
            return $"{{\nID : {Id},\nName: {Name} }}";
        }

    }
}
