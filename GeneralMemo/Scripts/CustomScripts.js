$(document).ready(function () {

    var tform = document.forms[0];
    
    $(".tgtvals").addClass("positive-integer");
    $(".positive-integer").numeric({ decimal: false, negative: false }, function () { alert("Positive integers only"); this.value = ""; this.focus(); });
    //sumOfColumns();

    /*$(document).on("click", ".tgtvals", function () {
        var tgt = $(this);
        if (tgt.val()=="0" ) {
            tgt.val("");
        }
    });*/

    //when the values change validate the entries
    /**VALIDATE A TARGET ENTRY**/
    /*$(document).on("blur", ".tgtvals", function () {
        var tgt = $(this);
        tgt.removeClass("positive-integer");
        var RequestStageID = tform.RequestStageID.value.toLowerCase();
        if (RequestStageID != "hr upload") {
            var r = $.trim(tgt.val());
            if (r == null || r == "") {
                //@ViewBag.ErrorMessage="Please provide a numeric value.";
                //showDialog("Error", "Please provide a numeric value.", 560);
                tgt.addClass('errorRow').val(0).focus();
                return false;
            } else {
                tgt.removeClass('errorRow');
                var k = tgt.val();
                var e = k;
                if (e.length > 1) {
                    e.replace(/^[0]+/g, "")
                }
                if (e == "0" || parseInt(e, 10) == 0) {
                    tgt.val(0);
                    //return false
                }
                if (k.length > 1) {
                    k = addCommas(k.replace(/^[0]+/g, ""));
                    tgt.val(k);
                }
            }
        }
        sumOfColumns();
    });*/

    /*$(document).on("focus", ".tgtvals", function () {
        var tgt = $(this);
        tgt.addClass("positive-integer");
        $(".positive-integer").numeric({ decimal: false, negative: false }, function () { this.value = "0"; this.focus(); });
    });*/

    /*$(".has-delete").not(":last").on("click", function (e) {
        //console.log($(this).parent());
        var staffID     = $(this).parent().find('td:nth-child(3) input').val();
        var staffName   = $(this).parent().find('td:nth-child(2) input').val();
        if (confirm("Delete this entry / " + staffName + "?")) {
            tform.StaffNumber.value = staffID;
            $("#DeleteStaff").trigger("click");
        } else {
            tform.StaffNumber.value = "";
        }
    });*/

    //lets get the totals
    /*function sumOfColumns() {
        var cabalsum = 0;
        var cabal_lsum = 0;
        var sabalsum = 0;
        var sabal_lsum = 0;
        var fxsum = 0;
        var rvsum = 0;
        var fdsum = 0;
        var incsum = 0;
        var inc_lsum = 0;

        $(".cabal").each(function (index) { cabalsum += new Number(removeCommas($(this).val())); });
        $(".cabal_l").each(function (index) { cabal_lsum += new Number(removeCommas($(this).val())); });
        $(".sabal").each(function (index) { sabalsum += new Number(removeCommas($(this).val())); });
        $(".sabal_l").each(function (index) { sabal_lsum += new Number(removeCommas($(this).val())); });
        $(".fx").each(function (index) { fxsum += new Number(removeCommas($(this).val())); });
        $(".rv").each(function (index) { rvsum += new Number(removeCommas($(this).val())); });
        $(".fd").each(function (index) { fdsum += new Number(removeCommas($(this).val())); });
        $(".inc").each(function (index) { incsum += new Number(removeCommas($(this).val())); });
        $(".inc_l").each(function (index) { inc_lsum += new Number(removeCommas($(this).val())); });

        if( $("tr#sum_row").length<=0 ){
            $(".inputTargetGridInitiate tbody").append("<tr id='sum_row'><td colspan='4' align='right'>TOTAL BRANCH /DEPT HEAD FIGURES</td>" +
                                                "<td class='cabalsum'></td>" +
                                                "<td class='cabal_lsum'></td>" +
                                                "<td class='sabalsum'></td>" +
                                                "<td class='sabal_lsum'></td>" +
                                                "<td class='fxsum'></td>" +
                                                "<td class='rvsum'></td>" +
                                                "<td class='fdsum'></td>" +
                                                "<td class='incsum'></td>" +
                                                "<td class='inc_lsum'></td>" +
                                                "</tr>");
        }
        $(".cabalsum").html(addCommas(cabalsum) );
        $(".cabal_lsum").html(addCommas(cabal_lsum));
        $(".sabalsum").html(addCommas(sabalsum));
        $(".sabal_lsum").html(addCommas(sabal_lsum));
        $(".fxsum").html(addCommas(fxsum));
        $(".rvsum").html(addCommas(rvsum));
        $(".fdsum").html(addCommas(fdsum));
        $(".incsum").html(addCommas(incsum));
        $(".inc_lsum").html(addCommas(inc_lsum));
    }*/
    /***end:lets get the totals**/
    /*function getStaffProfile(staffnumber) {
        //alert()
        //    alert(staffnumber)
        var StaffNumber = "";
        var StaffName = "";
        var StaffGrade = "";

        $.ajax({
            type: "POST",
            url: "../HRSetup/GetStaffProfile",
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ StaffNumber: staffnumber }),
            success: function (data) {
                //console.log(data)
                var StaffName = "";

                if (data[0] == undefined || data[0] == null) {
                    StaffName = data.name;
                    StaffNumber = data.employee_number;
                    StaffGrade = data.grade_code;

                    $("#in_StaffName").val("");
                    $("#in_StaffNumber").val("");
                    $("#in_StaffGrade").val("");

                    $(".web-err").html(StaffNumber + "_" + StaffName);

                } else {
                    StaffName = data[0].name;
                    StaffNumber = data[0].employee_number;
                    StaffGrade = data[0].grade_code;

                    $("#in_StaffName").val(StaffName);
                    $("#in_StaffNumber").val(StaffNumber);
                    $("#in_StaffGrade").val(StaffGrade);

                    $(".web-err").html("");
                }

            }, error: function (data) {
                //console.log(data)
            }
        });
    }
    function addCommas(str) {
        return (str + "").replace(/\b(\d+)((\.\d+)*)\b/g, function (a, b, c) {
            return (b.charAt(0) > 0 && !(c || ".").lastIndexOf(".") ? b.replace(/(\d)(?=(\d{3})+$)/g, "$1,") : b) + c;
        });
    }
    function roundNumber(number) {
        var newnumber = new Number(number + '').toFixed(parseInt(2));
        return new String(parseFloat(newnumber));
    }
    function removeCommas(e) {
        if (!e || e == "") { return false; }
        while (e.indexOf(",") > -1) { e = e.replace(",", ""); }
        return e;
    }*/
});

