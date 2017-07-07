using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Es.Search
{
    public class LocationSearchItem
    {
        public string CityName { set; get; }
        public string RegionName { set; get; }
        public string DistrictName { set; get; }
        public string CitySeo { set; get; }
        public string RegionSeo { set; get; }
        public string DistrictSeo { set; get; }
        /// <summary>
        /// Parent region name for region
        /// </summary>
        public string ParentRegionName { set; get; }
        public string FullSeo
        {
            get
            {
                var seo = string.Empty;
                if (!string.IsNullOrEmpty(RegionSeo))
                {
                    seo = RegionSeo;
                }
                if (!string.IsNullOrEmpty(CitySeo))
                {
                    if (!string.IsNullOrEmpty(RegionSeo))
                    {
                        seo += "/";
                    }
                    seo += CitySeo;
                }
                if (!string.IsNullOrEmpty(DistrictSeo))
                {
                    if (!string.IsNullOrEmpty(RegionSeo) || !string.IsNullOrEmpty(RegionSeo))
                    {
                        seo += "/";
                    }
                    seo += DistrictSeo;
                }
                return seo;
            }
        }

        public string Seo {
            get {
                if(!string.IsNullOrEmpty(DistrictSeo))
                {
                    return  $"{CitySeo}/{DistrictSeo}";
                }
                if (!string.IsNullOrEmpty(CitySeo))
                {
                    return CitySeo;
                }
                if (!string.IsNullOrEmpty(RegionSeo))
                {
                    return RegionSeo;
                }
                return string.Empty;
            }
        }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(DistrictName))
                {
                    return DistrictName;
                }
                if (!string.IsNullOrEmpty(CityName))
                {
                    return CityName;
                }
                if (!string.IsNullOrEmpty(RegionName))
                {
                    return RegionName;
                }
                return string.Empty;
            }
        }

        public string FullName
        {
            get
            {
                var name = string.Empty;
                if (!string.IsNullOrEmpty(CityName))
                {
                    name = CityName;
                }
                if (!string.IsNullOrEmpty(RegionName))
                {
                    if (!string.IsNullOrEmpty(CityName))
                    {
                        name += ", ";
                    }
                    name += RegionName;
                    if (!string.IsNullOrEmpty(ParentRegionName))
                    {
                        name += ", " + ParentRegionName;
                    }
                }
                
                return name;
            }
        }
    }
}
