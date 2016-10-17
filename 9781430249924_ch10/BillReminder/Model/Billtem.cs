using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BillReminder.Model
{
    public class Billtem : ObservableObject
    {
        private readonly IDataService _dataService;

        public Billtem()
        {
        }

        public Billtem(IDataService dataService)
        {
            _dataService = dataService;
        }

        public int BillID { get; set; }

        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }

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

        private string _category = string.Empty;

        public string Category
        {
            get { return _category; }

            set
            {
                if (_category == value)
                {
                    return;
                }
                _category = value;
                RaisePropertyChanged("Category");
            }
        }

        private DateTime? _dueDate = DateTime.Today;

        public DateTime? DueDate
        {
            get { return _dueDate; }

            set
            {
                if (_dueDate == value)
                {
                    return;
                }
                _dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }

        private DateTime? _paidDate = DateTime.Today;

        public DateTime? PaidDate
        {
            get { return _paidDate; }

            set
            {
                if (_paidDate == value)
                {
                    return;
                }
                _paidDate = value;
                RaisePropertyChanged("PaidDate");
            }
        }

        private Decimal _amount;

        public Decimal Amount
        {
            get { return _amount; }

            set
            {
                if (_amount == value)
                {
                    return;
                }
                _amount = value;
                RaisePropertyChanged("Amount");
            }
        }

        private Decimal? _paidAmount = 0;

        public Decimal? PaidAmount
        {
            get { return _paidAmount; }

            set
            {
                if (_paidAmount == value)
                {
                    return;
                }
                _paidAmount = value;
                RaisePropertyChanged("PaidAmount");
            }
        }


        private bool _isPaid;

        public bool IsPaid
        {
            get { return _isPaid; }

            set
            {
                if (_isPaid == value)
                {
                    return;
                }
                _isPaid = value;
                RaisePropertyChanged("IsPaid");
            }
        }

        private RelayCommand _payCommand;

        public RelayCommand PayCommand
        {
            get
            {
                return _payCommand
                       ?? (_payCommand = new RelayCommand(ExecutePayCommand));
            }
        }

        private void ExecutePayCommand()
        {
            PaidAmount = Amount;
            _dataService.MarkPaid(BillID, (decimal) PaidAmount);
            IsPaid = true;
        }
    }
}
