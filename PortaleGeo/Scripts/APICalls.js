$(document).ready(function () {

    $("#Upload").click(function () {
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
        const fileField = document.getElementById('Geo');
        
        if (fileField != null) {
            setTimeout(function () { StartProgressBar() }, 500);
        }
        else {
            setTimeout(function () { StartProgressBar() }, 20000);
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
                setTimeout(function () { CloseProgressBar() }, 10000);
            },
            error: function () { },
        })
    }




})