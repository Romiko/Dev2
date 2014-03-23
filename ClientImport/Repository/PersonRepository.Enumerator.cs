using System;
using System.Collections;
using System.Collections.Generic;
using ClientImport.DTO;

namespace ClientImport.Repository
{
    public partial class PersonRepository
    {
        class Enumerator : IEnumerator<IPerson>
        {
            private readonly IPerson[] people;

            int position = -1;

            public Enumerator(IPerson[] list)
            {
                people = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < people.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public IPerson Current
            {
                get
                {
                    try
                    {
                        return people[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void Dispose()
            {
            }
        }
    }
}
