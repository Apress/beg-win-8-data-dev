using BillReminder.Helpers;
using BillReminder.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;

namespace BillReminder.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BillViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;         
        private string _title = string.Empty;

        public int BillID { get; set; }
       
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string _name = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private Category _selectedCategory = null;

        public Category SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }

            set
            {
                _selectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }

        private DateTime _dueDate = System.DateTime.Today;

        public DateTime DueDate
        {
            get
            {
                return _dueDate;
            }

            set
            {
                if (_dueDate == value)
                { return; }
                _dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }

        private Decimal _amount = 0;

        public Decimal Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                { return; }
                _amount = value;
                RaisePropertyChanged("Amount");
            }
        }

        private bool _isrecurring = true;

        public bool Isrecurring
        {
            get
            {
                return _isrecurring;
            }

            set
            {
                _isrecurring = value;
                RaisePropertyChanged("Isrecurring");
            }
        }



        public BillViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Title = "New Bill";
            Messenger.Default.Register<Billtem>(this, message =>
            {
                Bill bill = _dataService.GetBillByID(message.BillID);
                BillID = bill.BillID;
                Name = bill.Name;
                Amount = bill.Amount;
                DueDate = bill.DueDate;
                Isrecurring = bill.IsRecurring;                
                SelectedCategory = _dataService.GetCategoryByID(bill.CategoryID);
                Title = "Edit Bill";

            });
        }

        public IList<Category> Categories
        {
            get
            {
                return _dataService.GetCategories();
            }
        }

        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            if (BillID > 0)
            {
                _dataService.UpdateBill(new Bill()
                {
                    BillID = BillID,
                    Name = _name,
                    Amount = _amount,
                    IsRecurring = _isrecurring,
                    CategoryID = _selectedCategory.CategoryID,
                    DueDate = _dueDate
                });
            }
            else
            {
                _dataService.AddBill(new Bill()
                {
                    Name = _name,
                    Amount = _amount,
                    IsRecurring = _isrecurring,
                    CategoryID = _selectedCategory.CategoryID,
                    DueDate = _dueDate
                });
            }
            _navigationService.Navigate(typeof(MainPage));
        }

        private RelayCommand _backCommand;

        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand
                    ?? (_backCommand = new RelayCommand(ExecuteBackCommand));
            }
        }

        private void ExecuteBackCommand()
        {
            _navigationService.GoBack();

        }
    }
}