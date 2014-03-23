using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientImport.DTO;
using ClientImport.DataStructures;

namespace ClientImport.Repository
{
    public partial class PersonRepository : IRepository<IPerson>
    {
        private readonly BinarySearchTree<string, IPerson> data;
        public SortKey DefaultSortKey { get { return defaultSortKey; } }

        readonly SortKey defaultSortKey = SortKey.SurnameFirstNameAge;

        public PersonRepository()
        {
            data = new BinarySearchTree<string, IPerson>();
        }

        public PersonRepository(SortKey sortKey)
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

            if (data.Find(key) == null)
                data.Add(key, person);
            else
                throw new ArgumentException("Duplicate Key found.");
        }

        public void Delete(IPerson person)
        {
            var key = BuildKey(person, defaultSortKey);
            var node = data.Find(key);
            if(node != null)
                data.Delete(key);
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
                                 else if (data.Find(key) == null)
                                 {
                                     Add(person);
                                 }
                             }
                   );
        }

        public IEnumerator<IPerson> GetEnumerator()
        {
            return new Enumerator(data.Select(r => r.KeyValue.Value).ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}