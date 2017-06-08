function slideToBlock(selector) {
    $('html, body').animate({ scrollTop: jQuery(selector).offset().top - 150 }, 500);
    return false;
}
