using GalaSoft.MvvmLight;
using SQLite;
namespace BillReminder.Model
{
 
    public class Category 
    {
        [PrimaryKey, AutoIncrement]
        public int CategoryID { get; internal set; }

        [MaxLength(50)]
        public string Name { get; internal set; }
    }
}