//validate the new staff number
//getStaffProfile('20071488')
/*
function getStaffProfile(staffnumber) {
    //alert()
//    alert(staffnumber)
    var StaffNumber = "";
    var StaffName = "";
    var StaffGrade = "";

    $.ajax({
        type: "POST",
        url: "../HRSetup/GetStaffProfile",
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ StaffNumber: staffnumber }),
        success: function (data) {
            //console.log(data)
            var StaffName = "";

            if (data[0] == undefined || data[0] == null) {
                StaffName = data.name;
                StaffNumber = data.employee_number;
                StaffGrade = data.grade_code;

                $("#in_StaffName").val("");
                $("#in_StaffNumber").val("");
                $("#in_StaffGrade").val("");

                $(".web-err").html(StaffNumber + "_" + StaffName);

            } else {
                StaffName = data[0].name;
                StaffNumber = data[0].employee_number;
                StaffGrade = data[0].grade_code;

                $("#in_StaffName").val(StaffName);
                $("#in_StaffNumber").val(StaffNumber);
                $("#in_StaffGrade").val(StaffGrade);

                $(".web-err").html("");
            }

        }, error: function (data) {
            //console.log(data)
        }
    });
}
function addCommas(str) {
    return (str + "").replace(/\b(\d+)((\.\d+)*)\b/g, function (a, b, c) {
        return (b.charAt(0) > 0 && !(c || ".").lastIndexOf(".") ? b.replace(/(\d)(?=(\d{3})+$)/g, "$1,") : b) + c;
    });
}
function roundNumber(number) {
    var newnumber = new Number(number + '').toFixed(parseInt(2));
    return new String(parseFloat(newnumber));
}
function removeCommas(e) {
    if (!e || e == "") { return false; }
    while (e.indexOf(",") > -1) { e = e.replace(",", ""); }
    return e;
}*/
