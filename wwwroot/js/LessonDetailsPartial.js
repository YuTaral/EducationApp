function attachListeners() {
    var modal = document.getElementById('uploadModal');
    var btn = document.getElementById('openUploadPopup');
    var closeBtn = document.getElementById('cancelBtn');
    var closeDialogBtn = document.getElementById('closeResults');

    btn.addEventListener('click', (e) => {
        modal.style.display = 'block';
    });

    closeBtn.addEventListener('click', (e) => {
        e.preventDefault();
        modal.style.display = 'none';
    });

    window.addEventListener('click', (e) => {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    });

    var fileInput = document.getElementById('fileInput');
    var uploadBtn = document.getElementById('uploadBtn');

    uploadBtn.addEventListener('click', (e) => {
        var selectedFile = fileInput.files[0];
        console.log('File selected:', selectedFile);
        modal.style.display = 'none';
    });


    if (closeDialogBtn != null) {
        closeDialogBtn.addEventListener('click', function () {
            document.getElementById('modalResults').style.display = 'none';
        });
    }
        
}

attachListeners();