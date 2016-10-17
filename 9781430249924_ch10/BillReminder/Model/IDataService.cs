using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BillReminder.Model
{
    public interface IDataService
    { 
        IList<Category> GetCategories();
        ObservableCollection<Billtem> GetBills(DateTime month);
        void MarkPaid(int billId, decimal amount);
        void AddBill(Bill bill);
        void UpdateBill(Bill bill);
        Bill GetBillByID(int billId);
        Category GetCategoryByID(int categoryId);
    }
}
