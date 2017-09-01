$(function () {
    $.validator.methods.date = function (value, element) {
        if ($.browser.webkit) {

            //ES - Chrome does not use the locale when new Date objects instantiated:
            var d = new Date();

            return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
        }
        else {

            return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
        }
    };

    $.validator.methods.date = function (value, element) {
        if ($.browser.webkit) {
            var d = value.split("/");
            return this.optional(element) || !/Invalid|NaN/.test(new Date((/chrom(e|ium)/.test(navigator.userAgent.toLowerCase())) ? d[1] + "/" + d[0] + "/" + d[2] : value));
        }
        else {
            return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
        }
    };
});