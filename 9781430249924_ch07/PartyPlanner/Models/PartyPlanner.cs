using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Spatial;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
 
namespace PartyPlanner.Models
{

    public class Party
    {
        public int PartyID { get; set; }
        public string PartyName { get; set; }
        public string DateTime { get; set; }
        public bool Completed { get; set; }
        public virtual ICollection<Guest> Guests { get; set; }
        public virtual ICollection<ShoppingItem> ShoppingList { get; set; }
    }

    public class Guest
    {
        public int GuestID { get; set; }
        public string FamilyName { get; set; }
        public int NumberOfPeople { get; set; }
        [JsonIgnore]
        public Party Party { get; set; }
    }

    public class ShoppingItem
    {
        public int ShoppingItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public Party Party { get; set; }
    }     
     
}