$(document).ready(function () {

    $("#prova").click(function () {
        $(".overLay,.popup").fadeIn();

        StartProgressBar();
        CallApi;
    });

    //progress bar function

    function StartProgressBar() {

        var currentDate = new Date();
        var second = currentDate.getSeconds();
        if (second < 10) {
            second = "0" + second;
        }

        $(".progress-bar").css('width', second + '%');
        const fileField = document.getElementById('tableID');
        if (fileField != null) {
            setTimeout(function () { StartProgressBar() }, 10);
        }
        else {
            setTimeout(function () { StartProgressBar() }, 20);
        }
        


    }

    //Close Progress Bar

    function CloseProgressBar() {
        $("#Fade_area").removeAttr("style");
        $("#myModal").removeAttr("style");


    }



    function CallApi() {

        $.ajax({
            url: "",
            type: 'post',
            contentType: "application/json",
            success: function () {
                setTimeout(function () { CloseProgressBar() }, 10);
            },
            error: function () { },
        })
    }




})