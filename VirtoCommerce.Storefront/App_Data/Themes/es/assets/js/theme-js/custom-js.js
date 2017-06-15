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
            if (jQuery(item).val() && jQuery(item).val() != "*") {
                terms.push({
                    name: jQuery(item).attr("property-name"), value: jQuery(item).val()
                });
            }
        });
    });

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
            jQuery("#results_no").html(data.metaData.totalItemCount);
            jQuery("#results").css("opacity", "1");
            setTimeout(function () {
                jQuery("#results").css("opacity", "0");
            }, 4000);
        },
        method: "POST",
        contentType: "application/json"
    });
}