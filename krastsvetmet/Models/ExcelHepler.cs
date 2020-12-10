using ClosedXML.Excel;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace krastsvetmet.Models
{
    public static class ExcelHepler
    {
        public static ICollection<NomenclatureClass> GetNomenclatures( string path )
        {
            DataTable table = GetFirstTableFromFile( path );

            bool haveNotEnoughRows = table.Rows.Count < 3;
            bool haveNotEnoughCols = table.Columns.Count != 2;

            if ( haveNotEnoughRows || haveNotEnoughCols )
            {
                throw new ApplicationException( "Входная таблица не соответствует требуемым размерам!" );
            }

            bool wrongHeaders = table.Rows[ 0 ][ 0 ].ToString() != "id" || table.Rows[ 0 ][ 1 ].ToString() != "nomenclature";
            if ( wrongHeaders )
            {
                throw new ApplicationException( "Входная таблица не содержит номенклатур" );
            }

            List<NomenclatureClass> nomenclatures = new List<NomenclatureClass>();
            for ( int i = 1; i < table.Rows.Count; i++ )
            {
                uint id = Convert.ToUInt32( table.Rows[ i ][ 0 ] );
                string name = table.Rows[ i ][ 1 ].ToString();

                NomenclatureClass material = new NomenclatureClass( id, name );
                if ( nomenclatures.Any( n => n.Id == material.Id ) )
                {
                    throw new ApplicationException( $"Найден повторяющийся ID={material.Id} в номенклатурах. Загрузка не возможна!" );
                }

                nomenclatures.Add( material );
            }

            return nomenclatures;
        }

        public static ICollection<MachineClass> GetMachines( string machineTablePath, string parametersTablePath )
        {
            DataTable table = GetFirstTableFromFile( machineTablePath );

            bool haveNotEnoughRows = table.Rows.Count < 3;
            bool haveNotEnoughCols = table.Columns.Count != 2;

            if ( haveNotEnoughRows || haveNotEnoughCols )
            {
                throw new ApplicationException( "Входная таблица не соответствует требуемым размерам!" );
            }

            bool wrongHeaders = table.Rows[ 0 ][ 0 ].ToString() != "id" || table.Rows[ 0 ][ 1 ].ToString() != "name";
            if ( wrongHeaders )
            {
                throw new ApplicationException( "Входная таблица не содержит оборудование" );
            }

            Dictionary<uint, List<MachineParameterClass>> machineParameters = GetMachineParameters( parametersTablePath );

            List<MachineClass> machines = new List<MachineClass>();
            for ( int i = 1; i < table.Rows.Count; i++ )
            {
                uint id = Convert.ToUInt32( table.Rows[ i ][ 0 ] );
                string name = table.Rows[ i ][ 1 ].ToString();

                if ( machineParameters.ContainsKey( id ) == false )
                {
                    throw new ApplicationException( "Machine must have at least one parameter" );
                }

                MachineClass machine = new MachineClass( id, name, machineParameters[ id ] );
                if ( machines.Any( m => m.Id == machine.Id ) )
                {
                    throw new ApplicationException( $"Найден повторяющийся ID={machine.Id} в оборудовании. Загрузка не возможна!" );
                }
                machines.Add( machine );
            }

            return machines;
        }

        public static ICollection<BatchClass> GetBatches( string path, ICollection<NomenclatureClass> nomenclatures )
        {
            DataTable table = GetFirstTableFromFile( path );


            bool haveNotEnoughRows = table.Rows.Count < 3;
            bool haveNotEnoughCols = table.Columns.Count != 2;

            if ( haveNotEnoughRows || haveNotEnoughCols )
            {
                throw new ApplicationException( "Входная таблица не соответствует требуемым размерам!" );
            }

            bool wrongHeaders = table.Rows[ 0 ][ 0 ].ToString() != "id" || table.Rows[ 0 ][ 1 ].ToString() != "nomenclature id";
            if ( wrongHeaders )
            {
                throw new ApplicationException( "Входная таблица не содержит оборудование" );
            }


            List<BatchClass> batches = new List<BatchClass>();
            for ( int i = 1; i < table.Rows.Count; i++ )
            {
                uint batchId = Convert.ToUInt32( table.Rows[ i ][ 0 ] );
                uint nomenclatureId = Convert.ToUInt32( table.Rows[ i ][ 1 ] );

                if ( !nomenclatures.Any( n => n.Id == nomenclatureId ) )
                {
                    throw new ApplicationException( "NomenclatureId не найдена в загруженных номенклатурах!" );
                }

                NomenclatureClass nomenclature = nomenclatures.First( n => n.Id == nomenclatureId );

                BatchClass batch = new BatchClass( batchId, nomenclature );

                if ( batches.Any( b => b.Id == batch.Id ) )
                {
                    throw new ApplicationException( $"Найден повторяющийся ID={batch.Id} в партиях. Загрузка не возможна!" );
                }

                batches.Add( batch );
            }

            return batches;
        }
        public static void SaveReportToFile( string path, ICollection<ProductInfo> report )
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add( "Расписание" );

            worksheet.Cell( "A1" ).Value = "ID партии";
            worksheet.Cell( "B1" ).Value = "ID оборудования";
            worksheet.Cell( "C1" ).Value = "Время начала";
            worksheet.Cell( "D1" ).Value = "Время окончания";

            int i = 0;
            foreach ( var productInfo in report )
            {
                worksheet.Cell( 2 + i, 1 ).Value = productInfo.Batch.Id;
                worksheet.Cell( 2 + i, 2 ).Value = productInfo.Machine.Id;
                worksheet.Cell( 2 + i, 3 ).Value = productInfo.StartTime;
                worksheet.Cell( 2 + i, 4 ).Value = productInfo.EndTime;
                i++;
            }
            workbook.SaveAs( path );
        }

        private static Dictionary<uint, List<MachineParameterClass>> GetMachineParameters( string path )
        {
            DataTable table = GetFirstTableFromFile( path );

            bool haveNotEnoughRows = table.Rows.Count < 3;
            bool haveNotEnoughCols = table.Columns.Count != 3;

            if ( haveNotEnoughRows || haveNotEnoughCols )
            {
                throw new ApplicationException( "Входная таблица не соответствует требуемым размерам!" );
            }

            bool wrongHeaders = table.Rows[ 0 ][ 0 ].ToString() != "machine tool id" || table.Rows[ 0 ][ 1 ].ToString() != "nomenclature id" || table.Rows[ 0 ][ 2 ].ToString() != "operation time";
            if ( wrongHeaders )
            {
                throw new ApplicationException( "Входная таблица не содержит параметры оборудования" );
            }

            Dictionary<uint, List<MachineParameterClass>> machineParameters = new Dictionary<uint, List<MachineParameterClass>>();

            for ( int i = 1; i < table.Rows.Count; i++ )
            {
                uint machineId = Convert.ToUInt32( table.Rows[ i ][ 0 ] );

                uint nomenclatureId = Convert.ToUInt32( table.Rows[ i ][ 1 ] );
                uint operationTime = Convert.ToUInt32( table.Rows[ i ][ 2 ] );

                MachineParameterClass param = new MachineParameterClass( nomenclatureId, operationTime );

                if ( machineParameters.ContainsKey( machineId ) == false )
                {
                    machineParameters.Add( machineId, new List<MachineParameterClass>() );
                }

                machineParameters[ machineId ].Add( param );
            }

            return machineParameters;
        }

        private static DataTable GetFirstTableFromFile( string path )
        {
            using FileStream stream = File.Open( path, FileMode.Open, FileAccess.Read );
            using IExcelDataReader reader = ExcelReaderFactory.CreateReader( stream );

            DataSet dataset = reader.AsDataSet();

            if ( dataset.Tables.Count == 0 )
            {
                throw new FileLoadException();
            }
            return dataset.Tables[ 0 ];
        }
    }
}
