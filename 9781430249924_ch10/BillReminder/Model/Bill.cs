using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.Linq;
namespace BillReminder.Model
{

    [Table]
    public class Bill  
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int BillID { get; internal set; }

        [Column(CanBeNull = false)]
        public string Name { get; internal set; }

         [Column(CanBeNull = false)]
        public DateTime DueDate { get; internal set; }

         [Column(CanBeNull = false)]
        public bool IsRecurring { get; internal set; }

         [Column(CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int CategoryID{get; internal set; }

         private EntityRef<Category> category;

         [Association(Storage = "category", ThisKey = "CategoryID", OtherKey = "CategoryID", IsForeignKey = true)]
         public Category Category
         {
             get { return category.Entity; }
             set
             {
                 if (value != null)
                 {
                     CategoryID = value.CategoryID;
                 }                
                 category.Entity = value;                
             }
         }

         [Column(CanBeNull = false)]
        public Decimal Amount { get; internal set; }
    }
}
