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

function setFilter(mainUrl) {
    var queryString = "";
    /*if (jQuery("#region-value").val() && jQuery("#region-value").val() != "*") {
        queryString += "region:" + jQuery("#region-value").val() + ";";
    }
    if (jQuery("#city-value").val() && jQuery("#city-value").val() != "*") {
        queryString += "city:" + jQuery("#city-value").val() + ";";
    }
    if (jQuery("#estate-type-value").val() && jQuery("#estate-type-value").val() != "*") {
        queryString += "estatetype:" + jQuery("#estate-type-value").val() + ";";
    }
    if (jQuery("#condition-value").val() && jQuery("#condition-value").val() != "*") {
        queryString += "condition:" + jQuery("#condition-value").val() + ";";
    }
    if (jQuery("#distancetosea-range").val() && jQuery("#distancetosea-range").val() != "*") {
        queryString += "distancetosea:" + jQuery("#distancetosea-range").val() + ";";
    }
    if (jQuery("#price-value").val() && jQuery("#price-value").val() != "*") {
        queryString += "price:" + jQuery("#price-value").val() + ";";
    }*/
    var ids_container_controls = ["#main-filter-controls input", "#addition-filter-controls input"];
    ids_container_controls.forEach(function (key) {
        $.each(jQuery(key), function (index, item) {
            if (jQuery(item).val() && jQuery(item).val() != "*") {
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
        window.location.href = mainUrl + '?terms=' + encodeURIComponent(queryString.substr(0, queryString.length - 1));
    }
}
