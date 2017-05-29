﻿using System.Linq;
using Newtonsoft.Json.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Converters
{
    public static class DynamicPropertyConverter
    {
        public static DynamicProperty ToDynamicProperty(this coreDto.DynamicObjectProperty propertyDto)
        {
            var result = new DynamicProperty();

            result.InjectFrom<NullableAndEnumValueInjecter>(propertyDto);

            if (propertyDto.DisplayNames != null)
            {
                result.DisplayNames = propertyDto.DisplayNames.Select(x => new LocalizedString(new Language(x.Locale), x.Name)).ToList();
            }

            if (propertyDto.Values != null)
            {
                if (result.IsDictionary)
                {
                    var dictValues = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.Value)
                        .Cast<JObject>()
                        .Select(x => x.ToObject<platformDto.DynamicPropertyDictionaryItem>())
                        .ToArray();

                    result.DictionaryValues = dictValues.Select(x => x.ToDictItem()).ToList();
                }
                else
                {
                    result.Values = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.ToLocalizedString())
                        .ToList();
                }
            }

            return result;
        }


        public static coreDto.DynamicObjectProperty ToDynamicPropertyDto(this DynamicProperty dynamicProperty)
        {
            var result = new coreDto.DynamicObjectProperty();

            result.InjectFrom<NullableAndEnumValueInjecter>(dynamicProperty);

            if (dynamicProperty.Values != null)
            {
                result.Values = dynamicProperty.Values.Select(v => v.ToPropertyValueDto()).ToList();
            }
            else if (dynamicProperty.DictionaryValues != null)
            {
                result.Values = dynamicProperty.DictionaryValues.Select(x => x.ToPropertyValueDto()).ToList();
            }

            return result;
        }

        private static DynamicPropertyDictionaryItem ToDictItem(this platformDto.DynamicPropertyDictionaryItem dto)
        {
            var result = new DynamicPropertyDictionaryItem();
            result.InjectFrom<NullableAndEnumValueInjecter>(dto);
            if (dto.DisplayNames != null)
            {
                result.DisplayNames = dto.DisplayNames.Select(x => new LocalizedString(new Language(x.Locale), x.Name)).ToList();
            }
            return result;
        }

        private static LocalizedString ToLocalizedString(this coreDto.DynamicPropertyObjectValue dto)
        {
            return new LocalizedString(new Language(dto.Locale), dto.Value.ToString());
        }

        private static coreDto.DynamicPropertyObjectValue ToPropertyValueDto(this DynamicPropertyDictionaryItem dictItem)
        {
            var result = new coreDto.DynamicPropertyObjectValue { Value = dictItem };
            return result;
        }

        private static coreDto.DynamicPropertyObjectValue ToPropertyValueDto(this LocalizedString dynamicPropertyObjectValue)
        {
            var result = new coreDto.DynamicPropertyObjectValue
            {
                Value = dynamicPropertyObjectValue.Value,
                Locale = dynamicPropertyObjectValue.Language.CultureName
            };

            return result;
        }
    }
}
