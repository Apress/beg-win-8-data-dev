
using System.Data.Linq;

namespace BillReminder.Model
{
    public class BillReminderDataContext : DataContext
    {
        public BillReminderDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<Bill> Bills
        {
            get
            {
                return this.GetTable<Bill>();
            }
        }

        public Table<Category> Categories
        {
            get
            {
                return this.GetTable<Category>();
            }
        }

        public Table<PaidBill> PaidBills
        {
            get
            {
                return this.GetTable<PaidBill>();
            }
        }
    }
}
