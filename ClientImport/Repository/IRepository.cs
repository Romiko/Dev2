using System.Collections.Generic;

namespace ClientImport.Repository
{
    public enum SortKey { SurnameFirstNameAge, SurnameFirstName }

    public interface IRepository <TData> : IEnumerable<TData>
    {
        SortKey DefaultSortKey { get; }
        void Add(TData data);
        void Delete(TData data);
        void Import(IEnumerable<dynamic> records, bool ignoreDuplicates = true);
    }
}
