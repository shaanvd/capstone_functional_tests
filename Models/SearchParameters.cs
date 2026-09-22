using System.Collections.Generic;

namespace CapstoneProject.Models
{
    public class SearchParameters
    {
        public string City { get; set; } = string.Empty;
        public string SearchKeyword { get; set; } = string.Empty;
        public List<string> Filters { get; set; } = new List<string>(); 
        public string MinRating { get; set; } = string.Empty;
    }
}