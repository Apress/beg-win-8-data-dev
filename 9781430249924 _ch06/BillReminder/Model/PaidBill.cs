using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace BillReminder.Model
{
    class PaidBill  
    {
        [PrimaryKey]
        public int PaidBillID { get; internal set; }

        [Indexed]
        public int BillID { get; internal set; }

        public DateTime PaidDate { get; internal set; }

        public Decimal Amount { get; internal set; }

    }
}
