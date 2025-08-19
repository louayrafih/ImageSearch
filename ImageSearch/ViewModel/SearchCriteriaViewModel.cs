using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageSearch.Model;
using ImageSearch.Services;
using System.Collections.ObjectModel;

namespace ImageSearch.ViewModel
{
    internal partial class SearchCriteriaViewModel : ObservableObject
    {
        private const string EQUALS = "=";
        private const string DOESNOTEQUAL = "<>";
        private const string GREATERTHAN = ">";
        private const string LESSTHAN = "<";

        [ObservableProperty]
        private ObservableCollection<SearchCriteriaModel> searchCriteria = new ObservableCollection<SearchCriteriaModel>();
        [ObservableProperty]
        private ObservableCollection<PolygonPointModel> polygonPoints = new ObservableCollection<PolygonPointModel>();

        [ObservableProperty]
        private ObservableCollection<string> availableProperties;

        [ObservableProperty]
        private ObservableCollection<string> availableOperators;

        private IGeolocationService geoLocationService;
        /// <summary>
        /// VM for Searching Images based on criteria.
        /// </summary>
        /// <param name="locService">Geo location service for GeolocationView</param>
        public SearchCriteriaViewModel(IGeolocationService locService)
        {
            geoLocationService = locService;
            //In a commercial setting this would pull from a service that would be able to provide the operators it supports.
            AvailableOperators = new ObservableCollection<string>()
            {
                EQUALS, DOESNOTEQUAL, GREATERTHAN, LESSTHAN
            };
        }

        [RelayCommand]
        private void AddCriteria()
        {
            SearchCriteria.Add(new SearchCriteriaModel
            {
                Property = AvailableProperties.FirstOrDefault(),
                Operator = AvailableOperators.FirstOrDefault(),
                Value = string.Empty
            });
        }

        [RelayCommand]
        private void RemoveCriteria(SearchCriteriaModel criteria)
        {
            SearchCriteria.Remove(criteria);
        }


        [RelayCommand]
        private void AddPoint()
        {
            PolygonPoints.Add(new PolygonPointModel());
        }

        [RelayCommand]
        private void RemoveLastPoint()
        {
            if (PolygonPoints.Any())
                PolygonPoints.Remove(PolygonPoints.Last());
        }




        // In a commercial environment, these checks would be operating in parallel and once one fails, all the others would end their check using a CancellationToken.
        // I would also consider a separate service for searching for tags.\
        // Assumption that each metadata property is unique except for tags, which can have multiple values.
        /// <summary>
        /// Checks if the image matches the search criteria defined.
        /// </summary>
        /// <param name="img">image to check criteria</param>
        /// <returns>
        /// boolean indicating if the image matches the criteria.
        /// </returns>
        public bool MatchesCriterion(ImageViewModel img)
        {
            //Geo Search - Check for any input and if the img fails the boundary check, then return false immediately.
            if (PolygonPoints.Any() && ((!geoLocationService?.IsWithinPolygon(PolygonPoints, img.Image)) ?? false))
                return false;



            //A failure on a criterion will return false and skip the remaining criteria.
            foreach (var criterion in SearchCriteria)
            {
                if (!criterion.Valid) continue; //invalid criterion will be ignored.

                var propValues = img.PropertyLookup[criterion.Property]; // Try to get the value from base properties, tags or metadata

                if (!propValues.Any())
                    return false;

                if (criterion.Property == "Tag") //Tag Logic, would be implemented better without string literals
                {
                    if (!CheckList(criterion, propValues)) return false;
                }
                else
                {
                    var propValue = propValues.FirstOrDefault(); // Take the first value  
                                                                 // numeric comparison first, double covers int and decimal
                    if (double.TryParse(criterion.Value, out var valNum) && double.TryParse(propValue, out var cmpNum))
                    {
                        var compareResult = cmpNum.CompareTo(valNum);
                        if (!CheckDouble(criterion, cmpNum, valNum))
                            return false;
                    }
                    //string comparison
                    else if (!(CheckString(criterion, propValue)))
                        return false;
                }
            }

            return true;

        }

        private bool CheckString(SearchCriteriaModel criterion, string propValue)
        {
            return criterion.Operator switch
            {
                EQUALS => propValue.ToString().ToLowerInvariant() == criterion.Value.ToLowerInvariant(),
                DOESNOTEQUAL => propValue.ToString().ToLowerInvariant() != criterion.Value.ToLowerInvariant(),
                _ => false // only = and <>
            };
        }

        private bool CheckDouble(SearchCriteriaModel criterion, double cmpNum, double propNum)
        {
            var compareResult = cmpNum.CompareTo(propNum);
            return criterion.Operator switch
            {
                EQUALS => compareResult == 0,
                DOESNOTEQUAL => compareResult != 0,
                GREATERTHAN => compareResult > 0,
                LESSTHAN => compareResult < 0,
                _ => false
            };
        }

        private bool CheckList(SearchCriteriaModel criterion, IEnumerable<string> propValues)
        {
            if (criterion.Operator == EQUALS)
            {
                return propValues.Any(propValue => CheckString(criterion, propValue));
            }
            else if (criterion.Operator == DOESNOTEQUAL)
            {
                return propValues.All(propValue => CheckString(criterion, propValue));
            }

            return false;
        }
    }
}