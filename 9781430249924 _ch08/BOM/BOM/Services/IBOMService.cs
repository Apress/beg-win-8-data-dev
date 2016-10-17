using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOM.BOMWcfServices;
using Microsoft.Practices.Prism.PubSubEvents;
using BOM.Models;
using BOM.ViewModels;

namespace BOM.Services
{
    public interface IBOMService
    {

        Task<ObservableCollection<Component>> GetComponentsAsync();

        Task<ObservableCollection<Part>> GetAllPartsAsync();

        void AddComponentAsync(Component component);

        void AddPartAsync(Part part);

        void AddBOMAsync(BillOfMaterial bom);

        void AddDummyData();
    }

    public class BOMService : IBOMService
    {
         private IEventAggregator _eventAggregator;
         public BOMService(IEventAggregator eventAggregator)
         {
             _eventAggregator = eventAggregator;
         }


        public Task<ObservableCollection<Component>> GetComponentsAsync()
        {
            var client= new BOMServicesClient();
            return  client.GetAllComponentsAsync();           
        }

        public Task<ObservableCollection<Part>> GetAllPartsAsync()
        {
            var client = new BOMServicesClient();
            return client.GetAllPartsAsync();
        }

        public async void AddComponentAsync(Component component)
        {
            var client = new BOMServicesClient();
            await client.AddComponentAsync(component);
            _eventAggregator.GetEvent<ComponentSaveEvent>().Publish(null);
        }

        public async void AddPartAsync(Part part)
        {
            var client = new BOMServicesClient();
            await client.AddPartAsync(part);
            _eventAggregator.GetEvent<PartSaveEvent>().Publish(null);
        }

        public async void AddBOMAsync(BillOfMaterial bom)
        {
            var client = new BOMServicesClient();
            await client.AddBOMAsync(bom);
            _eventAggregator.GetEvent<ComponentSaveEvent>().Publish(null);
        }

        public async void AddDummyData()
        {
            var client = new BOMServicesClient();
            await client.AddDummyDataAsync();
            
        }
    }
}
