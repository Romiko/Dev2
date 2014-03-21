using System;
using System.Collections.Generic;
using System.Linq;
using ClientImport.DTO;
using ClientImport.DataStructures;

namespace ClientImport
{
    public interface IBinaryTree<in TKey, in TValue>
    {
        void Add(TKey key, TValue value);
    }

    public class PersonStorage : IStorage<IPerson>
    {
        public UnbalancedBinaryTree<string, IPerson> Data;
        public SortKey DefaultSortKey { get { return defaultSortKey; } }

        readonly SortKey defaultSortKey = SortKey.SurnameFirstNameAge;

        public PersonStorage()
        {
            Data = new UnbalancedBinaryTree<string, IPerson>();
        }

        public PersonStorage(SortKey sortKey)
            : this()
        {
            defaultSortKey = sortKey;
        }

        public static string BuildKey(IPerson person, SortKey key)
        {
            switch (key)
            {
                case SortKey.SurnameFirstName:
                    return string.Concat(person.Surname, person.FirstName);
                default:
                    return string.Concat(person.Surname, person.FirstName, person.Age);
            }
        }

        public void Add(IPerson person)
        {
            var key = BuildKey(person, defaultSortKey);

            if (Data.Find(key) == null)
                Data.Add(key, person);
            else
                throw new ArgumentException("Duplicate Key found.");
        }

        public void Import(IEnumerable<dynamic> records, bool ignoreDuplicates = true)
        {
            records
                .ToList()
                .ForEach(record =>
                             {
                                 var person = new Person
                                                  {
                                                      FirstName = record.FirstName,
                                                      Surname = record.Surname,
                                                      Age = Convert.ToInt16(record.Age)
                                                  };
                                 var key = BuildKey(person, defaultSortKey);
                                 if (!ignoreDuplicates)
                                     Add(person);
                                 else if (Data.Find(key) == null)
                                 {
                                     Add(person);
                                 }
                             }
                   );
        }
    }
}