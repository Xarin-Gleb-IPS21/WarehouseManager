using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManager.Domain.Services;

public interface IDataService<T> where T : class
{
    IReadOnlyList<T> GetAll();
    T? GetById(int id);
    void Add(T item);
    void Update(T item);
    void Delete(int id);
}
