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
    public class PaidBill  
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = false)]
        public int PaidBillID { get; internal set; }

        [Column(CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public int BillID { get; internal set; }

        private EntityRef<Bill> bill;

        [Association(Storage = "bill", ThisKey = "BillID", OtherKey = "BillID", IsForeignKey = true)]
        public Bill Bill
        {
            get { return bill.Entity; }
            set
            {
                if (value != null)
                {
                    BillID = value.BillID;
                }

                bill.Entity = value;
                
            }
        }

         [Column(CanBeNull = true)]
        public DateTime PaidDate { get; internal set; }

         [Column(CanBeNull = true)]
        public Decimal Amount { get; internal set; }

    }
}
