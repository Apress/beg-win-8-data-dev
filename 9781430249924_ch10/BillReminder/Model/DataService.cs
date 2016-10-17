using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BillReminder.Model
{
    public class DataService : IDataService
    {
        public void AddBill(Bill bill)
        {
            App.CurrentApp.DB.Bills.InsertOnSubmit(bill);
            App.CurrentApp.DB.SubmitChanges();
        }

        public Bill GetBillByID(int billId)
        {
            return App.CurrentApp.DB.Bills.First(b => b.BillID == billId);
        }

        public ObservableCollection<Billtem> GetBills(DateTime month)
        {
            var bills = new ObservableCollection<Billtem>();
            var fromDate = new DateTime(month.Year, month.Month, 1); //first day of the month
            var toDate = fromDate.AddMonths(1).AddDays(-1); // last day of the month
            var query = from bill in App.CurrentApp.DB.Bills
                        join cat in App.CurrentApp.DB.Categories on bill.CategoryID equals
                            cat.CategoryID
                        join paid in App.CurrentApp.DB.PaidBills on bill.BillID equals paid.BillID into
                            pp
                        from paid in pp.DefaultIfEmpty()
                        where (bill.IsRecurring || (bill.DueDate >= fromDate && bill.DueDate <= toDate))
                        select new Billtem(this)
                            {
                                BillID = bill.BillID
                                ,
                                Name = bill.Name
                                ,
                                Category = cat.Name
                                ,
                                DueDate = bill.DueDate
                                ,
                                Amount = bill.Amount
                                ,
                                PaidAmount = paid.Amount
                                ,
                                PaidDate = paid.PaidDate
                            };
            foreach (var item in query)
            {
                item.IsPaid = (item.PaidAmount > 0 && item.PaidDate > DateTime.MinValue);
                bills.Add(item);
            }

            return bills;
        }

        public IList<Category> GetCategories()
        {
            return App.CurrentApp.DB.Categories.ToList();
        }

        public Category GetCategoryByID(int categoryId)
        {
            return App.CurrentApp.DB.Categories.First(c => c.CategoryID == categoryId);
        }

        public void MarkPaid(int billId, decimal amount)
        {
            PaidBill paidBill;
            if (App.CurrentApp.DB.PaidBills.Count() > 0)
            {
                paidBill = App.CurrentApp.DB.PaidBills.First(b => b.BillID == billId);
            }
            else
            {
                paidBill= new PaidBill();
                paidBill.BillID = billId;
                App.CurrentApp.DB.PaidBills.InsertOnSubmit(paidBill);
            }
            paidBill.Amount = amount;
            paidBill.PaidDate = DateTime.Now;
            App.CurrentApp.DB.SubmitChanges();
        }

        public void UpdateBill(Bill bill)
        {
            App.CurrentApp.DB.SubmitChanges();
        }
    }
}
 