function createCustomSlider(contentId, marketId, url) {
    var el = jQuery(contentId);
    var page = 1;
    var spinner = jQuery(contentId + " .spinner");
    var content = jQuery(contentId + " .market-block-content");
    var next = jQuery(contentId + " .next");
    var prev = jQuery(contentId + " .prev");
    function loadData(fromNext) {
        jQuery.get(url + '/' + marketId + '/' + page + '/3', function (data) {
            spinner.hide();
            if (data !== '') {
                content.html(data);
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
    var el = jQuery(contentId);
    var page = 1;
    var spinner = jQuery(contentId + " .spinner");
    var content = jQuery(contentId + " .market-block-content");
    var more = jQuery(contentId + " .more-button button");
    function loadData() {
        jQuery.get(url + '/' + marketId + '/' + page + '/4', function (data) {
            spinner.hide();
            if (data === '') {
                more.hide();
            }
            else {
                more.show();
            }
            content.append(data);
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