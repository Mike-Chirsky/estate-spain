function slideToBlock(selector) {
    $('html, body').animate({ scrollTop: jQuery(selector).offset().top - 200 }, 500);
    return false;
}

function setCurrentValueDd(items, value) {
    jQuery.each(items, function (index, item) {
        if (jQuery(item).attr('data-value') === value) {
            jQuery(item).click();
        }
    });
}
var ids_container_controls = ["#main-filter-controls input", "#addition-filter-controls input"];
function getTerms() {
    var terms = [];
    ids_container_controls.forEach(function (key) {
        $.each(jQuery(key), function (index, item) {
            if (jQuery(item).val() && jQuery(item).val() != "*" && jQuery(item).attr("data-ignore") === undefined) {
                terms.push({
                    name: jQuery(item).attr("property-name"), value: jQuery(item).val()
                });
            }
        });
    });

    // filter by location
    var regionElementData = jQuery("#location-path");
    if (regionElementData.val()) {

        if (regionElementData.attr("data-region") && regionElementData.attr("data-region") !== "") {
            terms.push({ name: "region", value: regionElementData.attr("data-region") });
        }
        if (regionElementData.attr("data-city") && regionElementData.attr("data-city") !== "") {
            terms.push({ name: "city", value: regionElementData.attr("data-city") });
        }
    }

    if ($("#estate-type-value").val()) {
        terms.push({ name: "estatetype", value: $("#estate-type-value").val() });
    }

    if (jQuery("#filter-checks input:checked").length > 0) {
        var sysfilterItem = { name: "sys_filter:", value: "" };
        $.each(jQuery("#filter-checks input:checked"), function (index, item) {
            sysfilterItem.value += jQuery(item).val() + ",";
        });
        terms.push(sysfilterItem);
    }
    return terms;
}

function getTermsString() {
    var queryString = "";
    ids_container_controls.forEach(function (key) {
        $.each(jQuery(key), function (index, item) {
            if (jQuery(item).val() && jQuery(item).val() != "*" && jQuery(item).attr("data-ignore") === undefined) {
                queryString += jQuery(item).attr("property-name") + ":" + jQuery(item).val() + ";";
            }
        });
    });

    if (jQuery("#filter-checks input:checked").length > 0) {
        queryString += "sys_filter:";
        $.each(jQuery("#filter-checks input:checked"), function (index, item) {
            queryString += jQuery(item).val() + ",";
        });
    }
    if (queryString.length > 0) {
        return queryString.substr(0, queryString.length - 1);
    }
    return "";
}

function getFoundResults() {
    var terms = getTerms();
    if (terms.length === 0) {
        return;
    }
    
    jQuery.ajax({
        url: "storefrontapi/catalog/search",
        data: JSON.stringify({ mutableTerms: terms, responseGroup: 'ItemInfo' }),
        success: function (data) {
            $("#no-filter").hide();
            $("#count-objects").show();
            $("#result-count").text(data.metaData.totalItemCount);
            fillElement("#estate-type-value", data.aggregations, "estatetype");
            fillElement("#other_type", data.aggregations, "other_type");
            fillElement("#condition-value", data.aggregations, "condition");
            

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
    $(elem + ' li').click(function (event) {
        event.preventDefault();
        setDdValue(this, true, true);
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
        getFoundResults();
    }
}

function loadSearchData(url, search, element) {
    if (!search || !url) {
        return;
    }
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
                    jQuery(element + " .list").append('<li data-seo-path="' + item.seo + '" data-region="' + item.regionName + '" data-city="' + item.cityName + '" data-value="' + item.fullName + '">' + item.fullName + '</li>')
                }
                jQuery.each(jQuery(element + " .list li"), function (index, item) {
                    jQuery(item).click(function () {
                        selectSearchItem(item, element);
                    });
                });
                jQuery(element + " .list").css("display", "block");
            }
        },
        contentType: "application/json"
    });
}

function selectSearchItem(item, element) {
    jQuery(element + " .list").css("display", "none");
    jQuery(element + " input[type=text]").val(jQuery(item).attr("data-value"));
    jQuery(element + " input[type=hidden]").val(jQuery(item).attr("data-seo-path"));
    jQuery(element + " input[type=hidden]").attr("data-region", jQuery(item).attr("data-region"));
    jQuery(element + " input[type=hidden]").attr("data-city", jQuery(item).attr("data-city"));
    jQuery(element + " input[type=text]").addClass("selected");
    getFoundResults();
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
    });
    jQuery("#contact-us-button").click(function () {
        slideToBlock("#contact-us-form");
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

    if (window.location.href.indexOf("from_filter") > -1) {
        slideToBlock("#list-products");
    }

    jQuery("#to-parthner-form").click(function () {
        slideToBlock("#form-parthner");
    });

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
        if (isError) {
            return;
        }
        jQuery("#callback-form").hide();
        jQuery("#callback-spinner").show();
        var succes = function () {
            jQuery("#callback-spinner").hide();
            jQuery("#callback-succes").show();
        };

        var fail = function () {
            jQuery("#callback-spinner").hide();
            jQuery("#callback-fail").show();
        };
        succes();
    });

    jQuery("#request-form-submit").click(function () {
        sendContactUsForm();
    });
    jQuery("#request-form-submit-sidebar").click(function () {
        sendContactUsForm('-sidebar');
    });
    jQuery("#contact-us-form-footer-submit").click(function () {
        sendContactUsForm("-footer");
    })

    function sendContactUsForm(prefix) {
        var inputs = jQuery("#contact-us-form" + prefix).find("input");
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
                data[prData] = el.data;
            }
        });
        if (isError) {
            return;
        }
        jQuery("#contact-us-form-spinner" + prefix).show();
        jQuery("#contact-us-form" + prefix).hide();
        var succes = function () {
            jQuery("#contact-us-form-spinner" + prefix).hide();
            jQuery("#contact-us-form-succes" + prefix).show();
        };

        var fail = function () {
            jQuery("#contact-us-form-spinner" + prefix).hide();
            jQuery("#contact-us-form-fail" + prefix).show();
        };
        succes();
    }
});