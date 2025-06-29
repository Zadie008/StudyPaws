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

// date and time in top right corner of header
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

// pop-ups
function showPopup() {
    document.getElementById("popup").style.display = "flex";
}

function hidePopup() {
    document.getElementById("popup").style.display = "hide";
}

// drop down list arrows
document.addEventListener('DOMContentLoaded', function () {
    const dropdown = document.getElementById('dropdownTag');

    if (!dropdown) {
        console.error("Dropdown not found");
        return;
    }

    console.log("Dropdown found:", dropdown);

    let isOpen = false;

    dropdown.addEventListener('mousedown', () => {
        isOpen = !isOpen;
        console.log("mousedown triggered, isOpen =", isOpen);
        dropdown.classList.toggle('open', isOpen);
    });

    dropdown.addEventListener('blur', () => {
        isOpen = false;
        console.log("blur triggered");
        dropdown.classList.remove('open');
    });

    dropdown.addEventListener('change', () => {
        isOpen = false;
        console.log("change triggered");
        dropdown.classList.remove('open');
    });
});

// timer & study session textboxes
document.addEventListener('DOMContentLoaded', function () {
    const timerInputs = document.querySelectorAll('.timerInput');

    timerInputs.forEach(function (input) {
        input.addEventListener('blur', function () {
            let value = input.value.trim();

            if (value === '') {
                input.value = '00';
            } else if (!isNaN(value)) {
                let num = parseInt(value, 10);
                input.value = num < 10 ? '0' + num : num.toString();
            } else {
                input.value = '00';
            }
        });
    });
});

function validateMinTime(source, args) {
    const hours = parseInt(document.getElementById('txtTimeHours').value) || 0;
    const minutes = parseInt(document.getElementById('txtTimeMinutes').value) || 0;
    const seconds = parseInt(document.getElementById('txtTimeSeconds').value) || 0;

    const totalSeconds = (hours * 3600) + (minutes * 60) + seconds;
    args.IsValid = totalSeconds >= 60;
}

document.addEventListener('DOMContentLoaded', function () {
    const inputs = [
        document.getElementById('txtTimeHours'),
        document.getElementById('txtTimeMinutes'),
        document.getElementById('txtTimeSeconds')
    ];

    inputs.forEach(input => {
        input.addEventListener('input', function () {
            Page_ClientValidate('timerValidation');
        });
    });
});

// timer & study session countdown
function formatTime(totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return String(minutes).padStart(2, '0') + ":" + String(seconds).padStart(2, '0');
}