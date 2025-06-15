// entry animation of navigation on default page
window.addEventListener('DOMContentLoaded', () => {
    if (document.body.classList.contains('is-default')) {
        document.body.classList.add('page-enter');
        setTimeout(() => {
            document.body.classList.remove('page-enter');
        }, 600);
    }
});

// exit animation when navigation item is clicked
document.querySelectorAll('.navbar a').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();
        document.body.classList.add('page-exit');
        const url = this.href;
        setTimeout(() => {
            window.location.href = url;
        }, 500);
    });
});