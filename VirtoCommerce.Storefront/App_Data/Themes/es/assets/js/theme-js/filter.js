var ignoredKeyCodes = [9, 16, 17, 18, 19, 20, 27, 33, 34, 35, 36, 37, 38, 39, 40, 45, 91, 92, 93, 106, 107, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 144, 145, 186, 187, 190, 191, 192, 219, 220, 221];
var loadResultObj = null;
var loadLocationObj = null;
// init document
$(document).ready(function () {
    // expand addition filter if open on preview page
    if (document.cookie.indexOf("open-submenu=true") > -1)
    {
        setTimeout(function () {
            $('.adv-search-1.adv_extended_class').css('height', 'auto');
            $('.adv_extended_class .adv1-holder').css('height', 'auto');
            $('#adv_extended_options_text_adv').parent().find('.adv_extended_options_text').hide();
            $('#adv_extended_options_text_adv').parent().find('.extended_search_check_wrapper').slideDown();
            $('#adv_extended_options_text_adv').parent().find('.adv_extended_close_button').show();
        }, 100);
        
    }
    $('.js-show-filters').on('click', function () {
        var self = $(this);
        var selfBlock = self.parent();

        if (!selfBlock.hasClass('opened')) {
            selfBlock.addClass('opened');
            self.text('Скрыть фильтры');
        }
        else {
            selfBlock.removeClass('opened');
            self.text('Показать фильтры');
        }
    });

    $("#send-main-filter").click(function () {
        var locationPath = $("#location-path").val();
        var locationValue = $("#location-value").val();
        var esTypeVl = $("#estate-type-value").val();
        if ((locationPath === undefined || locationPath === "") && locationValue !== undefined && locationValue !== '') {
            var requestStr = getSearchRequest('#main-filter-controls');
            var location = window.baseUrl + getSearchPath('#main-filter-controls');
            window.location.href = location + (location.indexOf('keyword') && requestStr.length > 0 ? "&" : "") + requestStr;
        }
        else {
            var terms = getTerms('#main-filter-controls');
            var requestStr = getSeoRequest(terms);
            var location = window.baseUrl + requestStr;
            var fromFilter = false;
            if (window.location.href.split('?')[0] == location.split('?')[0]) {
                fromFilter = true;
            }
            $("#send-main-filter").addClass("loading");
            $("#send-main-filter-mobile").addClass("loading");
            
            
            $.ajax({
                url: "storefrontapi/product/filter/checkurl",
                data: JSON.stringify({ url: location, terms: terms }),
                success: function (data) {
                    if (data === true) {
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    } else if (data === false) {
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    }
                    else {
                        location = data;
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    }
                },
                error: function (err) {
                    console.error(err);
                },
                method: "POST",
                contentType: "application/json"
            });
        }
    });
    $("#send-main-filter-mobile").click(function () {
        var locationPath = $("#location-path").val();
        var locationValue = $("#location-value").val();
        var esTypeVl = $("#estate-type-value").val();
        if ((locationPath === undefined || locationPath === "") && locationValue !== undefined && locationValue !== '') {
            var requestStr = getSearchRequest('#main-filter-controls-mobile');
            var location = window.baseUrl + getSearchPath('#main-filter-controls-mobile');
            window.location.href = location + (location.indexOf('keyword') && requestStr.length > 0 ? "&" : "") + requestStr;
        }
        else {
            var terms = getTerms('#main-filter-controls');
            var requestStr = getSeoRequest(terms);
            var location = window.baseUrl + requestStr;
            var fromFilter = false;
            if (window.location.href.split('?')[0] == location.split('?')[0]) {
                fromFilter = true;
            }
            $("#send-main-filter").addClass("loading");
            $("#send-main-filter-mobile").addClass("loading");
            $.ajax({
                url: "storefrontapi/product/filter/checkurl",
                data: JSON.stringify({ url: location, terms: terms }),
                success: function (data) {
                    if (data === true) {
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    } else if (data === false) {
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    }
                    else {
                        location = data;
                        window.location.href = location + (location.indexOf('?') > -1 ? (fromFilter ? "&from_filter=1" : "") : (fromFilter ? "?from_filter=1" : ""));
                    }
                },
                error: function (err) {
                    console.error(err);
                },
                method: "POST",
                contentType: "application/json"
            });
        }
    });
    initValue();



    // filter events
    $("#main-filter-controls #location-value").keyup(function (e) {
        $("#main-filter-controls-mobile #location-path").val('');
        $("#main-filter-controls-mobile #location-path").attr('data-city', '');
        $("#main-filter-controls-mobile #location-path").attr('data-region', '');
        $("#main-filter-controls #location-path").val('');
        $("#main-filter-controls #location-path").attr('data-city', '');
        $("#main-filter-controls #location-path").attr('data-region', '');
        if (e.target.value.length < 3) {
            return;
        }
        if (ignoredKeyCodes.indexOf(e.which) === -1) {
            $("#main-filter-controls-mobile #location-value").val($("#main-filter-controls #location-value").val());
            loadSearchData("storefrontapi/location/search", $("#main-filter-controls #location-value").val(), ["#main-filter-controls .location-search", "#main-filter-controls-mobile .location-search"], ["#main-filter-controls-mobile", "#main-filter-controls"], "#main-filter-controls");
        }

    });

    $("#main-filter-controls #location-value").click(function () {
        if ($(".location-search .list").css("display") !== 'none') {
            $(".location-search .list").css("display", 'none');
        }
        else if ($(".location-search .list li").length > 0) {
            $(".location-search .list").css("display", 'block');
        }
    });
    // mobile
    $("#main-filter-controls-mobile #location-value").keyup(function (e) {
        $("#main-filter-controls-mobile #location-path").val('');
        $("#main-filter-controls-mobile #location-path").attr('data-city', '');
        $("#main-filter-controls-mobile #location-path").attr('data-region', '');
        $("#main-filter-controls #location-path").val('');
        $("#main-filter-controls #location-path").attr('data-city', '');
        $("#main-filter-controls #location-path").attr('data-region', '');
        if (e.target.value.length < 3) {
            return;
        }
        if (ignoredKeyCodes.indexOf(e.which) === -1) {
            $("#main-filter-controls #location-value").val($("#main-filter-controls-mobile #location-value").val());
            loadSearchData("storefrontapi/location/search", $("#main-filter-controls-mobile #location-value").val(), ["#main-filter-controls .location-search", "#main-filter-controls-mobile .location-search"], ["#main-filter-controls-mobile", "#main-filter-controls"], "#main-filter-controls-mobile");
        }

    });

    $("#main-filter-controls-mobile #location-value").click(function () {
        if ($(".location-search .list").css("display") !== 'none') {
            $(".location-search .list").css("display", 'none');
        }
        else if ($(".location-search .list li").length > 0) {
            $(".location-search .list").css("display", 'block');
        }
    });
    // end mobile
    $.each($("#main-filter-controls-mobile #filter-checks input"), function (index, item) {
        $(item).change(function () {
            getFoundResults("#main-filter-controls-mobile", ["#main-filter-controls", "#main-filter-controls-mobile"]);
            setRelativeCheckbox("main-filter-controls-mobile", $(item).val(), $(item).prop('checked'));
        });
    });

    $.each($("#main-filter-controls #filter-checks input"), function (index, item) {
        $(item).change(function () {
            getFoundResults("#main-filter-controls", ["#main-filter-controls", "#main-filter-controls-mobile"]);
            setRelativeCheckbox("main-filter-controls", $(item).val(), $(item).prop('checked'));
        });
    });
    // end filter events
});
// end init document
function getTerms(rootElement) {
    var terms = {};
    var element = $(rootElement + " #location-path");
    var value = getSeoName(element.attr("data-region"), "region");
    if (value != "" && value != undefined) {
        terms["region"] = value;
    }
    value = getSeoName(element.attr("data-city"), "city");
    if (value != "" && value != undefined) {
        terms["city"] = value;
    }
    element = $(rootElement + " #estate-type-value");
    value = getSeoName(element.val(), "estatetype");
    if (value != "" && value != undefined) {
        terms["estatetype"] = value;
    }
    element = $(rootElement + " #other_type");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["type"] = value;
    }
    element = $(rootElement + " #condition-value");
    value = getSeoName(element.val(), "condition");
    if (value != "" && value != undefined) {
        terms["cond"] = value;
    }
    element = $(rootElement + " #distancetosea-range");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["distosea"] = value;
    }
    element = $(rootElement + " #price-value");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["price"] = value;
    }
    element = $(rootElement + " #bed-count");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["broom"] = value;
    }
    element = $(rootElement + " #bath-count");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["bath"] = value;
    }
    element = $(rootElement + " #property-square");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["sq"] = value;
    }
    element = $(rootElement + " #land-square");
    value = element.val();
    if (value != "" && value != "*" && value != undefined) {
        terms["ls"] = value;
    }
    element = $(rootElement + " #tag-value");
    if (element.is(":checked")) {
        value = getSeoName(element.val(), "tag");
        if (value != "" && value != "*" && value != undefined) {
            terms["tag"] = value;
        }
    }
    if ($(rootElement + " #filter-checks input:checked").length > 0) {
        var sysfilterValue = "";
        $.each($(rootElement + " #filter-checks input:checked"), function (index, item) {
            if ($(item).attr('data-ignore') === undefined || $(item).attr('data-ignore') === null || $(item).attr('data-ignore') === false)
                sysfilterValue += $(item).val() + ",";
        });
        if (sysfilterValue !== "") {
            terms["more"] = sysfilterValue;
        }
    }
    return terms;
}

