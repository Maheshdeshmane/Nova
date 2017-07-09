using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nova.Models
{
    public class DiscountDetailsModel
    {
        public List<string> ShopOfferImages { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopContact { get; set; }
        public string ShopOffer { get; set; }
        public string ShopOfferValidDate { get; set; }
        public string ShopGoogleMapAddress { get; set; }
        public string ShopOnlineAddress { get; set; }
    }
}