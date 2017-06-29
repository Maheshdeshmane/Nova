google.maps.event.addDomListener(window, 'load', function () {
    var places = new google.maps.places.Autocomplete(document.getElementById('lookingAt'));
    google.maps.event.addListener(places, 'place_changed', function () {
        var place = places.getPlace();
        document.getElementById('lookingAtLat').value = place.geometry.location.lat();
        document.getElementById('lookingAtLng').value = place.geometry.location.lng();
    });
});

function FillSubCatgory(item) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#selectedSubCategory").html(procemessage).show();
    var url = "/Home/GetSubCatgoryByCategoryId/";

    $.ajax({
        url: url,
        data: { stateid: item.value },
        cache: false,
        type: "POST",
        success: function (data) {
            var markup = "<option value='0'>Sub Category</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
            }
            $("#selectedSubCategory").html(markup).show();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}