function getSeoRequest(terms) {
    var returnPath = "";
    var hasCityOrRegion = terms.hasOwnProperty('city') || terms.hasOwnProperty('region');
    var hasEstateType = terms.hasOwnProperty('estatetype');
    var hasCondition = terms.hasOwnProperty('cond');
    var hasOtherType = terms.hasOwnProperty('type');
    var hasTag = terms.hasOwnProperty('tag');
    // region(city) + type + condition
    if (hasCityOrRegion && hasEstateType && hasCondition) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += '/' + terms['estatetype'];
        returnPath += '/' + terms['cond'];
        returnPath += getRequstParams(['city', 'region', 'estatetype', 'cond'], terms);
    }
    // region(city) + type + tag
    else if (hasCityOrRegion && hasEstateType && hasTag) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += '/' + terms['estatetype'];
        returnPath += '/' + terms['tag'];
        returnPath += getRequstParams(['city', 'region', 'estatetype', 'tag'], terms);
    }
    // region(city) + type
    else if (hasCityOrRegion && hasEstateType) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += '/' + terms['estatetype'];
        returnPath += getRequstParams(['city', 'region', 'estatetype'], terms);
    }
    // region(city) + condition
    else if (hasCityOrRegion && hasCondition) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += '/' + terms['cond'];
        returnPath += getRequstParams(['city', 'region', 'cond'], terms);
    }
    // type + condition
    else if (hasEstateType && hasCondition) {
        returnPath = terms['estatetype'];
        returnPath += '/' + terms['cond'];
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['estatetype', 'cond'], terms);
    }
    // type + tag
    else if (hasEstateType && hasTag) {
        returnPath = terms['estatetype'];
        returnPath += '/' + terms['tag'];
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['estatetype', 'tag'], terms);
    }
    // region(city) + tag
    else if (hasCityOrRegion && hasTag) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += '/' + terms['tag'];
        returnPath += getRequstParams(['city', 'region', 'tag'], terms);
    }
    // region
    else if (hasCityOrRegion) {
        if (terms.hasOwnProperty('city')) {
            returnPath = terms['city'];
        }
        else {
            returnPath = terms['region'];
        }
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['city', 'region'], terms);
    }
    // type
    else if (hasEstateType) {
        returnPath = terms['estatetype'];
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['estatetype'], terms);
    }
    // condition
    else if (hasCondition) {
        returnPath = terms['cond'];
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['cond'], terms);
    }
    // other-type
    else if (hasOtherType) {
        returnPath = getSeoName(terms['type'], "other_type");
        if (returnPath == "") {
            returnPath = "search";
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
            returnPath += getRequstParams([], terms);
        }
        else {
            returnPath += getRequstParams(['type'], terms);
        }
    }
    else if (hasTag)
    {
        returnPath = terms['tag'];
        if (terms.hasOwnProperty('type')) {
            terms['type'] = getSeoName(terms['type'], "other_type_dict");
        }
        returnPath += getRequstParams(['tag'], terms);
    }
    return returnPath;
}

