using System.Collections.Generic;
using System.Web.Mvc;

namespace Nova.Models
{
    public class BaseRequest
    {
        public string lookingFor { get; set; }
        public string lookingAtLat { get; set; }
        public string lookingAtLng { get; set; }
        public string lookingAt { get; set; }
        public string lookingBy { get; set; } = "DISCOUNT";
        public int lookingTillDistance { get; set; } = 20;
        public IList<SelectListItem> category { get; set; }
        public IList<SelectListItem> subCategory { get; set; }
        public string selectedCategory { get; set; }
        public string selectedSubCategory { get; set; }
    }
}