using System;
using System.Collections.Generic;
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

        #endregion

        public TagCollection CurrentTagCollection { set; get; }

        #region Current filters select
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
    }
}