function getRequstParams(excludeProperty, terms) {
    var result = "";
    for (var prop in terms) {
        if (!terms.hasOwnProperty(prop) || excludeProperty.indexOf(prop) > -1) {
            continue;
        }
        if (result === '') {
            result = prop + "=" + terms[prop];
        }
        else {
            result += '&' + prop + "=" + terms[prop];
        }
    }
    return result == "" ? result : ("?" + result);
}

function getSearchRequest(rootElement)
{
    var terms = getTerms(rootElement);
    var term = "";
    for (var item in terms) {
        if (!terms.hasOwnProperty(item))
            continue;
        if (item == "type")
        {
            terms["type"] = getSeoName(terms["type"], "other_type_dict");
        }
        if (term == "") {
            term += item + "=" + terms[item];
        }
        else {
            term += "&" + item + "=" + terms[item];
        }
    }
    return term;
}

function getFoundResults(rootElement, fillElements) {
    var terms = getTerms(rootElement);
    if (terms.hasOwnProperty('type'))
    {
        terms['type'] = getSeoName(terms['type'], "other_type_dict");
    }
    loadFoundResult(terms, function (data) {
        var elementSetTodefult = false;
        fillElements.forEach(function (element) {
            
            if (fillElement(element + " #estate-type-value", data.aggregations, "estatetype") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #other_type", data.aggregations, "other_type") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #condition-value", data.aggregations, "condition") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #bed-count", data.aggregations, "bedrooms", "Спален") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #bath-count", data.aggregations, "bath", "Санузлов") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #property-square", data.aggregations, "propertysquare", "Метраж (м<sup>2</sup>)") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (fillElement(element + " #land-square", data.aggregations, "landsquare", "Участок (м<sup>2</sup>)") && !elementSetTodefult) {
                elementSetTodefult = true;
            }
            if (!elementSetTodefult) {
                $(element + " #no-filter").hide();
                $(element + " #count-objects").show();
                $(element + " #result-count").text(numEnding(data.totalItemCount));
            }
        });
        if (elementSetTodefult) {
            getFoundResultWithoutFill(rootElement, fillElements);
        }
    });
}
function getFoundResultWithoutFill(rootElement, fillElements) {
    var terms = getTerms(rootElement);
    if (terms.hasOwnProperty('type')) {
        terms['type'] = getSeoName(terms['type'], "other_type_dict");
    }
    loadFoundResult(terms, function (data) {
        fillElements.forEach(function (element) {
            $(element + " #no-filter").hide();
            $(element + " #count-objects").show();
            $(element + " #result-count").text(numEnding(data.totalItemCount));
        });
    });
}
function loadFoundResult(terms, callback) {
    if (loadResultObj != null && loadResultObj.readyState != 4) {
        loadResultObj.abort();
    }
    $("#send-main-filter").addClass("loading");
    $("#send-main-filter-mobile").addClass("loading");
    loadResultObj = $.ajax({
    	url: "storefrontapi/product/totalcount",
        data: JSON.stringify(terms),
        success: function (data) {
            $("#send-main-filter").removeClass("loading");
            $("#send-main-filter-mobile").removeClass("loading");
            callback(data)
        },
        method: "POST",
        contentType: "application/json"
    });
}

