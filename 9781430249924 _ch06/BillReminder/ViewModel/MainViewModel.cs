using GalaSoft.MvvmLight;
using BillReminder.Model;
using BillReminder.Helpers;
using GalaSoft.MvvmLight.Command;
using BillReminder.Views;
using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace BillReminder.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;     
        private string _welcomeTitle = string.Empty;     

         
        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;            
        }

        public ObservableCollection<Billtem> Bills
        {
            get
            {
               return  _dataService.GetBills(DateTime.Now);
            }
        }

        private Billtem _selectedBill = null;

        public Billtem SelectedBill
        {
            get
            {
                return _selectedBill;
            }

            set
            {
                _selectedBill = value;
                ShowAppBar = true;
                RaisePropertyChanged("SelectedBill");
            }
        }

        private bool _showAppBar;

        public bool ShowAppBar
        {
            get
            {
                return _showAppBar;
            }

            set
            {               
                _showAppBar = value;
                RaisePropertyChanged("ShowAppBar");
            }
        }



        private RelayCommand _addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(ExecuteAddCommand));
            }
        }

        private RelayCommand _editCommand;

        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand(ExecuteEditCommand));
            }
        }

        private void ExecuteAddCommand()
        {
           _navigationService.Navigate(typeof(BillView));
           
        }

        private void ExecuteEditCommand()
        {   
            _navigationService.Navigate(typeof(BillView));
            Messenger.Default.Send<Billtem>(SelectedBill);
        }
         
    }
}