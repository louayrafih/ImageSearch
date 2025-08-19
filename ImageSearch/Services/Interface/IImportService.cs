using ImageSearch.Model;
using ImageSearch.ViewModel;

namespace ImageSearch.Services
{
    internal interface IImportService
    {
        /// <summary>
        /// Supported file types for importing images. Use in OpenFileDialog.
        /// </summary>
        string FileTypes { get; }

        /// <summary>
        /// imports images from a file based on the file extension.
        /// </summary>
        /// <param name="fileName">file path </param>
        /// <returns></returns>
        IEnumerable<ImageViewModel> ImportImages(string fileName);
    }
}
