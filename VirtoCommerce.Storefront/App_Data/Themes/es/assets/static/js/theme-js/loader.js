function loadListProducts(url)
{
    $.get(url, function (data) {
        var responseData = $(data);
        var container = responseData.find("div#list-products");
        var pagination = responseData.find(".pagination-container.clearfix");
        $("div#list-products").html(container.html());
        $(".pagination-container.clearfix").html(pagination.html());
    });
}