function numEnding(number) {

    if (/1\\d$/.test(number))
        return number + " объектов";
    if (/1$/.test(number))
        return number + " объект";
    if (/(2|3|4)$/.test(number))
        return number + " объекта";
    return number + " объектов";
}

function fillElement(id, aggregations, field, addText) {
    // remove old values
    $(id).parent().find("ul.dropdown-menu li:not(:first)").remove();
    var currentValue = $(id).val();
    var list = $(id).parent().find("ul.dropdown-menu");
    var types = getValues(aggregations, field);
    var setToDefault = true;
    types.forEach(function (item) {
        if (item.value === currentValue || currentValue === "*" || currentValue === "" || currentValue === undefined) {
            setToDefault = false;
        }
        if (seoLinks && seoLinks[field]) {
            var seoPath = "";
            seoLinks[field].forEach(function (seo) {
                if (seo.value === item.value) {
                    seoPath = seo.seoLink;
                }
            });
            list.append('<li data-value="' + item.value + '" data-seo-path="' + seoPath + '"' + (addText ? 'display-header="' + addText + '"' : '') + '>' + item.value + '</li>');
        }
        else {
            list.append('<li data-value="' + item.value + '"' + (addText ? 'display-header="' + addText + '"' : '') + '>' + item.value + '</li>');
        }
    });
    if ((currentValue === "" || currentValue === undefined) && setToDefault) {
        setToDefault = false;
    }
    /* uncommet for set default value element
    if (setToDefault) {

        setDdValue($(id).parent().find("ul.dropdown-menu li:first"), false, false, true);
    }*/
    setClickDdElement(".search_wrapper");
    return setToDefault;
}

