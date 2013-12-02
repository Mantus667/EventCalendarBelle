$(document).ready(function () {
    alert("wuhu");
    $('#colorSelector').ColorPicker({
        color: '#0000ff',
        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#color_field').val('#' + hex);
            $('#colorSelector div').css('background-color', '#' + hex);
        }
    });
});