using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHelper
{
    public interface IGenericRestClient<T>
    {
        void Add(T entity);

        void Delete(int id);

        IEnumerable<T> GetAll();

        T GetById(int id);

        void Update(T entity, int id);

        IEnumerable<T> GetByFilter(string filter);

        T GetByIdExpand(int id, string expand);

        IEnumerable<T> Execute(string url);
    }
}
