using System;
using BillReminder.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BillReminder.Design
{
    public class DesignDataService : IDataService
    {      

        public string DBPath
        {
            get
            {
                BillReminder.App app = (App.Current as App);
                return app.DBPath;
            }
        }

        public IList<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Table<Category>().OrderBy(c => c.Name);
                foreach (var _category in query)
                {
                    var category = new Category()
                    {
                        CategoryID = _category.CategoryID,
                        Name = _category.Name                       
                    };
                    categories.Add(category);
                }
            }
            return categories;
        }

        public ObservableCollection<Billtem> GetBills(DateTime month)
        {
            DateTime fromDate = new DateTime(month.Year, month.Month, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);
            string sql = string.Format("SELECT b.BillID,b.Name, c.Name as Category, b.DueDate, p.PaidDate, b.Amount, p.Amount as PaidAmount  FROM Bill b Join Category c on b.CategoryID= c.CategoryID LEFT JOIN PaidBill p on p.BillID = b.BillID WHERE (b.IsRecurring = 1 or b.DueDate BETWEEN  '{0}' AND '{1}')", fromDate.ToString("MM/dd/yyy"), toDate.ToString("MM/dd/yyy"));
            var bills = new ObservableCollection<Billtem>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Query<Billtem>(sql);

                foreach (var item in query)
                {
                    Billtem di = new Billtem();
                    di.BillID = item.BillID;
                    di.Name = item.Name;
                    di.Category = item.Category;
                    di.DueDate = item.DueDate;
                    di.Amount = item.Amount;
                    di.PaidAmount = item.PaidAmount;
                    di.PaidDate = item.PaidDate;
                    bills.Add(di);
                }

            }
            return bills;
        }

        public void MarkPaid(int billID, decimal amount)
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                db.Execute("INSERT INTO PaidBill (BillID, PaidDate, Amount) values (?,?,?)", billID, DateTime.Today.ToString("MM/dd/yyyy"), amount);
            }
        }

        public void AddBill(Bill bill)
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                db.Insert(bill);
            }
        }

        public void UpdateBill(Bill bill)
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                db.Update(bill);
            }
        }

        public Bill GetBillByID(int billID)
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                return db.Get<Bill>(billID);
            }
        }

        public Category GetCategoryByID(int categoryID)
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                return db.Get<Category>(categoryID);
            }
        }
    }
}