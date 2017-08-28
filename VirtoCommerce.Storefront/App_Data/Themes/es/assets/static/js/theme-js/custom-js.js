function slideToBlock(selector, offest) {
    if (offest === undefined)
    {
        offest = 200;
    }
    var el = $(selector);
    if (!el || !el.offset())
    {
        return false;
    }
    $('html, body').animate({ scrollTop: el.offset().top - offest }, 500);
    return false;
}

function valideEmail(str)
{
    var re = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return re.test(str);
}

function sendGAEvent(category, action, label, prefix) {
    if (prefix === "-sidebar") label = "Right Side";
    if (prefix === "-footer") label = "Popup bottom";
    if (prefix === "") label = "Bottom";

    if (window.gaPage) 
        label = window.gaPage + ": " + label;

    if (window.dataLayer)
        window.dataLayer.push({
            event: "form",
            eventCategory: category,
            eventAction: action,
            eventURL: label
        });
}

$(document).ready(function () {

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
    document.subscribe = function (contaner) {
        if (valideEmail($(contaner+" #subscribe-email").val())) {
            $(contaner +" #subscribe-email").removeClass("error");
            $(contaner +" #subscribe-form").hide();
            $(contaner +" #subscribe-spinner").show();
            $.post('storefrontapi/getresponse/subscribe', { email: $(contaner +" #subscribe-email").val() }, function (data) {
                $(contaner +" #subscribe-spinner").hide();
                $(contaner +" #subscribe-ok").show();
                sendGAEvent("Email subscription form", "Submitted success", "Footer");
            }).fail(function () {
                $(contaner +" #subscribe-spinner").hide();
                $(contaner +" #subscribe-fail").show();
                sendGAEvent("Email subscription form", "Submitted fail", "Footer");
            });
        }
        else {
            $(contaner +" #subscribe-email").addClass("error");
        }
        return false;
    }
    $("#request-callback-product-button-mobile").click(function () {
        slideToBlock("#show_contact", 0);
        $(".mobilemenu-close").click();
    });
    
    $("#request-callback-button-mobile").click(function () {
        slideToBlock("#contact-us-form");
        $(".mobilemenu-close").click();
    });
    $("#contact-us-button-mobile").click(function () {
        slideToBlock("#contact-us-form");
        $(".mobilemenu-close").click();
    });
    $("#contact-us-button-product-mobile").click(function () {
        slideToBlock("#show_contact", 0);
        $(".mobilemenu-close").click();
    });
    $("#request-callback-button").click(function () {
        slideToBlock("#contact-us-form");
    });
    $("#contact-us-button").click(function () {
        slideToBlock("#contact-us-form");
    });
    $("#contact-us-button-product").click(function () {
        slideToBlock("#show_contact", 0);
    });

    if (window.location.href.indexOf("from_filter") > -1 || window.location.href.indexOf("page") > -1) {
        slideToBlock("#list-products");
    }

    $("#to-parthner-form").click(function () {
        slideToBlock("#partner-form");
    });
    // forms
    $("#partner-form-submit").click(function(){
        var inputs = $("#partner-form").find("input");
        var params = {};
        var isError = false;
        $.each(inputs, function (index, item) {
            var el = $(item);
            var prData = el.attr('data-property');
            var prRequaired = el.attr("required");
            if (prRequaired && (el.val() === '' || (el.attr('data-property') === 'userEmail' && !valideEmail(el.val())))) {
                el.addClass('error');
                isError = true;
            }
            else {
                el.removeClass('error');
                params[prData] = el.val();
            }
        });
        if (isError) {
            return;
        }
        params['fromUrl'] = window.location.href;
        $("#partner-form").hide();
        $("#partner-form-spinner").show();
        $.ajax({
            type: 'POST',
            url: 'storefrontapi/forms',
            data: JSON.stringify(params),
            success: function (data) {
                $("#partner-form-spinner").hide();
                $("#partner-form-succes").show();
                sendGAEvent("Call me form", "Submitted success", "Bottom");
            },
            error: function (err) {
                $("#partner-form-spinner").hide();
                $("#partner-form-fail").show();
                $("#partner-form").show();
                sendGAEvent("Call me form", "Submitted fail", "Bottom");
            },
            contentType: "application/json",
            dataType: 'json'
        });
    });
    $("#callback-send").click(function () {
        // get fields
        var inputs = $("#callback-form").find("input");
        var params = {};
        var isError = false;
        $.each(inputs, function (index, item) {
            var el = $(item);
            var property = el.attr('data-property');
            if (el.val() === '' && property === 'userPhone') {
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
        $("#callback-form").hide();
        $("#callback-spinner").show();
        $.ajax({
            type: 'POST',
            url: 'storefrontapi/forms',
            data: JSON.stringify(params),
            success: function (data) {
                $("#callback-spinner").hide();
                $("#callback-succes").show();
                sendGAEvent("Call me form", "Submitted success", "Bottom");
            },
            error: function (err) {
                $("#callback-spinner").hide();
                $("#callback-fail").show();
                $("#callback-form").show();
                sendGAEvent("Call me form", "Submitted fail", "Bottom");
            },
            contentType: "application/json",
            dataType: 'json'
        });
    });

    $("#request-form-submit").click(function () {
        sendContactUsForm();
    });
    $("#request-form-submit-sidebar").click(function () {
        sendContactUsForm('-sidebar');
    });
    $("#request-form-submit-image").click(function () {
        sendContactUsForm('-image');
    });
    $("#contact-us-form-footer-submit").click(function () {
        sendContactUsForm("-footer");
    });

    function sendContactUsForm(prefix) {
        if (!prefix)
        {
            prefix = "";
        }
        var inputs = $("#contact-us-form" + prefix).find(":input");
        var data = {};        
        $.each(inputs, function (index, item) {
            var el = $(item);
            var prData = el.attr('data-property');
            data[prData] = el.val();
            //var prRequaired = el.attr("required");
            /*if ((prData === 'userPhone' && el.val() === '') || (el.attr('type') === 'email' && !valideEmail(el.val()))) {
                if (!existEmail && !existPhone) {
                    $("#contact-us-message" + prefix).text("Необходимо заполнить номер телефона или Email");
                    el.addClass('error');
                    isError = true;
                }
            }
            else {
                if (prData === "userPhone") {
                    existPhone = true;
                }
                if (prData === "userEmail") {
                    existEmail = true;
                }
                el.removeClass('error');
                data[prData] = el.val();
            }*/
        });
        var hasEmail = false;
        var hasPhone = false;

        for (var prop in data)
        {
            if (!data.hasOwnProperty(prop))
            {
                continue;
            }
            var vl = data[prop];
            if (prop === 'userPhone') {
                hasPhone = vl !== '';
            }
            else if (prop === 'userEmail')
            {
                hasEmail = vl !== '' && valideEmail(vl);
            }
        }
        if (!(hasEmail || hasPhone)) {
            $("#contact-us-message" + prefix).text("Необходимо заполнить номер телефона или email");
            return;
        }
        data['fromUrl'] = window.location.href;
        $("#contact-us-form-spinner" + prefix).show();
        $("#contact-us-form" + prefix).hide();
        $.ajax({
            type: 'POST',
            url: 'storefrontapi/forms',
            data: JSON.stringify(data),
            success: function (data) {
                $("#contact-us-form-spinner" + prefix).hide();
                $("#contact-us-form-succes" + prefix).show();
                sendGAEvent("Contact me form", "Submitted success", null, prefix);
            },
            error: function (err) {
                $("#contact-us-form-spinner" + prefix).hide();
                $("#contact-us-form-fail" + prefix).show();
                $("#contact-us-form" + prefix).show();
                sendGAEvent("Contact me form", "Submitted fail", null, prefix);
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
    if (window.location.href.indexOf("sort_by=createddate-ascending") > -1) {
        $("#sortByDate").parent().parent().addClass("active");
        $("#sortByDateAsc").addClass("active");
    }
    if (window.location.href.indexOf("sort_by=createddate-descending") > -1) {
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
