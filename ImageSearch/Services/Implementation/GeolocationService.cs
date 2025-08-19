using ImageSearch.Model;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ImageSearch.Services
{
    internal class GeolocationService : IGeolocationService
    {
        private const string CENTERCOORDINATEKEY = "(Center) Coordinate";


        //Assumption is that this function will just look at the center point property and it will always be setup as the above.
        public bool IsWithinPolygon(IEnumerable<PolygonPointModel> polygonPoints, ImageImportModel image)
        {
            //need 3 points at least and the image needs to have a center point
            if (polygonPoints.Count() < 3 || !image.Metadata.ContainsKey(CENTERCOORDINATEKEY)) return false;

            var centerPoint = GetPointFromString(image.Metadata[CENTERCOORDINATEKEY].ToString());
            if (centerPoint == null) return false;

            var inside = false;
            var polygon = polygonPoints.ToArray();

            //In a commercial environment, I would implement algoriths based on the number of points in the polygon.
            //For example, if the polygon is a triangle, I would use a simpler method than when the polygon has more than 3 points(e.g. some variation of the ray casting algorithm).
            //This was pulled from https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].Latitude < centerPoint.Latitude && polygon[j].Latitude >= centerPoint.Latitude ||
                    polygon[j].Latitude < centerPoint.Latitude && polygon[i].Latitude >= centerPoint.Latitude)
                {
                    if (polygon[i].Longitude + (centerPoint.Latitude - polygon[i].Latitude) /
                       (polygon[j].Latitude - polygon[i].Latitude) *
                       (polygon[j].Longitude - polygon[i].Longitude) < centerPoint.Longitude)
                    {
                        inside = !inside;
                    }
                }
                j = i;
            }

            return inside;
        }

        //Assuming center coord is valid and has one of two setups Lat Long/DMS.
        //In a commercial environment, would make this more robust and consider other formats if there are any.
        /// <summary>
        /// Pulls a point from a string representation of a point.
        /// </summary>
        /// <param name="pointString">Lat, Long or DMS string</param>
        /// <returns>
        /// point model with latitude and longitude if the string is valid, otherwise null.
        /// </returns>
        private PolygonPointModel? GetPointFromString(string pointString)
        {
            pointString = pointString.Trim();

            // Lat, Long
            var parts = pointString.Split(',');
            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
            {
                return new PolygonPointModel() { Latitude = lat, Longitude = lon };
            }

            // DMS
            var regex = new Regex(@"(\d+)°\s*(\d+)'?\s*([NSEW])", RegexOptions.IgnoreCase);
            var matches = regex.Matches(pointString);

            if (matches.Count == 2)
            {
                double ConvertMatch(Match m)
                {
                    int degrees = int.Parse(m.Groups[1].Value);
                    int minutes = int.Parse(m.Groups[2].Value);
                    string dir = m.Groups[3].Value.ToUpper();

                    double value = degrees + (minutes / 60.0);
                    if (dir == "S" || dir == "W") value = -value;
                    return value;
                }

                double latitude = ConvertMatch(matches[0]);
                double longitude = ConvertMatch(matches[1]);

                return new PolygonPointModel() { Latitude = latitude, Longitude = longitude };
            }

            return null;
        }
    }
}
