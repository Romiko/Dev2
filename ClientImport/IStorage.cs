using System.Collections.Generic;
using ClientImport.DTO;

namespace ClientImport
{
    public enum SortKey { SurnameFirstNameAge, SurnameFirstName }

    public interface IStorage <in TData>
    {
        SortKey DefaultSortKey { get; }
        void Add(TData person);
        void Import(IEnumerable<dynamic> records, bool ignoreDuplicates = true);
    }
}