function getValues(aggregations, type) {
    var result = [];
    aggregations.forEach(function (agg) {
        if (agg.field === type) {
            result = agg.items;
        }
    });
    result.sort(function (a, b) {
        if (!isNaN(parseInt(a.value))) {
            return parseInt(a.value) - parseInt(b.value);
        }
        return 0;
    });
    return result;
}

function setClickDdElement(elem) {
    $.each($(elem + ' li'), function (index, el) {
        if ($(el).parent().hasClass("location")) {
            return;
        }
        $(el).off('click');
        $(el).click(function (event) {
            if (event) {
                event.preventDefault();
            }
            setDdValue(this, true, true);
        });
    });
}

function setDdValue(element, changeNotif, loadResult, setDefaultValue) {
    var pick, value, parent, parent_replace;
    var setSelect = true;
    parent_replace = '.filter_menu_trigger';
    if (setDefaultValue) {
        pick = $(element).parent().attr("data-default-value");
        setSelect = false;
    }
    else if ($(element).attr('data-value') == "*") {
        pick = $(element).attr('value');
    }
    else if ($(element).attr('display-header')) {
        pick = $(element).attr('display-header') + ' : <span class="dd-select-value">' + $(element).text() + '</span>';
    }
    else if ($(element).attr('formated-value')) {
        pick = $(element).attr('formated-value');
    }
    else {
        pick = $(element).text();
    }

    value = $(element).attr('data-value');
    parent = $(element).parent().parent();
    if ($(element).attr('data-seo-path')) {
        parent.find("input").attr('data-seo-path', $(element).attr('data-seo-path'));
    }
    // set select
    if (setSelect) {
        parent.addClass("selected");
    } else {
        parent.removeClass("selected");
    }
    parent.find(parent_replace).html(pick).append('<span class="caret caret_filter"></span>').attr('data-value', value);

    if (changeNotif) {
        parent.find('input').val(value).trigger('change');
    }
    else {
        parent.find('input').val(value);
    }
    //$(element).parent().hide();

    if (loadResult) {
        var rootElemet = '';
        var el = $(element);
        while (true) {
            var el = $(el.parent());
            if (el[0].id != '' && (el[0].id === 'main-filter-controls' || el[0].id === 'main-filter-controls-mobile')) {
                rootElemet = el[0].id;
                break;
            }
        }
        setRelativeValue(rootElemet, parent.find('input').attr('property-name'), value);
        getFoundResults('#' + rootElemet, ["#main-filter-controls", "#main-filter-controls-mobile"]);
    }
}
// set relative element value for other filter
function setRelativeValue(rootElement, propertyName, value) {
    var prefix = '';
    if (rootElement === 'main-filter-controls') {
        prefix = '-mobile';
    }
    var el = $("#main-filter-controls" + prefix + " input[property-name='" + propertyName + "']").parent();
    $.each(el.children("ul").children("li"), function (index, item) {
        var element = $(item);
        if (element.attr('data-value') === value) {
            setDdValue(item, false, false);
        }
    });
}

function setRelativeCheckbox(rootElement, value, isSet) {
    var prefix = '';
    if (rootElement === 'main-filter-controls') {
        prefix = '-mobile';
    }
    var el = $("#main-filter-controls" + prefix + " input[value='" + value + "'][type=checkbox]");
    el.prop("checked", isSet);
}


