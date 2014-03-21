using System.Collections.Generic;
using System.Dynamic;

namespace ClientImport.Parsers
{
    internal class DelimiterLine : DynamicObject
    {
        private readonly string[] record;
        private readonly IList<string> headers;

        public DelimiterLine(string line, IList<string> headers, char delimiter)
        {
            record = line.Split(delimiter);
            this.headers = headers;
        }

        public override bool TryGetMember(
            GetMemberBinder binder,
            out object result)
        {
            result = null;
            var index = headers.IndexOf(binder.Name);
            if (index >= 0 && index < record.Length)
            {
                result = record[index];
                return true;
            }
            return false;
        }
    }
}
