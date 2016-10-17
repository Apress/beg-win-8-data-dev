using GalaSoft.MvvmLight;
using BillReminder.Model;
using BillReminder.Helpers;
using GalaSoft.MvvmLight.Command;
 
using System;
using System.Collections.ObjectModel;
 
namespace BillReminder.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
       private readonly INavigationService _navigationService;     
         
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
                RaisePropertyChanged("SelectedBill");
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
             _navigationService.Navigate<BillViewModel>();
        }

        private void ExecuteEditCommand()
        {
            _navigationService.Navigate<BillViewModel>(SelectedBill);
        }
         
    }
}