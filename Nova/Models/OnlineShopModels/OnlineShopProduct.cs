using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nova.Models.OnlineShopModels
{
    public class OnlineShopProduct
    {
        public string FolderName { get; set; }
        public string ProductImageName { get; set; }
        public string ProductDesc { get; set; }
        public string ProductPrice { get; set; }
        public string ProductName { get; set; }
        public int ProductStar { get; set; }

    }
}