var ignoredKeyCodes = [9, 16, 17, 18, 19, 20, 27, 33, 34, 35, 36, 37, 38, 39, 40, 45, 91, 92, 93, 106, 107, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 144, 145, 186, 187, 190, 191, 192, 219, 220, 221];
function slideToBlock(selector, offest) {
    if (offest === undefined)
    {
        offest = 200;
    }
    var el = jQuery(selector);
    if (!el || !el.offset())
    {
        return false;
    }
    $('html, body').animate({ scrollTop: el.offset().top - offest }, 500);
    return false;
}

function setCurrentValueDd(items, value) {
    jQuery.each(items, function (index, item) {
        if (jQuery(item).attr('data-value') === value) {
            jQuery(item).click();
        }
    });
}
function getTerms(rootElement) {
    var terms = [];
    var key = rootElement + " input";
    $.each(jQuery(key), function (index, item) {
        if (jQuery(item).val() && jQuery(item).val() != "*" && jQuery(item).attr("data-ignore") === undefined) {
            var propAttr = jQuery(item).attr("property-name");
            if (propAttr && propAttr != '') {
                terms.push({
                    name: propAttr, value: jQuery(item).val()
                });
            }
        }
    });

    // filter by location
    var regionElementData = jQuery(rootElement + " #location-path");
    if (regionElementData.val()) {

        if (regionElementData.attr("data-region") && regionElementData.attr("data-region") !== "") {
            terms.push({ name: "region", value: regionElementData.attr("data-region") });
        }
        if (regionElementData.attr("data-city") && regionElementData.attr("data-city") !== "") {
            terms.push({ name: "city", value: regionElementData.attr("data-city") });
        }
    }

    if ($(rootElement + " #estate-type-value").val() && $(rootElement + " #estate-type-value").val()!= "*") {
        terms.push({ name: "estatetype", value: $(rootElement + " #estate-type-value").val() });
    }

    if (jQuery(rootElement + " #filter-checks input:checked").length > 0) {
        var sysfilterItem = { name: "sys_filter:", value: "" };
        $.each(jQuery(rootElement + " #filter-checks input:checked"), function (index, item) {
            sysfilterItem.value += jQuery(item).val() + ",";
        });
        terms.push(sysfilterItem);
    }
    return terms;
}

function getTermsString(rootElement) {
    var queryString = "";
    var key = rootElement + " input";
    $.each(jQuery(key), function (index, item) {
        if (jQuery(item).val() && jQuery(item).val() != "*" && jQuery(item).attr("data-ignore") === undefined) {
            var propAttr = jQuery(item).attr("property-name");
            if (propAttr && propAttr != '') {
                queryString += jQuery(item).attr("property-name") + ":" + jQuery(item).val() + ";";
            }
        }
    });

    if (jQuery(rootElement + " #filter-checks input:checked").length > 0) {
        queryString += "sys_filter:";
        $.each(jQuery(rootElement + " #filter-checks input:checked"), function (index, item) {
            queryString += jQuery(item).val() + ",";
        });
    }
    if (queryString.length > 0) {
        return queryString.substr(0, queryString.length - 1);
    }
    return "";
}

function getFoundResults(rootElement, fillElements) {
    var terms = getTerms(rootElement);
    if (terms.length === 0) {
        return;
    }
    
    jQuery.ajax({
        url: "storefrontapi/product/filter",
        data: JSON.stringify({ mutableTerms: terms, responseGroup: 'ItemInfo' }),
        success: function (data) {
            fillElements.forEach(function (element) {
                $(element + " #no-filter").hide();
                $(element + " #count-objects").show();
                $(element + " #result-count").text(data.metaData.totalItemCount);
                fillElement(element + " #estate-type-value", data.aggregations, "estatetype");
                fillElement(element + " #other_type", data.aggregations, "other_type");
                fillElement(element + " #condition-value", data.aggregations, "condition");
            });
        },
        method: "POST",
        contentType: "application/json"
    });
}

