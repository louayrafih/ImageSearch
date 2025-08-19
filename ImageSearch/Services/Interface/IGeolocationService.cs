using ImageSearch.Model;

namespace ImageSearch.Services
{
    internal interface IGeolocationService
    {
        /// <summary>
        /// Checks if the center point of the image is within the polygon defined by the points.
        /// </summary>
        /// <param name="polygonPoints">List of points to compose the polygon</param>
        /// <param name="image">Image to check if it has a center point and whether it is in the polygon</param>
        /// <returns> 
        /// A boolean indicating whether the center point of the image is within the polygon defined by the points.
        /// </returns>
        bool IsWithinPolygon(IEnumerable<PolygonPointModel> polygonPoints, ImageImportModel image);
    }
}
