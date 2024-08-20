var studentsWithAccess = document.getElementById('studentsWithAccess');
var studentsWithAccessInitial = [];

for (const option of studentsWithAccess.options) {
    studentsWithAccessInitial.push(option.value);
}

document.addEventListener('DOMContentLoaded', function () {
    // Grant access functionality
    var studentsWithNoAccess = document.getElementById('studentsWithNoAccess');

    var grantedAccess = document.getElementById('grantedAccess');
    var submitFormBtn = document.getElementById('submitFormBtn');
    var ungrantedAccess = false;

    studentsWithNoAccess.addEventListener('click', function (event) {
        if (event.target.tagName === 'OPTION') {
            moveStudent(studentsWithNoAccess, grantedAccess, event.target);

            if (grantedAccess.options.length > 0 && !ungrantedAccess && submitFormBtn.disabled) {
                submitFormBtn.disabled = false;
            }
        }
    });

    studentsWithAccess.addEventListener('click', function (event) {
        if (event.target.tagName === 'OPTION') {
            moveStudent(studentsWithAccess, studentsWithNoAccess, event.target);
            ungrantedAccess = true;

            if (submitFormBtn.disabled) {
                submitFormBtn.disabled = false;
            }
        }
    });

    grantedAccess.addEventListener('click', function (event) {
        if (event.target.tagName === 'OPTION') {
            moveStudent(grantedAccess, studentsWithNoAccess, event.target);
        }

        if (grantedAccess.options.length == 0 && !ungrantedAccess && !submitFormBtn.disabled) {
            submitFormBtn.disabled = true;
        }
    });

    function moveStudent(sourceSelect, targetSelect, student) {
        targetSelect.appendChild(student.cloneNode(true));
        sourceSelect.removeChild(student);
    }

    // Search functionality
    var searchInput = document.getElementById('searchInput');

    searchInput.addEventListener('input', function () {
        var allOptions = Array.from(studentsWithNoAccess.options);
        var searchValue = searchInput.value.toLowerCase();

        allOptions.forEach(function (option) {
            var text = option.text.toLowerCase();
            if (text.includes(searchValue)) {
                option.style.display = '';
            } else {
                option.style.display = 'none';
            }
        });

    });
});

// Grant Access form submission event listener
function handleSubmit() {
    var grantedStudentIds = document.getElementById('GrantedStudentIds');
    var ungrantedStudentIds = document.getElementById('UnGrantedStudentIds');
    var grantedAccess = document.getElementById('grantedAccess');

    for (var i = 0; i < grantedAccess.options.length; i++) {
        var id = grantedAccess.options[i].value;
        grantedStudentIds.value += id + ",";
    }

    for (var studentId of studentsWithAccessInitial) {
        var studentUngranted = true;

        for (var option of studentsWithAccess.options) {
            if (studentId == option.value) {
                studentUngranted = false;
                break;
            }
        }

        if (studentUngranted) {
            ungrantedStudentIds.value += studentId + ",";
        }
    }

    return true;
}