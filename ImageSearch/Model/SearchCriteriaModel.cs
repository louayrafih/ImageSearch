namespace ImageSearch.Model
{
    internal class SearchCriteriaModel
    {
        public required string Property { get; set; }
        public required string Operator { get; set; }
        public required string Value { get; set; }

        public bool Valid =>
            !string.IsNullOrWhiteSpace(Property) &&
            !string.IsNullOrWhiteSpace(Operator) &&
            !string.IsNullOrWhiteSpace(Value);
    }
}
