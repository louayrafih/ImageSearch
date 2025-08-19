using CsvHelper;
using ImageSearch.Model;
using ImageSearch.ViewModel;
using System.Globalization;
using System.IO;

namespace ImageSearch.Services
{
    internal class ImportService : IImportService
    {
        public string FileTypes => "Csv Files|*.csv";

        private static readonly Dictionary<string, Func<string, IEnumerable<ImageViewModel>>> importFactory = new Dictionary<string, Func<string, IEnumerable<ImageViewModel>>>();

        /// <summary>
        /// Importer that supports importing images from different file formats. Currently supports CSV files.
        /// </summary>
        public ImportService()
        {
            importFactory.Add("csv", ImportImagesCsv);
        }

        public IEnumerable<ImageViewModel> ImportImages(string fileName)
        {
            var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
            return importFactory[extension](fileName);
        }


        /// <summary>
        /// Imports images from a CSV file.
        /// </summary>
        /// <param name="fileName"> csv file path</param>
        /// <returns></returns>
        private IEnumerable<ImageViewModel> ImportImagesCsv(string fileName)
        {
            //In commercial environment I would add checks in place for expected properties and ensure the file exists and is readable.
            using var reader = new StreamReader(fileName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Read header row
            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            // Read each row
            while (csv.Read())
            {
                //Assumption: The first 6 columns are uneditable image properties in order (name, type, Size (MB), X length, Y length, DPI), the last column contains tags and the rest are metadata.
                var image = new ImageImportModel(csv.GetField(headers[0]), csv.GetField(headers[1]), Convert.ToDouble(csv.GetField(headers[2])), Convert.ToInt32(csv.GetField(headers[3])), Convert.ToInt32(csv.GetField(headers[4])), Convert.ToInt32(csv.GetField(headers[5])));

                for (int i = 6; i < headers.Length - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(csv.GetField(headers[i])))
                        image.Metadata.Add(headers[i], csv.GetField(headers[i]));
                }

                var tags = csv.GetField(headers[^1])
                                .Replace("\"", string.Empty)
                                .Split(',')
                                .Select(tag => tag.Trim())
                                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                                .ToList();
                foreach (var tag in tags)
                    image.Tags.Add(tag);

                yield return new ImageViewModel(image);
            }
        }
    }
}
