using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageSearch.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace ImageSearch.ViewModel
{
    internal partial class ImageImporterViewModel : ObservableObject
    {
        private IImportService importService;

        /// <summary>
        /// Image Importer VM
        /// </summary>
        /// <param name="service">Service to Import Images</param>
        public ImageImporterViewModel(IImportService service)
        {
            importService = service;
        }

        [ObservableProperty]
        private ImageViewModel selectedImage;
        public bool ImageSelected => SelectedImage != null;
        partial void OnSelectedImageChanged(ImageViewModel value)
        {
            OnPropertyChanged(nameof(ImageSelected));
        }




        [ObservableProperty]
        private ObservableCollection<ImageViewModel> importedImages = new();
        [ObservableProperty]
        private ObservableCollection<ImageViewModel> filteredImages = new();
        public bool ImagesLoaded => ImportedImages.Any();



        [ObservableProperty]
        private SearchCriteriaViewModel searchCriteriaDataContext = new SearchCriteriaViewModel(new GeolocationService());

        [RelayCommand]
        private void ImportImages()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = importService.FileTypes
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilteredImages.Clear();
                ImportedImages.Clear();

                var images = importService.ImportImages(openFileDialog.FileName).ToList();

                foreach (var image in images)
                {
                    ImportedImages.Add(image);
                    FilteredImages.Add(image);
                }

                SearchCriteriaDataContext.AvailableProperties =
                    new ObservableCollection<string>(
                        new[] { "DPI", "Height", "Name", "Size", "Tag", "Width" }
                        .Concat(ImportedImages
                            .SelectMany(img => img.Image.Metadata)
                            .Select(meta => meta.Key))
                        .Distinct()
                        .OrderBy(x => x)
                    );
            }
            OnPropertyChanged(nameof(ImagesLoaded));
        }


        //In a commercial environment, I would setup the search to search on the remaining set of items when new criteria is added and to search on items removed when criteria is removed instead of searching them all every time.

        [RelayCommand]
        private void Search()
        {
            FilteredImages.Clear();
            

            var results = ImportedImages.AsParallel().Where(img => SearchCriteriaDataContext.MatchesCriterion(img));

            foreach (var item in results)
                FilteredImages.Add(item);

            SelectedImage = FilteredImages.FirstOrDefault();
        }
    }
}