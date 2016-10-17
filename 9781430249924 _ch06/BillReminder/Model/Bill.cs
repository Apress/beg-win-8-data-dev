using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace BillReminder.Model
{
public class Bill  
{
    [PrimaryKey, AutoIncrement]
    public int BillID { get; internal set; }

    [MaxLength(150)]
    public string Name { get; internal set; }

    public DateTime DueDate { get; internal set; }

    public bool IsRecurring { get; internal set; }

    public int CategoryID{get; internal set; }

    public Decimal Amount { get; internal set; }
}
}