function loadSearchData(url, search, elements, listElement, rootElement) {
    if (!search || !url) {
        return;
    }
    if (loadLocationObj != null && loadLocationObj.readyState != 4) {
        loadLocationObj.abort();
    }
    if (elements.length > 0) {
        elements.forEach(function (element) {
            $(element + " input[type=text]").removeClass("selected");
        });
        $.ajax({
            url: url,
            data: JSON.stringify({ search: search }),
            method: "POST",
            success: function (data) {
                elements.forEach(function (element) {
                    if (data.items && data.items.length) {
                        $(element + " .list").html('');
                        for (var i = 0; i < data.items.length; i++) {
                            var item = data.items[i];
                            var infoRegion = "";
                            var infoCity = "";
                            if (item.regionName) {
                                infoRegion = 'data-region="' + item.regionName + '"';
                            }
                            if (item.cityName) {
                                infoCity = 'data-city="' + item.cityName + '"';
                            }
                            $(element + " .list").append('<li data-seo-path="' + item.seo + '" ' + infoRegion + ' ' + infoCity + ' data-value="' + item.fullName + '">' + item.fullName + '</li>')
                        }
                        $.each($(element + " .list li"), function (index, item) {
                            $(item).click(function () {
                                selectSearchItem(item, elements, listElement, rootElement);
                            });
                        });
                        $(element + " .list").css("display", "block");
                    }
                    else {
                        $(element + " .list").html('');
                        $(element + " .list").hide();
                    }
                });

            },
            contentType: "application/json"
        });

    }

}

function selectSearchItem(item, elements, listElements, rootElement) {
    elements.forEach(function (element) {
        $(element + " .list").css("display", "none");
        $(element + " #location-value").val($(item).attr("data-value")).change();
        $(element + " #location-value").addClass("selected");
        $(element + " #location-path").val($(item).attr("data-seo-path"));
        var region = $(item).attr("data-region");
        if (region) {
            $(element + " #location-path").attr("data-region", region);
        }
        else {
            $(element + " #location-path").attr("data-region", "");
        }
        var city = $(item).attr("data-city");
        if (city) {
            $(element + " #location-path").attr("data-city", city);
        }
        else {
            $(element + " #location-path").attr("data-city", "");
        }
        
    });

    getFoundResults(rootElement, listElements);
}


function getRequestSeoPath(rootElement) {
    if (rootElement === undefined) {
        rootElement = "";
    }
    var locationPath = $(rootElement + ' #location-path').val();
    if (locationPath === undefined) {
        locationPath = "";
    }
    var typePath = $(rootElement + ' #estate-type-value').attr('data-seo-path');
    if (typePath === undefined) {
        typePath = "";
    }
    if (locationPath !== "" && typePath !== "") {
        return locationPath + "/" + typePath;
    }
    else if (locationPath !== "") {
        return locationPath;
    }
    else if (typePath !== "") {
        return typePath;
    }
    return "";
}

function getSearchPath(rootElement) {
    if (rootElement === undefined) {
        rootElement = "";
    }
    var locationValue = $(rootElement + " #location-value").val();
    if (locationValue === undefined) {
        locationValue = "";
    }
    if (locationValue === "") {
        return "search?"
    }
    else {
        return "search?search=" + locationValue;
    }
}

function initValue() {
    if (!initFilterValueRange)
    {
        return;
    }
    var el = $("#main-filter-controls .price-cmb ul");
    setCurrentValueDd(el, initFilterValueRange.price);
    el = $("#main-filter-controls-mobile .price-cmb ul");
    setCurrentValueDd(el, initFilterValueRange.price);
    el = $("#main-filter-controls-mobile .distance-to-sea ul");
    setCurrentValueDd(el, initFilterValueRange.distanceToSea);
    el = $("#main-filter-controls .distance-to-sea ul");
    setCurrentValueDd(el, initFilterValueRange.distanceToSea);
}

function setCurrentValueDd(elements, value) {
    $.each(elements, function (index, element) {
        var els = $(element).children("li");
        $.each(els, function (index, item) {
            var elItem = $(item);
            if (elItem.attr("data-value") === value) {
                setDdValue(elItem, false, false);
            }
        });
    });
}

function getSeoName(value, key) {
    var array = seoLinks[key];
    if (array === undefined) {
        return "";
    }
    for (var i = 0; i < array.length; i++) {
        if (array[i].value === value) {
            return array[i].seoLink;
        }
    }
    return "";
}