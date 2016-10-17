using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BillReminder.Model
{
    public interface IDataService
    {
        //void GetData(Action<DataItem, Exception> callback);
        string DBPath { get; }
        IList<Category> GetCategories();
        ObservableCollection<Billtem> GetBills(DateTime month);
        void MarkPaid(int billID, decimal amount);
        void AddBill(Bill bill);
        void UpdateBill(Bill bill);
        Bill GetBillByID(int billID);
        Category GetCategoryByID(int categoryID);
    }
}
