using ImageSearch.Services;
using ImageSearch.ViewModel;
using System.Text;
using System.Windows;

namespace ImageSearch.View
{
    /// <summary>
    /// </summary>
    public partial class ImageImportView : Window
    {
        public ImageImportView()
        {
            InitializeComponent();

            // In a commercial environment, I would implement DI and provide the service through a DI container.
            DataContext = new ImageImporterViewModel(new ImportService());
        }
    }
}