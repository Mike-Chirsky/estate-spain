function slideToBlock(selector) {
    $('html, body').animate({ scrollTop: jQuery(selector).offset().top - 150 }, 500);
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
        url: "/storefrontapi/catalog/search",
        data: JSON.stringify({ mutableTerms: terms }),
        success: function (data) {
            $("#no-filter").hide();
            $("#count-objects").show();
            $("#result-count").text(data.metaData.totalItemCount);
            /*jQuery("#results_no").html(data.metaData.totalItemCount);
            jQuery("#results").css("opacity", "1");
            setTimeout(function () {
                jQuery("#results").css("opacity", "0");
            }, 4000);*/
        },
        method: "POST",
        contentType: "application/json"
    });
}

function loadSearchData(url, search, element) {
    if (!search || !url) {
        return;
    }
    jQuery.ajax({
        url: url,
        data: JSON.stringify({ search: search }),
        method: "POST",
        success: function (data) {
            if (data.items && data.items.length) {
                jQuery(element + " .list").html('');
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items[i];
                    jQuery(element + " .list").append('<li data-seo-path="' + item.fullSeo + '" data-region="' + item.regionName + '" data-city="' + item.cityName + '" data-value="' + item.fullName + '">' + item.fullName + '</li>')
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