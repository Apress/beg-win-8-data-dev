using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using BOM.Service.Models;

namespace BOM.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IBOMServices
    {

        [OperationContract]
        IList<Part> GetAllParts();

        [OperationContract]
        IList<Component> GetAllComponents();

        [OperationContract]
        void AddComponent(Component component);

        [OperationContract]
        void DeleteComponent(Component component);

        [OperationContract]
        void AddPart(Part part);

        [OperationContract]
        void DeletePart(Part part);

        [OperationContract]
        void AddBOM(BillOfMaterial bom);

        [OperationContract]
        void RemoveBOM(BillOfMaterial bom);

        [OperationContract]
        void AddDummyData(); 
    }    
}
