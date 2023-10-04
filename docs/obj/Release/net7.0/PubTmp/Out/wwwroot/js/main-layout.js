$(".dropdown-item").on("click", (el) => {
    $.each($(".dropdown-item"), function (index, item) {
        $(item).removeClass("active");
    });
    $(el.target).addClass("active");
});

window.getActiveFilterType = function () {
    let active = $(".active");
    let value = $(active[0]).text();
    return value;
};