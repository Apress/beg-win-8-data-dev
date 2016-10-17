using Microsoft.Isam.Esent.Interop;
using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using WinRTDatabase;
using WinRTDatabase.Core;
 

namespace PasswordManager.Models
{
    public class ApplicationDataRepository:IDataRepository
    {
        private Database _database;
        private const string DatabaseName = "_PasswordDB";

        public async void CreateInstance()
        {           
            var exists = await Database.DoesDatabaseExistsAsync(DatabaseName, StorageLocation.Local);
            if (!exists)
            {
                _database = await Database.CreateDatabaseAsync(DatabaseName, StorageLocation.Local);
                _database.CreateTable<Category>();
                _database.CreateTable<Password>();
                
                var categoriesTable  = await _database.Table<Category>();
                var passwordsTable  = await _database.Table<Password>();
              
                Categories = categoriesTable;
                Passwords = passwordsTable;
                CreateDefaultData();

                SaveResult result =  await _database.SaveAsync();
                if (result.Error == null)
                {
                    Debug.WriteLine(result.Error == null ? "Database created with Defult data" : result.Error.Message);
                }
            }
            else
            {
                _database = await Database.OpenDatabaseAsync(DatabaseName, true, StorageLocation.Local);
                Categories = await _database.Table<Category>();
                Passwords = await _database.Table<Password>();
            }
        }

        public Table<Category> Categories
        { get; set; }

        public Table<Password> Passwords
        { get; set; }


        public async void SavePassword(Password password, bool isnew=true)
        {
            if (isnew)
            {
                Passwords.Add(password);
            }
            SaveResult result  = await _database.SaveAsync();
            if (result.Error == null)
            {
                Debug.WriteLine(result.Error == null ? "Saved password" : result.Error.Message);
            }
        }

        public async void AddCategory(Category category)
        {
            Categories.Add(category);
            SaveResult result  = await _database.SaveAsync();
            if (result.Error == null)
            {
                Debug.WriteLine(result.Error == null ? "Saved Category" : result.Error.Message);
            }          
        } 

        private void CreateDefaultData()
        {
            Categories.Add(new Category { CategoryId = 1, CategoryName = "Bank" });
            Categories.Add(new Category { CategoryId = 2, CategoryName = "Web Site" });
            Categories.Add(new Category { CategoryId = 3, CategoryName = "Utilities" });
            Categories.Add(new Category { CategoryId = 4, CategoryName = "Subscriptions" });
            Categories.Add(new Category { CategoryId = 5, CategoryName = "Software" });
            Categories.Add(new Category { CategoryId = 6, CategoryName = "Other" });


            Passwords.Add(new Password { PasswordId = Guid.NewGuid(), Title = "Capital One", UserName = "vinodh-kumar", WebSite = "www.capitalone.com", Passcode = "book8data", CategoryId = 1 });
            Passwords.Add(new Password { PasswordId = Guid.NewGuid(), Title = "Bank of America", UserName = "vinodh-kumar", Passcode = "boa8data", CategoryId = 1, Key = "3121", WebSite = "www.bankofamerica.com" });
            Passwords.Add(new Password { PasswordId = Guid.NewGuid(), Title = "Youtube", UserName = "vinodh.kumar", Passcode = "tigerforce", CategoryId = 2, WebSite = "www.youtube.com" });
            Passwords.Add(new Password { PasswordId = Guid.NewGuid(), Title = "Outlook", UserName = "vinodh-kumar", Passcode = "funtime432", CategoryId = 2, WebSite = "www.outlook.com" });
        }


        public async void DeletePassword(Guid id)
        {
            var password = Passwords.Where(p => p.PasswordId == id).FirstOrDefault();
            Passwords.Remove(password);
            SaveResult result  =  await _database.SaveAsync();
            if (result.Error == null)
            {
                Debug.WriteLine(result.Error == null ? "Deleted Password" : result.Error.Message);
            }
        }       

        public List<Category> GetCategories()
        {
            if (Categories == null) return null;
            return Categories.ToList();
        } 

        public List<Password> GetAllPasswords()
        {
            if (Passwords == null) return null;
            return Passwords.ToList();           
        }
    }
}
