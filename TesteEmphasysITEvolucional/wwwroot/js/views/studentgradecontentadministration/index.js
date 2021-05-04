(function (next) {

    next(window.jQuery, window, document);

}(function ($, window, document) {

    $(function () {

        $('#frmLoadDatabase').on('submit', function (e) {
            $(this).find('input[type="submit"]').prop('disabled', true);
        });
    });

}));