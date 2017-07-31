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

function valideEmail(str)
{
    var re = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return re.test(str);
}
jQuery(document).ready(function () {

    // accordion actions on info blog page
    $('#accordingPay').click(function () {
        setTimeout(function () {
            slideToBlock("#accordingPay");
        }, 300);

    });
    $('#accordingMortagage').click(function () {
        setTimeout(function () {
            slideToBlock("#accordingMortagage");
        }, 300);
    });
    $('#accordingVisa').click(function () {
        setTimeout(function () {
            slideToBlock("#accordingVisa");
        }, 300);
    });
    $('#accordingAdd').click(function () {
        setTimeout(function () {
            slideToBlock("#accordingAdd");
        }, 300);
    });
    $('#accordingCoasts').click(function () {
        setTimeout(function () {
            slideToBlock("#accordingCoasts");
        }, 300);
    });
    // end according
    $('.scroll-btn').on('click', function () {
        slideToBlock('.region-tails');
    });

    $('.menu .menu-item').on('click', function () {
        var li = $(this);

        if(li.hasClass('selected')) {
            li.removeClass('selected');
        }
        else {
            li.addClass('selected').siblings().removeClass('selected');
        }
    });

    $(document).on('click', function (e) {
        if($('.menu .menu-item').has(e.target).length === 0) {
            $('.menu .menu-item').removeClass('selected');
        }
    });

    jQuery("#subcribe-action").click(function () {

        if (valideEmailjQuery("#subscribe-email").val()) {
            jQuery("#subscribe-email").removeClass("error");
            jQuery("#subscribe-form").hide();
            jQuery("#subscribe-spinner").show();
            jQuery.post('storefrontapi/getresponse/subscribe', { email: jQuery("#subscribe-email").val() }, function (data) {
                jQuery("#subscribe-spinner").hide();
                jQuery("#subscribe-ok").show();
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "subscribe",
                        eventURL: window.location.href
                    });
            }).fail(function () {
                jQuery("#subscribe-spinner").hide();
                jQuery("#subscribe-fail").show();
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "subscribe-error",
                        eventURL: window.location.href
                    });
            });
        }
        else {
            jQuery("#subscribe-email").addClass("error");
        }
        return false;
    });
    jQuery("#request-callback-button-mobile").click(function () {
        slideToBlock("#callback-form");
        jQuery(".mobilemenu-close").click();
    });
    jQuery("#contact-us-button-mobile").click(function () {
        slideToBlock("#contact-us-form");
        jQuery(".mobilemenu-close").click();
    });
    jQuery("#contact-us-button-product-mobile").click(function () {
        slideToBlock("#show_contact", 0);
        jQuery(".mobilemenu-close").click();
    });
    jQuery("#request-callback-button").click(function () {
        slideToBlock("#callback-form");
    });
    jQuery("#contact-us-button").click(function () {
        slideToBlock("#contact-us-form");
    });
    jQuery("#contact-us-button-product").click(function () {
        slideToBlock("#show_contact", 0);
    });

    if (window.location.href.indexOf("from_filter") > -1 || window.location.href.indexOf("page") > -1) {
        slideToBlock("#list-products");
    }

    jQuery("#to-parthner-form").click(function () {
        slideToBlock("#contact-us-form");
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
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "callback",
                        eventURL: window.location.href
                    });
            },
            error: function (err) {
                jQuery("#callback-spinner").hide();
                jQuery("#callback-fail").show();
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "callback-error",
                        eventURL: window.location.href
                    });
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
    });

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
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "contact-us" + prefix,
                        eventURL: window.location.href
                    });
            },
            error: function (err) {
                jQuery("#contact-us-form-spinner" + prefix).hide();
                jQuery("#contact-us-form-fail" + prefix).show();
                if (window.dataLayer)
                    window.dataLayer.push({
                        event: "form",
                        eventCategory: "Send Form",
                        eventAction: "contact-us" +prefix + "-error",
                        eventURL: window.location.href
                    });
            },
            contentType: "application/json",
            dataType: 'json'
        });
    }
    // end forms
    // set active sort by square
    if (window.location.href.indexOf("sort_by=propertysquare-ascending") > -1)
    {
        $("#sortBySquare").parent().parent().addClass("active");
        $("#sortBySquareAsc").addClass("active");
    }
    if (window.location.href.indexOf("sort_by=propertysquare-descending") > -1) {
        $("#sortBySquare").parent().parent().addClass("active");
        $("#sortBySquareDesc").addClass("active");
    }
    // set active sort by date
    if (window.location.href.indexOf("sort_by=created-ascending") > -1) {
        $("#sortByDate").parent().parent().addClass("active");
        $("#sortByDateAsc").addClass("active");
    }
    if (window.location.href.indexOf("sort_by=created-descending") > -1) {
        $("#sortByDate").parent().parent().addClass("active");
        $("#sortByDateDesc").addClass("active");
    }
    // set active sort by price
    if (window.location.href.indexOf("sort_by=price-ascending") > -1) {
        $("#sortByPrice").parent().parent().addClass("active");
        $("#sortByPriceAsc").addClass("active");
    }
    if (window.location.href.indexOf("sort_by=price-descending") > -1) {
        $("#sortByPrice").parent().parent().addClass("active");
        $("#sortByPriceDesc").addClass("active");
    }
    if (window.location.href.indexOf("#")) {
        setTimeout(function () { slideToBlock("#" + window.location.href.split("#")[1]) }, 300);
    }
    // lazy load for slider
    $(".carousel").on("slide.bs.carousel", function (ev) {
        lazy = $(ev.relatedTarget).find("img[data-src]");
        lazy.attr("src", lazy.data('src'));
        lazy.removeAttr("data-src");
    });
});