function fillElement(id, aggregations, field) {
    // remove old values
    $(id).parent().find("ul.dropdown-menu li:not(:first)").remove();
    var currentValue = $(id).val();
    var list = $(id).parent().find("ul.dropdown-menu");
    var types = getValues(aggregations, field);
    var setToDefault = true;
    types.forEach(function (item) {
        if (item.value === currentValue) {
            setToDefault = false;
        }
        if (seoLinks && seoLinks[field]) {
            var seoPath = "";
            seoLinks[field].forEach(function (seo) {
                if (seo.value === item.value) {
                    seoPath = seo.seoLink;
                }
            });
            list.append('<li data-value="' + item.value + '" data-seo-path="' + seoPath + '">' + item.value + '</li>');
        }
        else {
            list.append('<li data-value="' + item.value + '">' + item.value + '</li>');
        }
    });
    if (setToDefault) {
        setDdValue($(id).parent().find("ul.dropdown-menu li:first"), false, false);
    }
    setClickDdElement(".search_wrapper");
}

function getValues(aggregations, type)
{
    var result = [];
    aggregations.forEach(function (agg) {
        if (agg.field === type)
        {
            result = agg.items;
        }
    });
    return result;
}

function setClickDdElement(elem)
{
    $.each(jQuery(elem + ' li'), function (index, el) {
        jQuery(el).off('click');
        jQuery(el).click(function () {
            event.preventDefault();
            setDdValue(this, true, true);
        });
    });
}

