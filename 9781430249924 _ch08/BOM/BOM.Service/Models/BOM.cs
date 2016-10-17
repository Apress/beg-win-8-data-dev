using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BOM.Service.Models
{
    [DataContract(IsReference = true)]
    public class Part
    {
        [Key]
        [DataMember]
        public int PartID { get; set; }

        [DataMember]
        public string PartName { get; set; }

        [DataMember]
        public int StockCount { get; set; }

        [DataMember]
        public virtual ICollection<BillOfMaterial> BOMParts { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Component
    {
        
        [Key]
        [DataMember]
        public int ComponentID { get; set; }

        [DataMember]
        public string ComponentName { get; set; }

        [DataMember]
        public virtual ICollection<BillOfMaterial> BOMComponents { get; set; }
    }

    [DataContract(IsReference = true)]
    public class BillOfMaterial 
    {
         [Key]
        [DataMember]
        public int BOMID { get; set; }

        [DataMember]
        public int ComponentID { get; set; }

        [DataMember]
        public int PartID { get; set; }

        [DataMember]
        public int Quantity { get; set; }


        [ForeignKey("PartID"), Column(Order = 0)]

        [DataMember]
        public virtual  Part BOMPart { get; set; }

        [ForeignKey("ComponentID"), Column(Order = 1)]
        [DataMember]
        public virtual Component BOMComponent { get; set; }

 
    }

}