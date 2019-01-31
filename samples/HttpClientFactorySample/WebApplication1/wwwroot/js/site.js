// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function() {
    $("[data-load]").each(function() {
        var $el = $(this);
        $el.load($el.attr("data-load"));
    })
})
