document.addEventListener("DOMContentLoaded", function () {
    var fileInput = document.getElementById("imageInput");
    if (fileInput) {
        fileInput.addEventListener("change", function () {
            var output = document.getElementById("ingPreview");
            if (fileInput.files && fileInput.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    if (e.target && e.target.result) {
                        output.src = e.target.result;
                        output.style.display = "block";
                    }
                };
                reader.readAsDataURL(fileInput.files[0]);
            }
        });
    }
});