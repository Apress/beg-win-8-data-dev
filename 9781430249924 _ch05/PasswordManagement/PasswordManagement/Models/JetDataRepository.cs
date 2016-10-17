using Microsoft.Isam.Esent.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PasswordManager.Models
{
    public class JetDataRepository:IDataRepository
    {
        private Instance _instance;
        private string _instancePath;
        private string _databasePath;
        private const string DatabaseName = "_PasswordDB";

        public void CreateInstance()
        {
            _instancePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseName);
            _databasePath = Path.Combine(_instancePath, "_Password.edb");
            _instance = new Instance(_databasePath);          
            _instance.Parameters.CreatePathIfNotExist = true;
            _instance.Parameters.TempDirectory = Path.Combine(_instancePath, "temp");
            _instance.Parameters.SystemDirectory = Path.Combine(_instancePath, "system");
            _instance.Parameters.LogFileDirectory = Path.Combine(_instancePath, "logs");
            _instance.Parameters.Recovery = true;
            _instance.Parameters.CircularLog = true;
            _instance.Init();
            CreateDatabase();
        }

        public async Task<bool> IsFileExist(string path)
        {
            try
            {
                await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
                return (true);
            }
            catch
            {
                return (false);
            }
        }

        private  async void CreateDatabase()
        {
            if (await IsFileExist(_databasePath))
                return;
            using (var session = new Session(_instance))
            {
                
                JET_DBID database;
                Api.JetCreateDatabase(session
                    , _databasePath
                    , null
                    , out database
                    , CreateDatabaseGrbit.None);

                // create database schema
                using (var transaction = new Transaction(session))
                {

                    //Schema for Category Table
                    JET_TABLEID categoryTableId;
                    Api.JetCreateTable(session
                        , database
                        , "Categories" //table name
                        , 1
                        , 100
                        , out categoryTableId);
                    JET_COLUMNID categoryColumnid;

                    //CategoryId column
                    Api.JetAddColumn(session
                        , categoryTableId
                        , "CategoryId" //column name
                        , new JET_COLUMNDEF
                        {
                            cbMax = 16,
                            coltyp = JET_coltyp.IEEESingle,
                            grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
                        }
                        , null
                        , 0
                        , out categoryColumnid);

                    //CategoryName column
                    Api.JetAddColumn(session
                        , categoryTableId
                        , "CategoryName" //column name
                        ,  new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }, null, 0, out categoryColumnid);

                    //Creating Index
                    var categoryindexDef = "+CategoryId\0\0";
                    Api.JetCreateIndex(session
                        , categoryTableId
                        , "CategoryId_index" //index name
                        , CreateIndexGrbit.IndexPrimary
                        , categoryindexDef
                        , categoryindexDef.Length
                        , 100);


                    //Schema for Password table
                    JET_TABLEID passwordTableid;
                    Api.JetCreateTable(session
                        , database
                        , "Passwords" //table name
                        , 1
                        , 100
                        , out passwordTableid);

                    //creating columns for Password tables
                    JET_COLUMNID passwordColumnid;
                    Api.JetAddColumn(session
                        , passwordTableid
                        , "PasswordId" //column name
                        , new JET_COLUMNDEF
                        {
                            cbMax = 16,
                            coltyp = JET_coltyp.Binary,
                            grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "Title" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "UserName" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "Passcode" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "WebSite" //column name
                        ,  new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "Key" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "CategoryId" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.IEEESingle,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    Api.JetAddColumn(session
                        , passwordTableid
                        , "Note" //column name
                        , new JET_COLUMNDEF
                        {
                            coltyp = JET_coltyp.LongText,
                            cp = JET_CP.Unicode,
                            grbit = ColumndefGrbit.None
                        }
                        , null
                        , 0
                        , out passwordColumnid);

                    //creating index for Passwords table
                    var indexDef = "+PasswordId\0\0";
                    Api.JetCreateIndex(session
                        , passwordTableid
                        , "PasswordId_index" //index name
                        , CreateIndexGrbit.IndexPrimary
                        , indexDef
                        , indexDef.Length
                        , 100);

                    transaction.Commit(CommitTransactionGrbit.None);
                }
                Api.JetCloseDatabase(session, database, CloseDatabaseGrbit.None);
                Api.JetDetachDatabase(session, _databasePath);
            }

            //Add defult values to the database table 
            CreateDefaultData();
        }

        private void CreateDefaultData()
        {
            //Adding categories
            AddCategory(new Category { 
                CategoryId = 1
                , CategoryName = "Bank" 
            });
            AddCategory(new Category { 
                CategoryId = 2
                , CategoryName = "Web Site" 
            });
           
            //Adding password
            SavePassword(new Password { 
                PasswordId = Guid.NewGuid()
                , Title = "Capital One"
                , UserName = "vinodh-kumar"
                , WebSite = "www.capitalone.com"
                , Passcode = "book8data"
                , CategoryId = 1 
            }
            , true);
            SavePassword(new Password { 
                PasswordId = Guid.NewGuid()
                , Title = "Bank of America"
                , UserName = "vinodh-kumar"
                , Passcode = "boa8data"
                , CategoryId = 1
                , Key = "3121"
                , WebSite = "www.bankofamerica.com" 
            }
            , true);            
        }

        //More data

     /* AddCategory(new Category { CategoryId = 3, CategoryName = "Utilities" });
        AddCategory(new Category { CategoryId = 4, CategoryName = "Subscriptions" });
        AddCategory(new Category { CategoryId = 5, CategoryName = "Software" });
        AddCategory(new Category { CategoryId = 6, CategoryName = "Other" });

       SavePassword(new Password { PasswordId = Guid.NewGuid(), Title = "Youtube", UserName = "vinodh.kumar", Passcode = "tigerforce", CategoryId = 2, WebSite = "www.youtube.com" }, true);
       SavePassword(new Password { PasswordId = Guid.NewGuid(), Title = "Outlook", UserName = "vinodh-kumar", Passcode = "funtime432", CategoryId = 2, WebSite = "www.outlook.com" }, true);
      */
        public void AddCategory(Category ev)
        {
            using (var session = new Session(_instance))
            {
                JET_DBID dbid;
                Api.JetAttachDatabase(session
                    , _databasePath
                    , AttachDatabaseGrbit.None);
                //Opening database
                Api.JetOpenDatabase(session
                    , _databasePath
                    , String.Empty
                    , out dbid
                    , OpenDatabaseGrbit.None);
                //within a transaction
                using (var transaction = new Transaction(session))
                {
                    //opening the table
                    using (var table = new Table(session
                        , dbid
                        , "Categories"
                        , OpenTableGrbit.None))
                    {
                        //inserting row
                        using (var updater = new Update(session, table, JET_prep.Insert))
                        {
                            var columnId = Api.GetTableColumnid(session
                                , table
                                , "CategoryId"); //to CategoryId column
                            Api.SetColumn(session
                                , table
                                , columnId
                                , ev.CategoryId);

                            var columnDesc = Api.GetTableColumnid(session
                                , table
                                , "CategoryName"); //to CategoryName column
                            Api.SetColumn(session
                                , table
                                , columnDesc
                                , ev.CategoryName
                                , Encoding.Unicode);

                            updater.Save();
                        }
                    }
                    transaction.Commit(CommitTransactionGrbit.LazyFlush);
                }
            }
        }

        public void DeletePassword(Guid id)
        {
            ExecuteInTransaction((session, table) =>
            {
                Api.JetSetCurrentIndex(session, table, null);
                Api.MakeKey(session, table, id, MakeKeyGrbit.NewKey);
                if (Api.TrySeek(session, table, SeekGrbit.SeekEQ))
                {
                    Api.JetDelete(session, table);
                }
                return null;
            });
        }

        public void SavePassword(Password pwd, bool isnew)
        {
            ExecuteInTransaction((session, table) =>
                {
                    using (var updater = new Update(session, table, isnew ? JET_prep.Insert : JET_prep.Replace))
                    {
                        //set the password id depending upon the isnew parameter
                        if (isnew)
                        {
                            var columnId = Api.GetTableColumnid(session, table, "PasswordId");
                            Api.SetColumn(session, table, columnId, pwd.PasswordId);
                        }
                        //Title
                        var columnTitle = Api.GetTableColumnid(session, table, "Title");
                        Api.SetColumn(session, table, columnTitle, pwd.Title, Encoding.Unicode);
                        //UserName
                        var columnUserName = Api.GetTableColumnid(session, table, "UserName");
                        Api.SetColumn(session, table, columnUserName, pwd.UserName, Encoding.Unicode);
                        //Passcode
                        var columnPasscode = Api.GetTableColumnid(session, table, "Passcode");
                        Api.SetColumn(session, table, columnPasscode, pwd.Passcode, Encoding.Unicode);
                        //WebSite
                        var columnWebSite = Api.GetTableColumnid(session, table, "WebSite");
                        Api.SetColumn(session, table, columnWebSite, pwd.WebSite, Encoding.Unicode);
                        //Key
                        var columnKey = Api.GetTableColumnid(session, table, "Key");
                        Api.SetColumn(session, table, columnKey, pwd.Key, Encoding.Unicode);
                        //CategoryId
                        var columnCategoryId = Api.GetTableColumnid(session, table, "CategoryId");
                        Api.SetColumn(session, table, columnCategoryId, pwd.CategoryId);
                        //Note
                        var columnNote = Api.GetTableColumnid(session, table, "Note");
                        Api.SetColumn(session, table, columnNote, pwd.Note, Encoding.Unicode);
                        
                        updater.Save();
                    }
                    return null;
                });
        }

        public List<Category> GetCategories()
        {
            List<Category> results;
            using (var session = new Session(_instance))
            {
                JET_DBID dbid;
                Api.JetAttachDatabase(session, _databasePath, AttachDatabaseGrbit.None);
                Api.JetOpenDatabase(session, _databasePath, String.Empty, out dbid, OpenDatabaseGrbit.None);
                using (var transaction = new Transaction(session))
                {
                    using (var table = new Table(session, dbid, "Categories", OpenTableGrbit.ReadOnly))
                    {
                        results = new List<Category>();
                        if (Api.TryMoveFirst(session, table))
                        {
                            do
                            {
                                var category = new Category();
                                var columnId = Api.GetTableColumnid(session, table, "CategoryId");
                                category.CategoryId = Api.RetrieveColumnAsInt32(session, table, columnId) ?? -1;
                                var columnDesc = Api.GetTableColumnid(session, table, "CategoryName");
                                category.CategoryName = Api.RetrieveColumnAsString(session, table, columnDesc, Encoding.Unicode);
                                results.Add(category);
                            }
                            while (Api.TryMoveNext(session, table));
                        }
                    }
                }
            }
            return results;
        }

        private IList<Password> ExecuteInTransaction(Func<Session, Table, IList<Password>> dataFunc)
        {
            IList<Password> results;
            using (var session = new Session(_instance))
            {
                JET_DBID dbid;
                Api.JetAttachDatabase(session, _databasePath, AttachDatabaseGrbit.None);
                Api.JetOpenDatabase(session, _databasePath, String.Empty, out dbid, OpenDatabaseGrbit.None);
                using (var transaction = new Transaction(session))
                {
                    using (var table = new Table(session, dbid, "Passwords", OpenTableGrbit.None))
                    {
                        results = dataFunc(session, table);
                    }

                    transaction.Commit(CommitTransactionGrbit.None);
                }
            }

            return results;
        }

        public List<Password> GetAllPasswords()
        {
            List<Password> results = null;
            ExecuteInTransaction((session, table) =>
            {
                results = new List<Password>();
                if (Api.TryMoveFirst(session, table))
                {
                    do
                    {
                        //Call GetPassword method to create password object
                        //from the table row
                        results.Add(GetPassword(session, table));
                    }
                    while (Api.TryMoveNext(session, table));
                }
                return results;
            });
            return results;
        }

        private Password GetPassword(Session session, Table table)
        {
            var password = new Password();
            //retriving PasswordId column
            var columnId = Api.GetTableColumnid(session, table, "PasswordId");
            //assigning it to the PasswordId property
            password.PasswordId = Api.RetrieveColumnAsGuid(session, table, columnId) ?? Guid.Empty;
            //retriving Title
            var columnTitle = Api.GetTableColumnid(session, table, "Title");
            password.Title = Api.RetrieveColumnAsString(session, table, columnTitle, Encoding.Unicode);
            //retriving UserName
            var columnUsername = Api.GetTableColumnid(session, table, "UserName");
            password.UserName = Api.RetrieveColumnAsString(session, table, columnUsername, Encoding.Unicode);
            //retriving Passcode
            var columnPasscode = Api.GetTableColumnid(session, table, "Passcode");
            password.Passcode = Api.RetrieveColumnAsString(session, table, columnPasscode, Encoding.Unicode);
            //retriving WebSite
            var columnWebSite = Api.GetTableColumnid(session, table, "WebSite");
            password.WebSite = Api.RetrieveColumnAsString(session, table, columnWebSite, Encoding.Unicode);
            //retriving Key
            var columnKey = Api.GetTableColumnid(session, table, "Key");
            password.Key = Api.RetrieveColumnAsString(session, table, columnKey, Encoding.Unicode);
            //retriving Note
            var columnNote = Api.GetTableColumnid(session, table, "Note");
            password.Note = Api.RetrieveColumnAsString(session, table, columnNote, Encoding.Unicode);
            //retriving CategoryId
            var columnCategoryId = Api.GetTableColumnid(session, table, "CategoryId");
            password.CategoryId = Api.RetrieveColumnAsInt32(session, table, columnCategoryId) ?? -1;
            return password;
        }

        
    }
}