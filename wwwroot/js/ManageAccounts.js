function approveUser(target, id) {
    target.innerText = "Approved"
    target.disabled = true;
    target.classList.add('approved');

    var userIdsInput = document.getElementById('approvedUserIds');
    userIdsInput.value = userIdsInput.value + id + ',';

    document.getElementById('btnSave').disabled = false;
    document.getElementById('btnClear').disabled = false;
}

function clearApproved() {
    var approved = document.querySelectorAll('.approved');

    approved.forEach(function (element) {
        element.innerText = "Approve";
        element.disabled = false;
    });

    document.getElementById('approvedUserIds').value = '';
    document.getElementById('btnSave').disabled = true;
    document.getElementById('btnClear').disabled = true;
}