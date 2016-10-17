using BOM.BOMWcfServices;
using BOM.Models;
using BOM.Services;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BOM.ViewModels
{
    public class ComponentViewModel : ViewModel
    {
        private int _componentID;
        private string _ComponentName;
        private ObservableCollection<BillOfMaterial> _bOMComponents;

        public ICommand AddPartsCommand { get; private set; }
        private IEventAggregator _eventAggregator;
        
        public ComponentViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AddPartsCommand = new DelegateCommand(RaiseAddBOM);
        }

        public int ComponentID
        {
            get { return _componentID; }
            set { SetProperty(ref _componentID, value); }
        }

         public string ComponentName
        {
            get { return _ComponentName; }
            set { SetProperty(ref _ComponentName, value); }
        }

         public ObservableCollection<BillOfMaterial>  BOMComponents
        {
            get { return _bOMComponents; }
            set { SetProperty(ref _bOMComponents, value); }
        }
       
        private void RaiseAddBOM()
        {
            _eventAggregator.GetEvent<AddBOMEvent>().Publish(ComponentID);
        }
    }
}
