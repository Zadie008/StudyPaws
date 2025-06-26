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

function updateTimeAndDate() {
    const timeLabel = document.getElementById("headerContentPlaceHolder_lblTime");
    const dayLabel = document.getElementById("headerContentPlaceHolder_lblDay");
    const dateLabel = document.getElementById("headerContentPlaceHolder_lblDate");

    const now = new Date();

    // format time as hh:mm
    let hours = now.getHours();
    let minutes = now.getMinutes();
    hours = hours < 10 ? "0" + hours : hours;
    minutes = minutes < 10 ? "0" + minutes : minutes;
    const timeString = `${hours}:${minutes}`;

    // format day and date
    const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    const months = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const dayString = days[now.getDay()];
    const dateString = `${now.getDate()} ${months[now.getMonth()]}`;

    if (timeLabel) timeLabel.textContent = timeString;
    if (dayLabel) dayLabel.textContent = dayString;
    if (dateLabel) dateLabel.textContent = dateString;
}

window.addEventListener("DOMContentLoaded", updateTimeAndDate);

// update time every second
setInterval(updateTimeAndDate, 6000);
function showPopup() {
    document.getElementById("popup").style.display = "flex";
}

function hidePopup() {
    document.getElementById("popup").style.display = "hide";
}