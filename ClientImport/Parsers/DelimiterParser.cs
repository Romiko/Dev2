using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClientImport.Parsers
{
    public class DelimiterParser : IEnumerable
    {
        private readonly char delimiter;
        private readonly IList<string> headers;
        private readonly string[] records;

        public DelimiterParser(string csvContent, char delimiter)
        {
            this.delimiter = delimiter;
            records = csvContent.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (records.Length <= 0) return;
            headers = records[0].Split(delimiter).ToList();

            for (var i = 0; i <= headers.Count - 1; i++)
                headers[i] = headers[i].Trim();
        }

        public IEnumerator GetEnumerator()
        {
            var header = true;

            foreach (var line in records)
                if (header)
                    header = false;
                else
                    yield return new DelimiterLine(line, headers, delimiter);
        }
    }
}
