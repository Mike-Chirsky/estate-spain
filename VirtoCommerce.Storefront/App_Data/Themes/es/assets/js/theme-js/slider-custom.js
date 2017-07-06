function createCustomSlider(contentId, marketId, url) {
    var el = jQuery(contentId);
    var page = 1;
    var spinner = jQuery(contentId + " .spinner");
    var content = jQuery(contentId + " .market-block-content");
    var next = jQuery(contentId + " .next");
    var prev = jQuery(contentId + " .prev");
    function loadData() {
        jQuery.get(url + '/' + marketId + '/' + page, function (data) {
            spinner.hide();
            content.html(data);
            content.show();
        }).fail(function () {
            el.hide();
        });
        spinner.show();
        content.hide();
    }
    next.click(function () {
        page++;
        loadData();
    });
    prev.click(function () {
        page--;
        if (page < 1)
        {
            page = 1;
        }
        loadData();
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
        jQuery.get(url + '/' + marketId + '/' + page, function (data) {
            spinner.hide();
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