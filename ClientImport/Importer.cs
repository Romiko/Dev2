using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientImport.Parsers;

namespace ClientImport
{
    public class Importer
    {
        public IEnumerable<dynamic> Data { get; set; }

        public Importer(string filePath, char delimiter)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var streamReader = new StreamReader(stream);
                var content = streamReader.ReadToEnd();
                LoadDataFromFile(content, delimiter);
            }
        }

        /// <summary>
        /// Loads Data and will close the stream!
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        public Importer(Stream stream, char delimiter)
        {
            using (stream)
            {
                var streamReader = new StreamReader(stream);
                var content = streamReader.ReadToEnd();
                LoadDataFromFile(content, delimiter);
            }
        }

        private void LoadDataFromFile(string content, char delimiter)
        {
                Data = from dynamic c in new DelimiterParser(content, delimiter)
                       select c;
            }
        }
    }

