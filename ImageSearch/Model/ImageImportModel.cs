using System.Windows.Media.Imaging;

namespace ImageSearch.Model
{
    internal class ImageImportModel
    {

        public ImageImportModel(string name, string type, double size, int xWidthPi, int yHeightPi, int dpi)
        {
            Name = name;
            FileType = type;
            Size = size;
            WidthPi = xWidthPi;
            HeightPi = yHeightPi;
            Dpi = dpi;
        }

        public BitmapSource Image { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public double Size { get; set; } // Size in MB
        public int WidthPi { get; set; } // Width in pixels
        public int HeightPi { get; set; } // Height in pixels
        public int Dpi { get; set; } 


        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
        public IList<string> Tags { get; } = new List<string>();  
    }
}
