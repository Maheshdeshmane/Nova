using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nova.Models.OnlineShopModels
{
    public class OnlineShopDiscount
    {
        public string keyword { get; set; }
        public string discount { get; set; }
        public string offer { get; set; }
        public string validFrom { get; set; }
        public string validTill { get; set; }
        public string AdditionalDetails { get; set; }

    }
}