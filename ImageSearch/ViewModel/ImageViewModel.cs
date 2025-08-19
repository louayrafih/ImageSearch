using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageSearch.Model;
using System.Collections.ObjectModel;

namespace ImageSearch.ViewModel
{
    internal partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageImportModel image;


        //View properties
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string size;
        [ObservableProperty]
        private string dimensions;
        [ObservableProperty]
        private string dpi;

        [ObservableProperty]
        private ObservableCollection<string> tags;

        [ObservableProperty]
        private ObservableCollection<string> metadata;

        /// <summary>
        /// key-value pairs of properties for the image.
        /// </summary>
        public ILookup<string, string> PropertyLookup =>
            new[]
            {
                new { Key = "Name", Value = Image.Name },
                new { Key = "Size", Value = Image.Size.ToString() },
                new { Key = "Width", Value = Image.WidthPi.ToString() },
                new { Key = "Height", Value = Image.HeightPi.ToString() },
                new { Key = "DPI", Value = Image.Dpi.ToString() }
            }
            .Concat(
                Image.Tags.Select(tag => new { Key = "Tag", Value = tag })
            )
            .Concat(
                Image.Metadata.Select(md => new { Key = md.Key, Value = md.Value?.ToString() ?? string.Empty })
            )
            .ToLookup(item => item.Key, item => item.Value);


        /// <summary>
        /// Image VM with user friendly properties.
        /// </summary>
        /// <param name="image">source of information</param>
        public ImageViewModel(ImageImportModel image)
        {
            Image = image;
            Name = image.Name;
            Size = $"{image.Size} MB";
            Dimensions = $"{image.WidthPi} x {image.HeightPi}";
            Dpi = $"{image.Dpi} DPI";

            Tags = new ObservableCollection<string>(image.Tags);
            Metadata = new ObservableCollection<string>(image.Metadata.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        
        [RelayCommand]
        private void AddTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag) && !Image.Tags.Contains(tag))
            {
                Image.Tags.Add(tag);
                Tags.Add(tag);
            }
        }

        [RelayCommand]
        private void RemoveTag(string tag)
        {
            if (Image.Tags.Contains(tag))
            {
                Image.Tags.Remove(tag);
                Tags.Remove(tag);
            }
        }
    }
}
