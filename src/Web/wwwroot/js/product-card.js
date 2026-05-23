document.addEventListener("DOMContentLoaded", function () {
    var cards = document.querySelectorAll(".product-card-clickable");
    cards.forEach(function (card) {
        card.addEventListener("click", function (event) {
            if (event.target.closest(".card-action-button")) {
                return;
            }
            var url = card.dataset.productUrl;
            if (url) {
                window.location.href = url;
            }
        });

        card.addEventListener("keydown", function (event) {
            if ((event.key === "Enter" || event.key === " ") && !event.target.closest(".card-action-button")) {
                event.preventDefault();
                var url = card.dataset.productUrl;
                if (url) {
                    window.location.href = url;
                }
            }
        });
    });
});
