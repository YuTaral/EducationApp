function toggleDescription(event) {

    var extraDescription = event.target.parentElement.nextElementSibling;
    var switchIcon = event.target;

    if (extraDescription.classList.contains('open')) {
        switchIcon.innerHTML = "&#43;";
        extraDescription.classList.remove('open');
    } else {
        switchIcon.innerHTML = "&#8722;";
        extraDescription.classList.add('open');
    }
}

var lessons = document.querySelectorAll('.toggle');

if (lessons.length > 0) {

    lessons.forEach(l => {
        l.addEventListener('click', toggleDescription);
    });
}