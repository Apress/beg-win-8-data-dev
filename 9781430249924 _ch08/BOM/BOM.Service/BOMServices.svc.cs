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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BOMServices : IBOMServices
    {
        public void AddDummyData()
        {
            using (var ctx = new BOMDataContext())
            {                
                Part part1 = new Part();
                part1.PartName = "Encoder 11MM ROTARY SW TOP ADJ";
                part1.StockCount = 100;
                ctx.Parts.Add(part1);

                Part part2 = new Part();
                part2.PartName = "Diode ZENER 3V 200MW SOD-523F";
                part2.StockCount = 70;
                ctx.Parts.Add(part2);

                Part part3 = new Part();
                part3.PartName = "Analog multiplexer";
                part3.StockCount = 170;
                ctx.Parts.Add(part3);

                Part part4 = new Part();
                part4.PartName = "USB Connector";
                part4.StockCount = 525;
                ctx.Parts.Add(part4);

                Part part5 = new Part();
                part5.PartName = "CRYSTAL 26.000000 MHZ 8PF SMD";
                part5.StockCount = 1100;
                ctx.Parts.Add(part5);

                Part part6 = new Part();
                part6.PartName = "Spartan-3A FPGA";
                part6.StockCount = 11;
                ctx.Parts.Add(part6);

                Component component1 = new Component();
                component1.ComponentName = "Heater Core";
                ctx.Components.Add(component1);

                Component component2 = new Component();
                component2.ComponentName = "Car engine Thermostat";
                ctx.Components.Add(component2);

                ctx.SaveChanges();

                BillOfMaterial bom1 = new BillOfMaterial();
                bom1.PartID = 1;
                bom1.ComponentID = 1;
                bom1.Quantity = 1;
                ctx.BillOfMaterials.Add(bom1);

                BillOfMaterial bom2 = new BillOfMaterial();
                bom2.PartID = 2;
                bom2.ComponentID = 1;
                bom2.Quantity = 1;
                ctx.BillOfMaterials.Add(bom2);

                BillOfMaterial bom3 = new BillOfMaterial();
                bom3.PartID = 3;
                bom3.ComponentID = 1;
                bom3.Quantity = 1;
                ctx.BillOfMaterials.Add(bom3);

                BillOfMaterial bom4 = new BillOfMaterial();
                bom4.PartID = 4;
                bom4.ComponentID = 1;
                bom4.Quantity = 1;
                ctx.BillOfMaterials.Add(bom4);

                BillOfMaterial bom5 = new BillOfMaterial();
                bom5.PartID = 1;
                bom5.ComponentID = 2;
                bom5.Quantity = 1;
                ctx.BillOfMaterials.Add(bom5);

                BillOfMaterial bom6 = new BillOfMaterial();
                bom6.PartID = 5;
                bom6.ComponentID = 2;
                bom6.Quantity = 1;
                ctx.BillOfMaterials.Add(bom6);

                BillOfMaterial bom7 = new BillOfMaterial();
                bom7.PartID = 3;
                bom7.ComponentID = 2;
                bom7.Quantity = 1;
                ctx.BillOfMaterials.Add(bom7);

                BillOfMaterial bom8 = new BillOfMaterial();
                bom8.PartID = 6;
                bom8.ComponentID = 2;
                bom8.Quantity = 1;
                ctx.BillOfMaterials.Add(bom8);

                BillOfMaterial bom9 = new BillOfMaterial();
                bom9.PartID = 4;
                bom9.ComponentID = 2;
                bom9.Quantity = 1;
                ctx.BillOfMaterials.Add(bom9);

                ctx.SaveChanges();
            }
        }

        public IList<Part> GetAllParts()
        {
            using (var ctx = new BOMDataContext())
            {
                ctx.Configuration.ProxyCreationEnabled = false;
                var parts = ctx.Parts
                    .Include("BOMParts")
                    .ToList();
                ctx.Configuration.ProxyCreationEnabled = true;
                return parts;
            }
        }


        public IList<Component> GetAllComponents()
        {
            using (var ctx = new BOMDataContext())
            {
                ctx.Configuration.ProxyCreationEnabled = false;

                var components =  ctx.Components
                    .Include("BOMComponents")
                    .Include("BOMComponents.BOMPart")
                    .ToList();
                ctx.Configuration.ProxyCreationEnabled = true;
                return components;
            }            
        }

        public void AddComponent(Component component)
        {
            using (var ctx = new BOMDataContext())
            {
                ctx.Components.Add(component);
                ctx.SaveChanges();
            }
        }


        public void DeleteComponent(Component component)
        {
            using (var ctx = new BOMDataContext())
            {

                foreach (var bom in component.BOMComponents)
                {
                    ctx.BillOfMaterials.Remove(bom);
                }
                ctx.Components.Remove(component);
                ctx.SaveChanges();
            }
        }

        public void AddPart(Part part)
        {
            using (var ctx = new BOMDataContext())
            {
                ctx.Parts.Add(part);
                ctx.SaveChanges();
            }
        }


        public void DeletePart(Part part)
        {
            using (var ctx = new BOMDataContext())
            {

                foreach (var bom in part.BOMParts)
                {
                    ctx.BillOfMaterials.Remove(bom);
                }
                ctx.Parts.Remove(part);
                ctx.SaveChanges();
            }
        }

        public void AddBOM(BillOfMaterial bom)
        {
            using (var ctx = new BOMDataContext())
            {
                var bomv = ctx.BillOfMaterials.Where(b => b.ComponentID == bom.ComponentID && b.PartID == bom.PartID);
                if (bomv.Any())
                {
                    var oldBOM = bomv.First();
                    oldBOM.Quantity = oldBOM.Quantity + bom.Quantity;
                }
                else
                {
                    ctx.BillOfMaterials.Add(bom);
                }
                ctx.SaveChanges();
            }
        }

        public void RemoveBOM(BillOfMaterial bom)
        {
            using (var ctx = new BOMDataContext())
            {
                ctx.BillOfMaterials.Remove(bom);
                ctx.SaveChanges();
            }
        }       
    }
}
