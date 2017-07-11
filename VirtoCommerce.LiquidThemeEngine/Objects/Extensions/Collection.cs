﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Collection
    {

        #region constants
        private const string RegionKey = "region";
        private const string CityKey = "city";
        private const string OtherTypeKey = "other_type";
        private const string ConditionKey = "condition";
        private const string SeaDistanceKey = "distancetosea";
        private const string PriceRangeKey = "price";
        private const string EstateTypeKey = "estatetype";
        private const string BedRoomsKey = "bedrooms";
        private const string BathRoomsKey = "bath";
        private const string LandSquareKey = "landsquare";
        private const string PropertySquareKey = "propertysquare";
        private const string SysFilterKey = "sys_filter";


        #endregion

        public TagCollection CurrentTagCollection { set; get; }

        #region Images
        public string HeaderImage
        {
            get
            {
                return GetImageSrcByType("Header");
            }
        }
        public string LinkBlockType1Image
        {
            get
            {
                return GetImageSrcByType("linkblock-type1");
            }
        }
        public string LinkBlockType2Image
        {
            get
            {
                return GetImageSrcByType("linkblock-type2");
            }
        }
        public string LinkBlockType3Image
        {
            get
            {
                return GetImageSrcByType("linkblock-type3");
            }
        }

        public string LinkBlockRightImage
        {
            get
            {
                return GetImageSrcByType("linkblock-right");
            }
        }

        public string LinkBlockLeftImage
        {
            get
            {
                return GetImageSrcByType("linkblock-left");
            }
        }
        public string LinkBlockCentrImage
        {
            get
            {
                return GetImageSrcByType("linkblock-centr");
            }
        }

        public string LinkBlockCentr1Image
        {
            get
            {
                return GetImageSrcByType("linkblock-centr1");
            }
        }
        #endregion

        #region Info property
        public string H1
        {
            get
            {
                return GetPropertyByName("h1")?.Value;
            }
        }
        public string H11
        {
            get
            {
                return GetPropertyByName("h1-1")?.Value;
            }
        }
        public string H2Listing
        {
            get
            {
                return GetPropertyByName("h2-listing")?.Value;
            }
        }
        public string H2Tip
        {
            get
            {
                return GetPropertyByName("h2-tip")?.Value;
            }
        }
        public string H21
        {
            get
            {
                return GetPropertyByName("h2-1")?.Value;
            }
        }
        public string H3SeotextDown1
        {
            get
            {
                return GetPropertyByName("h3-seotext-down1")?.Value;
            }
        }
        public string H3SeotextDown2
        {
            get
            {
                return GetPropertyByName("h3-seotext-down2")?.Value;
            }
        }
        public string H3SeotextDown3
        {
            get
            {
                return GetPropertyByName("h3-seotext-down3")?.Value;
            }
        }
        public string SeotextUp
        {
            get
            {
                return GetDescriptionByType("seotext-up");
            }
        }

        public string SeotextDown1
        {
            get
            {
                return GetDescriptionByType("seotext-down1");
            }
        }

        public string SeotextDown2
        {
            get
            {
                return GetDescriptionByType("seotext-down2");
            }
        }

        public string SeotextDown3
        {
            get
            {
                return GetDescriptionByType("seotext-down3");
            }
        }

        public string LinkBlockType1
        {
            get
            {
                return GetDescriptionByType("linkblock-type1");
            }
        }
        public string LinkBlockType2
        {
            get
            {
                return GetDescriptionByType("linkblock-type2");
            }
        }
        public string LinkBlockType3
        {
            get
            {
                return GetDescriptionByType("linkblock-type3");
            }
        }

        public string LinkBlockRight
        {
            get
            {
                return GetDescriptionByType("linkblock-right");
            }
        }

        public string LinkBlockLeft
        {
            get
            {
                return GetDescriptionByType("linkblock-left");
            }
        }
        public string LinkBlockCentr
        {
            get
            {
                return GetDescriptionByType("linkblock-centr");
            }
        }

        public string LinkBlockCentr1
        {
            get
            {
                return GetDescriptionByType("linkblock-centr1");
            }
        }

        public string InfoBlockListing1
        {
            get
            {
                return GetDescriptionByType("linkblock-listing1");
            }
        }

        public string InfoBlockListing2
        {
            get
            {
                return GetDescriptionByType("linkblock-listing2");
            }
        }

        public string InfoBlockListing3
        {
            get
            {
                return GetDescriptionByType("linkblock-listing3");
            }
        }

        #endregion

        #region Current filters select

        public string CurrentSysFilterValue
        {
            get
            {
                return string.Join(",", CurrentTagCollection.Where(x => x.GroupName.Equals(SysFilterKey, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Value));
            }
        }

        public Tag CurrentPropertySquareFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(PropertySquareKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentLandSquareFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(LandSquareKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentBathRoomsFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(BathRoomsKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentBedRoomsFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(BedRoomsKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentEstateTypeFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(EstateTypeKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentRegionFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(RegionKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentCityFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(CityKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentOtherTypeFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(OtherTypeKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public Tag CurrentConditFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(ConditionKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentSaeDistanceFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(SeaDistanceKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Tag CurrentPriceRangeFilter
        {
            get
            {
                return CurrentTagCollection.FirstOrDefault(x => x.GroupName.Equals(PriceRangeKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        #endregion


        #region Filter values

        public ICollection<Tag> PropertySquareFilter
        {
            get
            {
                return GetFiltersByGroupName(PropertySquareKey)?.OrderBy(x => x.Value).ToList();
            }
        }

        public ICollection<Tag> LandSquareFilter
        {
            get
            {
                return GetFiltersByGroupName(LandSquareKey)?.OrderBy(x => x.Value).ToList();
            }
        }

        public ICollection<Tag> BathRoomsFilter
        {
            get
            {
                return GetFiltersByGroupName(BathRoomsKey);
            }
        }

        public ICollection<Tag> BedRoomsFilter
        {
            get
            {
                return GetFiltersByGroupName(BedRoomsKey);
            }
        }

        public ICollection<Tag> EstateTypeFilter
        {
            get
            {
                return GetFiltersByGroupName(EstateTypeKey);
            }
        }

        public ICollection<Tag> RegionFilter
        {
            get
            {
                return GetFiltersByGroupName(RegionKey);
            }
        }

        public ICollection<Tag> CityFilter
        {
            get
            {
                return GetFiltersByGroupName(CityKey);
            }
        }

        public ICollection<Tag> OtherTypeFilter
        {
            get
            {
                return GetFiltersByGroupName(OtherTypeKey);
            }
        }

        public ICollection<Tag> ConditFilter
        {
            get
            {
                return GetFiltersByGroupName(ConditionKey);
            }
        }

        #endregion

        /// <summary>
        /// Get list filters by group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private ICollection<Tag> GetFiltersByGroupName(string groupName)
        {
            if (AllTags.Count == 0)
            {
                return new List<Tag>();
            }
            return AllTags.Where(x => x.GroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Get description content by description type
        /// </summary>
        private string GetDescriptionByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return string.Empty;
            }
            return Descriptions.FirstOrDefault(x => type.Equals(x.Type, StringComparison.InvariantCultureIgnoreCase))?.Content;
        }

        private string GetImageSrcByType(string type)
        {
            var img = Images.FirstOrDefault(x => Path.GetFileName(x.Src)?.StartsWith(type, StringComparison.InvariantCultureIgnoreCase) ?? false);
            return img != null ? img.Src : "http://wpresidence.net/wp-content/uploads/2015/11/file112128225584523-980x777.jpg";
        }

        /// <summary>
        /// Get property by name
        /// </summary>
        private ProductProperty GetPropertyByName(string propertyName)
        {
            return Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public ICollection<ProductProperty> Properties { get; set; }

        public Descriptions Descriptions { get; set; }
        public string FullName { get; internal set; }

        public string Type { set; get; }
        public string ProductType { get; set; }

        public string CityUrl { set; get; }
        public string RegionUrl { set; get; }
        public string CityName { set; get; }
        public string RegionName { set; get; }
    }
}
