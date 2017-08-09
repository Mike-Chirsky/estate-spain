function createCustomSlider(contentId, marketId, url) {
    var el = $(contentId);
    var page = 1;
    var spinner = $(contentId + " .spinner");
    var content = $(contentId + " .market-block-content");
    var next = $(contentId + " .next");
    var prev = $(contentId + " .prev");
    function loadData(fromNext) {
        $.get(url + '/' + marketId + '/' + page + '/3', function (data) {
            spinner.hide();
            if (data !== '') {
                content.html(data);
                setTimeout(function () {
                    lazyLoad(content);
                }, 100);
                $(".market-block-content .product-body").click(function () {
                    var link;
                    link = $(this).attr('data-link');
                    if (link) {
                        window.open(link, '_self');
                    }
                });
                prev.removeClass('disabled');
                next.removeClass('disabled');
            }
            else {
                if (fromNext) {
                    next.addClass('disabled');
                }
                else {
                    prev.addClass('disabled');
                }
            }

            content.show();
        }).fail(function () {
            el.hide();
        });
        spinner.show();
        content.hide();
    }
    next.click(function () {
        page++;
        loadData(true);
    });
    prev.click(function () {
        page--;
        if (page < 1) {
            page = 1;
        }
        loadData(false);
    });
    loadData();
}

function createCustomSliderMobile(contentId, marketId, url) {
    var el = $(contentId);
    var page = 1;
    var spinner = $(contentId + " .spinner");
    var content = $(contentId + " .market-block-content");
    var more = $(contentId + " .more-button button");
    function loadData() {
        $.get(url + '/' + marketId + '/' + page + '/4', function (data) {
            spinner.hide();
            if (data === '') {
                more.hide();
            }
            else {
                more.show();
            }
            content.append(data);
            if (data != '') {
                $(".market-block-content .product-body").click(function () {
                    var link;
                    link = $(this).attr('data-link');
                    if (link) {
                        window.open(link, '_self');
                    }
                });
            }
            setTimeout(function () {
                lazyLoad(content);
            }, 100);
            lazyLoad(content);
        }).fail(function () {
            el.hide();
        });
        spinner.show();
    }
    more.click(function () {
        page++;
        loadData();
    });
    loadData();
}

function lazyLoad(element) {
    $(element).find(".carousel").on("slide.bs.carousel", function (ev) {
        lazy = $(ev.relatedTarget).find("img[data-src]");
        lazy.attr("src", lazy.data('src'));
        lazy.removeAttr("data-src");
    });
}