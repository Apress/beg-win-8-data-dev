using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
public interface IDataRepository
{
    void CreateInstance();
    void AddCategory(Category cat);
    void DeletePassword(Guid id);
    void SavePassword(Password pwd, bool isnew);
    List<Category> GetCategories();
    List<Password> GetAllPasswords();
}
}
