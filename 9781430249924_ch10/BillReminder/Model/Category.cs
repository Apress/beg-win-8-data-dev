using GalaSoft.MvvmLight;
using System.Data.Linq.Mapping;
namespace BillReminder.Model
{
    [Table]
    public class Category 
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CategoryID { get; internal set; }

        [Column(CanBeNull = false)]
        public string Name { get; internal set; }
    }
}