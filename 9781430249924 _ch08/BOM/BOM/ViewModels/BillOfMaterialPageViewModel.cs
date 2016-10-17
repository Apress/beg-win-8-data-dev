using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BOM.BOMWcfServices;
using BOM.Services;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using BOM.Models;

namespace BOM.ViewModels
{
    public class BillOfMaterialPageViewModel : ViewModel
    {
        private readonly INavigationService _navigationService; 
        private readonly IBOMService _bomservice;

        private string _headerLabel;

        private bool _isAddComponentPopupOpened;
        private bool _isAddPartPopupOpened;
        private bool _isShowBOMPopupOpened;

        private int _selectedComponentID;
        private Part _selectedPart;
        private string _componentName;
        private string _partName;
        private int _stockCount = 0;
        private int _bomQuantity = 0;


        private ObservableCollection<ComponentViewModel> _components;
        private ReadOnlyCollection<Part> _parts;

        public ICommand OpenComponentCommand { get; private set; }
        public ICommand OpenPartCommand { get; private set; }
        public ICommand AddComponentCommand { get; private set; }
        public ICommand AddPartCommand { get; private set; }
        public ICommand AddBOMCommand { get; private set; }
        private IEventAggregator _eventAggregator;
        public BillOfMaterialPageViewModel(INavigationService navigationService,IEventAggregator eventAggregator, IBOMService bomservice)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;           
            _bomservice = bomservice;
            OpenComponentCommand = new DelegateCommand(OpenComponentFlyout);
            OpenPartCommand = new DelegateCommand(OpenPartFlyout);
            AddComponentCommand = new DelegateCommand(AddComponentAsync);
            AddPartCommand = new DelegateCommand(AddPartAsync);
            AddBOMCommand = new DelegateCommand(AddBOMAsync);
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<ComponentSaveEvent>().Subscribe(RefreshComponentListAsync);
                eventAggregator.GetEvent<PartSaveEvent>().Subscribe(RefreshPartListAsync);
                eventAggregator.GetEvent<AddBOMEvent>().Subscribe(OpenBOMFlyout);
            }
        }

        private void OpenComponentFlyout()
        {
            ComponentName = string.Empty;
            IsAddComponentPopupOpened = true;
        }

        private void OpenPartFlyout()
        {
            PartName = string.Empty;
            StockCount = 0;
            IsAddPartPopupOpened = true;
        }

        public async void OpenBOMFlyout(object componentID)
        {
            _selectedComponentID=(int) componentID;
            IsShowBOMPopupOpened = true;
        } 

        public bool IsAddComponentPopupOpened
        {
            get { return _isAddComponentPopupOpened; }
            set { SetProperty(ref _isAddComponentPopupOpened, value); }
        }

        public bool IsAddPartPopupOpened
        {
            get { return _isAddPartPopupOpened; }
            set { SetProperty(ref _isAddPartPopupOpened, value); }
        }

        public bool IsShowBOMPopupOpened
        {
            get { return _isShowBOMPopupOpened; }
            set { SetProperty(ref _isShowBOMPopupOpened, value); }
        }
        
        public string HeaderLabel
        {
            get { return _headerLabel; }
            private set { SetProperty(ref _headerLabel, value); }
        }

        public string ComponentName
        {
            get { return _componentName; }
            set { SetProperty(ref _componentName, value); }
        }

        public string PartName
        {
            get { return _partName; }
            set { SetProperty(ref _partName, value); }
        }

        public int StockCount
        {
            get { return _stockCount; }
            set { SetProperty(ref _stockCount, value); }
        }

        public int BOMQuantity
        {
            get { return _bomQuantity; }
            set { SetProperty(ref _bomQuantity, value); }
        }

        public int SelectedComponentID
        {
            get { return _selectedComponentID; }
            set { SetProperty(ref _selectedComponentID, value); }
        }

        public Part SelectedPart
        {
            get { return _selectedPart; }
            set { SetProperty(ref _selectedPart, value); }
        }

        public ObservableCollection<ComponentViewModel> Components
        {
            get { return _components; }
            private set { SetProperty(ref _components, value); }
        }

        public ReadOnlyCollection<Part> Parts
        {
            get { return _parts; }
            private set { SetProperty(ref _parts, value); }
        }

        public override async void OnNavigatedTo(object navigationParameter,
                                                 Windows.UI.Xaml.Navigation.NavigationMode navigationMode,
                                                 System.Collections.Generic.Dictionary<string, object> viewModelState)
        {           

            HeaderLabel = "My BOM";
            //TO add dummy data;
            DummyData();
            GetComponentsAsync();
            GetPartsAsync();
           
        }

        private async void GetComponentsAsync()
        {
            var components = await _bomservice.GetComponentsAsync();
             
            var vmComponents = new ObservableCollection<ComponentViewModel>();
            foreach (Component item in  new ObservableCollection<Component>(components.ToList()))
            {
                ComponentViewModel cvm = new ComponentViewModel(_eventAggregator);
                cvm.ComponentID = item.ComponentID;
                cvm.ComponentName = item.ComponentName;
                cvm.BOMComponents = item.BOMComponents;
                vmComponents.Add(cvm);

            }
            Components = vmComponents;  
        }

        private async void DummyData()
        {
            var components = await _bomservice.GetComponentsAsync();
            if (components == null || (!components.Any()))
            {
                _bomservice.AddDummyData();              
            }
        }

        private async void GetPartsAsync()
        {
            var items = await _bomservice.GetAllPartsAsync();
            Parts = new ReadOnlyCollection<Part>(items.ToList());
        }

        private async void AddComponentAsync()
        {
            _bomservice.AddComponentAsync(new Component { ComponentName = _componentName });            
            IsAddComponentPopupOpened = false;
        }

        private async void AddPartAsync()
        {
            _bomservice.AddPartAsync(new Part { PartName = _partName, StockCount = _stockCount });
            IsAddPartPopupOpened = false;
        }

        private async void AddBOMAsync()
        {
            _bomservice.AddBOMAsync(new BillOfMaterial { ComponentID = _selectedComponentID, PartID = _selectedPart.PartID, Quantity = _bomQuantity });
            IsShowBOMPopupOpened = false;
        }      

        public async void RefreshComponentListAsync(object notUsed)
        {
            GetComponentsAsync();
        }
     
        public async void RefreshPartListAsync(object notUsed)
        {
            GetPartsAsync();
        }

    }
}
