$(function() {
    'use strict';

    $('#defaultconfig').maxlength({
        warningClass: "badge mt-1 badge-success",
        limitReachedClass: "badge mt-1 badge-danger"
    });

    $('#defaultconfig-2').maxlength({
        alwaysShow: true,
        threshold: 20,
        warningClass: "badge mt-1 badge-success",
        limitReachedClass: "badge mt-1 badge-danger"
    });

    $('#username').maxlength({
        alwaysShow: true,
        threshold: 6,
        warningClass: "badge mt-1 badge-success",
        limitReachedClass: "badge mt-1 badge-danger"
    });
    $('#password').maxlength({
        alwaysShow: true,
        threshold: 6,
        warningClass: "badge mt-1 badge-success",
        limitReachedClass: "badge mt-1 badge-danger"
    });

    $('#maxlength-textarea').maxlength({
        alwaysShow: true,
        warningClass: "badge mt-1 badge-success",
        limitReachedClass: "badge mt-1 badge-danger"
    });
});