function setDdValue(element, changeNotif, loadResult)
{
    var pick, value, parent, parent_replace;

    parent_replace = '.filter_menu_trigger';
    /*if (elem === '.advanced_search_sidebar') {
        parent_replace = '.sidebar_filter_menu';
    }*/

    if ($(element).attr('data-value') == "*") {
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
    if ($(element).attr('data-seo-path'))
    {
        parent.find("input").attr('data-seo-path', $(element).attr('data-seo-path'));
    }
    // set select
    if ($(element).attr('data-value') != "*") {
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

    if (loadResult) {
        var rootElemet = '';
        var el = jQuery(element);
        while (true) {
            var el = jQuery(el.parent());
            if (el[0].id != '' && ( el[0].id === 'main-filter-controls' || el[0].id === 'main-filter-controls-mobile'))
            {
                rootElemet = el[0].id;
                break;
            }
        }
        setRelativeValue(rootElemet, parent.find('input').attr('property-name'), value);
        getFoundResults('#' + rootElemet, ["#main-filter-controls", "#main-filter-controls-mobile"]);
    }
}
// set relative element value for other filter
function setRelativeValue(rootElement, propertyName, value)
{
    var prefix = '';
    if (rootElement === 'main-filter-controls') {
        prefix = '-mobile';
    }
    var el = jQuery("#main-filter-controls" + prefix + " input[property-name='" + propertyName + "']").parent();
    $.each(el.children("ul").children("li"), function (index, item) {
        var element = jQuery(item);
        if (element.attr('data-value') === value)
        {
            setDdValue(item, false, false);
        }
    });
}

function setRelativeCheckbox(rootElement, value, isSet) {
    var prefix = '';
    if (rootElement === 'main-filter-controls') {
        prefix = '-mobile';
    }
    var el = jQuery("#main-filter-controls" + prefix + " input[value='" + value + "'][type=checkbox]");
    el.prop("checked", isSet);
}



function loadSearchData(url, search, elements, listElement, rootElement) {
    if (!search || !url) {
        return;
    }
    elements.forEach(function (element) {
        jQuery(element + " input[type=text]").removeClass("selected");
        jQuery.ajax({
            url: url,
            data: JSON.stringify({ search: search }),
            method: "POST",
            success: function (data) {
                if (data.items && data.items.length) {
                    jQuery(element + " .list").html('');
                    for (var i = 0; i < data.items.length; i++) {
                        var item = data.items[i];
                        var infoRegion = "";
                        var infoCity = "";
                        if (item.regionName) {
                            infoRegion = 'data-region="' + item.regionName + '"';
                        }
                        if (item.cityName) {
                            infoCity = 'data-region="' + item.cityName + '"';
                        }
                        jQuery(element + " .list").append('<li data-seo-path="' + item.seo + '" ' + infoRegion + '" ' + infoCity + '" data-value="' + item.fullName + '">' + item.fullName + '</li>')
                    }
                    jQuery.each(jQuery(element + " .list li"), function (index, item) {
                        jQuery(item).click(function () {
                            selectSearchItem(item, elements, listElement, rootElement);
                        });
                    });
                    jQuery(element + " .list").css("display", "block");
                }
            },
            contentType: "application/json"
        });
    });
}

function selectSearchItem(item, elements, listElements, rootElement) {
    elements.forEach(function (element) {
        jQuery(element + " .list").css("display", "none");
        jQuery(element + " input[type=text]").val(jQuery(item).attr("data-value"));
        jQuery(element + " input[type=hidden]").val(jQuery(item).attr("data-seo-path"));
        jQuery(element + " input[type=hidden]").attr("data-region", jQuery(item).attr("data-region"));
        jQuery(element + " input[type=hidden]").attr("data-city", jQuery(item).attr("data-city"));
        jQuery(element + " input[type=text]").addClass("selected");
    });

    getFoundResults(rootElement, listElements);
}

function sortBy(value) {
    if (window.location.href.indexOf("?") === -1) {
        window.location.href += "?sort_by=" + value;
    }
    else {
        var locationParts = window.location.href.split('?');
        var paramsParts = locationParts[1].split('&');
        var paramsStr = "";
        paramsParts.forEach(function (item) {
            if (item.indexOf("sort_by") === -1) {
                paramsStr += item + "&";
            }
        });
        paramsStr += "sort_by=" + value;
        window.location.href = locationParts[0] + "?" + paramsStr;
    }
}
function getRequestSeoPath() {
    var locationPath = jQuery('#location-path').val();
    if (locationPath === undefined)
    {
        locationPath = "";
    }
    var typePath = jQuery('#estate-type-value').attr('data-seo-path');
    if (typePath === undefined)
    {
        typePath = "";
    }
    if (locationPath !== "" && typePath !== "") {
        return locationPath + "/" + typePath;
    }
    else if (locationPath !== "") {
        return locationPath;
    }
    else if (typePath !== "")
    {
        return typePath;
    }
    return "";
}

function valideEmail(str)
{
    var re = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return re.test(str);
}


jQuery(document).ready(function () {

    $('.scroll-btn').on('click', function () {
        slideToBlock('.region-tails');
    });

    jQuery("#subcribe-action").click(function () {

        if (valideEmailjQuery("#subscribe-email").val()) {
            jQuery("#subscribe-email").removeClass("error");
            jQuery("#subscribe-form").hide();
            jQuery("#subscribe-spinner").show();
            jQuery.post('storefrontapi/getresponse/subscribe', { email: jQuery("#subscribe-email").val() }, function (data) {
                jQuery("#subscribe-spinner").hide();
                jQuery("#subscribe-ok").show();
            }).fail(function () {
                jQuery("#subscribe-spinner").hide();
                jQuery("#subscribe-fail").show();
            });
        }
        else {
            jQuery("#subscribe-email").addClass("error");
        }
        return false;
    });
    jQuery("#request-callback-button").click(function () {
        slideToBlock("#callback-form");
        jQuery(".mobilemenu-close").click();
    });
    jQuery("#contact-us-button").click(function () {
        slideToBlock("#contact-us-form");
        jQuery(".mobilemenu-close").click();
    });
    jQuery("#contact-us-button-product").click(function () {
        slideToBlock("#show_contact", 0);
        jQuery(".mobilemenu-close").click();
    });
    jQuery('.js-show-filters').on('click', function () {
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

    if (window.location.href.indexOf("from_filter") > -1 || window.location.href.indexOf("page") > -1) {
        slideToBlock("#list-products");
    }

    jQuery("#to-parthner-form").click(function () {
        slideToBlock("#form-parthner");
    });
    // forms
    jQuery("#callback-send").click(function () {
        // get fields
        var inputs = jQuery("#callback-form").find("input");
        var params = {};
        var isError = false;
        jQuery.each(inputs, function (index, item) {
            var el = jQuery(item);
            var property = el.attr('data-property');
            if (el.val() === '' && property === 'phone') {
                el.addClass("error");
                isError = true;
            }
            else {
                el.removeClass("error");
                params[property] = el.val();
            }
        });
        params['fromUrl'] = window.location.href;
        if (isError) {
            return;
        }
        jQuery("#callback-form").hide();
        jQuery("#callback-spinner").show();
        jQuery.ajax({
            type: 'POST',
            url: 'storefrontapi/forms',
            data: JSON.stringify(params),
            success: function (data) {
                jQuery("#callback-spinner").hide();
                jQuery("#callback-succes").show();
            },
            error: function (err) {
                jQuery("#callback-spinner").hide();
                jQuery("#callback-fail").show();
            },
            contentType: "application/json",
            dataType: 'json'
        });
    });

    jQuery("#request-form-submit").click(function () {
        sendContactUsForm();
    });
    jQuery("#request-form-submit-sidebar").click(function () {
        sendContactUsForm('-sidebar');
    });
    jQuery("#request-form-submit-image").click(function () {
        sendContactUsForm('-image');
    });
    jQuery("#contact-us-form-footer-submit").click(function () {
        sendContactUsForm("-footer");
    })

    function sendContactUsForm(prefix) {
        if (!prefix)
        {
            prefix = "";
        }
        var inputs = jQuery("#contact-us-form" + prefix).find(":input");
        var data = {};
        var isError = false;
        jQuery.each(inputs, function (index, item) {
            var el = jQuery(item);
            var prData = el.attr('data-property');
            var prRequaired = el.attr("required");
            if (prRequaired && (el.val() === '' || (el.attr('type') === 'email' && !valideEmail(el.val())))) {
                el.addClass('error');
                isError = true;
            }
            else {
                el.removeClass('error');
                data[prData] = el.val();
            }
        });
        if (isError) {
            return;
        }
        data['fromUrl'] = window.location.href;
        jQuery("#contact-us-form-spinner" + prefix).show();
        jQuery("#contact-us-form" + prefix).hide();
        jQuery.ajax({
            type: 'POST',
            url: 'storefrontapi/forms',
            data: JSON.stringify(data),
            success: function (data) {
                jQuery("#contact-us-form-spinner" + prefix).hide();
                jQuery("#contact-us-form-succes" + prefix).show();
            },
            error: function (err) {
                jQuery("#contact-us-form-spinner" + prefix).hide();
                jQuery("#contact-us-form-fail" + prefix).show();
            },
            contentType: "application/json",
            dataType: 'json'
        });
    }
    // end forms

    // filter events
    $("#main-filter-controls #location-value").keyup(function (e) {
        if (ignoredKeyCodes.indexOf(e.which) === -1) {
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
        if(ignoredKeyCodes.indexOf(e.which) === -1) {
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
    $.each(jQuery("#main-filter-controls-mobile #filter-checks input"), function (index, item) {
        jQuery(item).change(function () {
            getFoundResults("#main-filter-controls-mobile", ["#main-filter-controls", "#main-filter-controls-mobile"]);
            setRelativeCheckbox("main-filter-controls-mobile", jQuery(item).val(), jQuery(item).prop('checked'));
        });
    });

    $.each(jQuery("#main-filter-controls #filter-checks input"), function (index, item) {
        jQuery(item).change(function () {
            getFoundResults("#main-filter-controls", ["#main-filter-controls", "#main-filter-controls-mobile"]);
            setRelativeCheckbox("main-filter-controls", jQuery(item).val(), jQuery(item).prop('checked'));
        });
    });
    // end filter events
});