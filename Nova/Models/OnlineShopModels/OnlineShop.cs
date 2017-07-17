using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nova.Models.OnlineShopModels
{
    public class OnlineShop
    {
        public OnlineShopHome Home { get; set; }
        public List<OnlineShopProduct> Products { get; set; }
        public List<OnlineShopTeamMember> TeamMembers { get; set; }
        public List<string> ShopCategory { get; set; }
        public List<OnlineShopDiscount> Discounts { get; set; }
        public OnlineShopAwards Awards { get; set; }
        public List<OnlineShopTestimonials> Testimonials { get; set; }
    }
}   