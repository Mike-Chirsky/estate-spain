using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Collection
    {
        public ICollection<Tag> RegionFilter
        {
            get
            {
                return GetFiltersByGroupName("region");
            }
        }

        public ICollection<Tag> CityFilter
        {
            get
            {
                return GetFiltersByGroupName("city");
            }
        }

        public ICollection<Tag> OtherTypeFilter
        {
            get
            {
                return GetFiltersByGroupName("other_type");
            }
        }

        public ICollection<Tag> ConditFilter
        {
            get
            {
                return GetFiltersByGroupName("condition");
            }
        }

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
