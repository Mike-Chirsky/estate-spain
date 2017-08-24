function loadListProducts(url, slideToTop) {
    $(".busy-indicator-product").show();
    $("div#list-products").hide();
    $(".pagination-container.clearfix").hide();
    if (slideToTop) {
        setTimeout(function () { slideToBlock("#list-products"); }, 100);
    }
    $.get(url, function (data, slideToTop) {
        var responseData = $(data);
        var container = responseData.find("div#list-products");
        var pagination = responseData.find(".pagination-container.clearfix");
        $("div#list-products").html(container.html());
        $(".pagination-container.clearfix").html(pagination.html());
        if (window.history && window.history.pushState) {
            window.history.pushState({ container: container.html(), pagination: pagination.html() }, document.title, url);
        }
        $(".busy-indicator-product").hide();
        $("div#list-products").show();
        $(".pagination-container.clearfix").show();
        $(".product-list .product-body").click(function () {
            var link;
            link = $(this).attr('data-link');
            if (link && link != '') {
                window.open(link, '_self');
            }
        });
    }).fail(function (err) {
        $(".busy-indicator-product").hide();
        $("div#list-products").show();
        $(".pagination-container.clearfix").show();
    });
    return false;
}
window.onpopstate = function (e) {
    $("div#list-products").html(e.state.container);
    $(".pagination-container.clearfix").html(e.state.pagination);
    $(".product-list .product-body").click(function () {
        var link;
        link = $(this).attr('data-link');
        if (link && link != '') {
            window.open(link, '_self');
        }
    });
}
function sortBy(value) {
    var url = "";
    if (window.location.href.indexOf("?") === -1) {
        url = window.location.href + "?sort_by=" + value;
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
        url = locationParts[0] + "?" + paramsStr;
    }
    loadListProducts(url);
}
function setCurrentSort(id, asc, resets) {
    if (asc) {
        $(id).parent().parent().addClass("active");
        $(id + "Asc").addClass("active");
        $(id + "Desc").removeClass("active");
    }
    else {
        $(id).parent().parent().addClass("active");
        $(id + "Asc").removeClass("active");
        $(id + "Desc").addClass("active");
    }
    resets.forEach(function (item) {
        $(item).parent().parent().removeClass("active");
        $(item + "Asc").removeClass("active");
        $(item + "Desc").removeClass("active");
    });
}
$(document).ready(function () {
    /* sorting controls*/
    /* sort by price */
    jQuery("#sortByPrice").click(function () {
        if (window.location.href.indexOf("price-ascending") !== -1) {
            sortBy("price-descending");
            setCurrentSort("#sortByPrice", false, ["#sortByDate", "#sortBySquare"]);
        }
        else {
            sortBy("price-ascending");
            setCurrentSort("#sortByPrice", true, ["#sortByDate", "#sortBySquare"]);
        }
    });
    jQuery("#sortByPriceDesc").click(function () {
        if (window.location.href.indexOf("price-descending") === -1) {
            sortBy("price-descending");
            setCurrentSort("#sortByPrice", false, ["#sortByDate", "#sortBySquare"]);
        }
    });
    jQuery("#sortByPriceAsc").click(function () {
        if (window.location.href.indexOf("price-ascending") === -1) {
            sortBy("price-ascending");
            setCurrentSort("#sortByPrice", true, ["#sortByDate", "#sortBySquare"]);
        }
    });
    /* sort by square */
    jQuery("#sortBySquare").click(function () {
        if (window.location.href.indexOf("propertysquare-ascending") !== -1) {
            sortBy("propertysquare-descending");
            setCurrentSort("#sortBySquare", false, ["#sortByPrice", "#sortByDate"]);
        }
        else {
            sortBy("propertysquare-ascending");
            setCurrentSort("#sortBySquare", true, ["#sortByPrice", "#sortByDate"]);
        }
    });

    jQuery("#sortBySquareDesc").click(function () {
        if (window.location.href.indexOf("propertysquare-descending") === -1) {
            sortBy("propertysquare-descending");
            setCurrentSort("#sortBySquare", false, ["#sortByPrice", "#sortByDate"]);
        }
    });
    jQuery("#sortBySquareAsc").click(function () {
        if (window.location.href.indexOf("propertysquare-ascending") === -1) {
            sortBy("propertysquare-ascending");
            setCurrentSort("#sortBySquare", true, ["#sortByPrice", "#sortByDate"]);
        }
    });
    /* sort by date*/
    jQuery("#sortByDate").click(function () {
        if (window.location.href.indexOf("createddate-ascending") !== -1) {
            sortBy("createddate-descending");
            setCurrentSort("#sortByDate", false, ["#sortByPrice", "#sortBySquare"]);
        }
        else {
            sortBy("createddate-ascending");
            setCurrentSort("#sortByDate", true, ["#sortByPrice", "#sortBySquare"]);
        }
    });
    jQuery("#sortByDateAsc").click(function () {
        if (window.location.href.indexOf("createddate-ascending") === -1) {
            sortBy("createddate-ascending");
            setCurrentSort("#sortByDate", true, ["#sortByPrice", "#sortBySquare"]);
        }
    });
    jQuery("#sortByDateDesc").click(function () {
        if (window.location.href.indexOf("createddate-descending") === -1) {
            sortBy("createddate-descending");
            setCurrentSort("#sortByDate", false, ["#sortByPrice", "#sortBySquare"]);
        }
    });
})
