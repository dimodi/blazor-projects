document.addEventListener("DOMContentLoaded", function () {
    Blazor.start();
});

function submitSignInForm() {
    var form = document.getElementById("sign-in-form");
    if (form) {
        form.submit();
    }
}
