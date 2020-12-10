using krastsvetmet.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace krastsvetmet.ViewModels
{
    public class MainVM : BindableBase
    {

        #region Public Properties
        public ObservableCollection<ProductInfo> ProductInfos { get; set; } = new ObservableCollection<ProductInfo>();
        public ObservableCollection<MachineClass> Machines { get; set; } = new ObservableCollection<MachineClass>();
        public ObservableCollection<NomenclatureClass> Nomenclatures { get; set; } = new ObservableCollection<NomenclatureClass>();
        public ObservableCollection<BatchClass> Batches { get; set; } = new ObservableCollection<BatchClass>();
        #endregion

        #region Delegate commands
        public DelegateCommand LoadMachinesCommand { get; }
        public DelegateCommand LoadNomenclaturesCommand { get; }
        public DelegateCommand LoadBatchesCommand { get; }
        public DelegateCommand CreateTimetableCommand { get; }
        public DelegateCommand SaveTimetableCommand { get; }
        #endregion

        public MainVM()
        {
            #region Command handlers
            LoadMachinesCommand = new DelegateCommand( () =>
            {
                try
                {
                    Machines.Clear();
                    Nomenclatures.Clear();
                    Batches.Clear();
                    ProductInfos.Clear();

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Таблицы Excel|*.xls;*.xlsx";
                    MessageBox.Show( "Выберете файл с оборудованием" );
                    if ( openFileDialog.ShowDialog() != true )
                    {
                        return;
                    }
                    string machineFilePath = openFileDialog.FileName;

                    MessageBox.Show( "Выберете файл с времем выполнения операций" );
                    if ( openFileDialog.ShowDialog() != true )
                    {
                        return;
                    }
                    string machineParametersFilePath = openFileDialog.FileName;

                    var machines = ExcelHepler.GetMachines( machineFilePath, machineParametersFilePath );
                    Machines.AddRange( machines );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Проверьте входные файлы.\n" + ex.Message );
                }
            } );

            LoadNomenclaturesCommand = new DelegateCommand( () =>
             {
                 try
                 {
                     Nomenclatures.Clear();
                     Batches.Clear();
                     ProductInfos.Clear();

                     OpenFileDialog openFileDialog = new OpenFileDialog();
                     openFileDialog.Filter = "Таблицы Excel|*.xls;*.xlsx";

                     if ( openFileDialog.ShowDialog() != true )
                     {
                         return;
                     }
                     string path = openFileDialog.FileName;

                     var nomenclatures = ExcelHepler.GetNomenclatures( path );
                     Nomenclatures.AddRange( nomenclatures );
                 }
                 catch ( Exception ex )
                 {
                     MessageBox.Show( "Проверьте входные файлы.\n" + ex.Message );
                 }
             } );

            LoadBatchesCommand = new DelegateCommand( () =>
             {
                 try
                 {
                     Batches.Clear();
                     ProductInfos.Clear();

                     OpenFileDialog openFileDialog = new OpenFileDialog();
                     openFileDialog.Filter = "Таблицы Excel|*.xls;*.xlsx";

                     if ( openFileDialog.ShowDialog() != true )
                     {
                         return;
                     }
                     string path = openFileDialog.FileName;

                     var batches = ExcelHepler.GetBatches( path, Nomenclatures );
                     Batches.AddRange( batches );
                 }
                 catch ( Exception ex )
                 {
                     MessageBox.Show( "Проверьте входные файлы.\n" + ex.Message );
                 }
             } );

            CreateTimetableCommand = new DelegateCommand( () =>
             {
                 try
                 {
                     ProductInfos.Clear();
                     ResetMachines();

                     MachineClass.LoadMachines( Machines, Batches );

                     List<ProductInfo> result = new List<ProductInfo>();

                     foreach ( var m in Machines )
                     {
                         result.AddRange( m.GetReport() );
                     }
                     ProductInfos.AddRange( result );
                 }
                 catch ( Exception ex )
                 {
                     MessageBox.Show( "Проверьте входные файлы.\n" + ex.Message );
                 }
             } );

            SaveTimetableCommand = new DelegateCommand( () =>
             {
                 try
                 {
                     SaveFileDialog saveFileDialog = new SaveFileDialog();
                     saveFileDialog.Filter = "Таблицы Excel|*.xlsx";
                     if ( saveFileDialog.ShowDialog() != true )
                     {
                         return;
                     }
                     ExcelHepler.SaveReportToFile( saveFileDialog.FileName, ProductInfos );
                     MessageBox.Show( "Файл с расписание сохранен!" );
                 }
                 catch ( Exception ex )
                 {
                     MessageBox.Show( "Проверьте входные файлы.\n" + ex.Message );
                 }
             } );
            #endregion

        }

        public void ResetMachines()
        {
            foreach ( var m in Machines )
            {
                m.Reset();
            }
        }
    }
}
