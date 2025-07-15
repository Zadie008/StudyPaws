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
    document.getElementById("popup").style.display = "none";
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

let remainingTime = typeof initialTime !== 'undefined' ? initialTime : 0;

function formatFullTime(seconds) {
    const hrs = Math.floor(seconds / 3600);
    const mins = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${String(hrs).padStart(2, '0')}:${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;
}

window.addEventListener('DOMContentLoaded', function () {
    const startSound = document.getElementById('timerStartSound');
    if (startSound) {
        startSound.play().catch(err => {
            console.warn("Start sound not played automatically:", err);
        });
    }

    animateDonut();
});

function animateDonut() {
    const ring = document.querySelector(".progress-ring-fill");
    if (!ring) return;

    const radius = 210;
    const circumference = 2 * Math.PI * radius;

    let startTime = null;

    function animate(timestamp) {
        if (!startTime) startTime = timestamp;
        const progress = Math.min((timestamp - startTime) / 1500, 1); // 1.5 seconds

        const offset = circumference * progress;
        ring.style.strokeDasharray = circumference;
        ring.style.strokeDashoffset = offset;

        if (progress < 1) {
            requestAnimationFrame(animate);
        } else {
            countdownInterval = setInterval(updateCountdown, 1000); // every second after that
        }
    }

    requestAnimationFrame(animate);
}

function updateCountdown() {
    const countdownLabel = document.getElementById("mainContentPlaceHolder_lblCountdown");
    const ring = document.querySelector(".progress-ring-fill");

    if (!countdownLabel || !ring) return;

    countdownLabel.textContent = formatFullTime(remainingTime);

    const progress = remainingTime / initialTime;
    const radius = 210;
    const circumference = 2 * Math.PI * radius;
    const offset = circumference * progress;

    ring.style.strokeDasharray = circumference;
    ring.style.strokeDashoffset = offset;

    if (remainingTime > 0) {
        remainingTime--;
    } else {
        clearInterval(countdownInterval);
        const alarm = document.getElementById("timerEndSound");
        if (alarm) alarm.play();
        showTimeUpPopup();
    }
}

let extraVisible = false;

function toggleExtraButtons() {
    const container = document.getElementById('extraTimeButtons');
    const btn = document.getElementById('mainContentPlaceHolder_btnToggleAddExtra');

    extraVisible = !extraVisible;

    if (extraVisible) {
        container.classList.add('show');
        btn.value = 'Back';
        //document.addEventListener('click', closeExtraOutside);
    } else {
        container.classList.remove('show');
        btn.value = 'Add';
        //document.removeEventListener('click', closeExtraOutside);
    }
    return false;
}

// to take away the +5/+10/+15 buttons when you click somewhere else on the screen (WORK IN PROGRESS)
/*function closeExtraOutside(e) {
    const container = document.getElementById('extraTimeButtons');
    const btn = document.getElementById('mainContentPlaceHolder_btnToggleAddExtra');

    if (!container.contains(e.target) && e.target !== btn) {
        container.style.display = 'none';
        btn.value = 'Add';
        extraVisible = false;
        document.removeEventListener('click', closeExtraOutside);
    }
}*/

function addExtraTime(mins) {
    const addedSeconds = mins * 60;
    initialTime += addedSeconds;
    remainingTime += addedSeconds;
    toggleExtraButtons();

    fetch('A200_View-timer.aspx/UpdateTimerDuration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ addedSeconds: addedSeconds })
    })
    .then(response => response.json())
    .then(data => {
        console.log("Update success:", data);
    })
    .catch(error => {
        console.error("Update failed:", error);
    });
    return false;
}

function stopTimer() {
    showPopup();
    return false; // don't allow postback
}

function confirmStop() {
    hidePopup();
    return true; // allow postback
}

function showTimeUpPopup() {
    document.getElementById("popupTimeUp").style.display = "flex";
}

function hideTimeUpPopup() {
    document.getElementById("popupTimeUp").style.display = "none";
    window.location.href = "Default.aspx";
}

// ---- VIEW PAST TIMER PAGE ---- //
document.addEventListener("DOMContentLoaded", function () {
    var icon = document.getElementById("filterIcon");
    var controls = document.getElementById("filterControls");

    if (icon && controls) {
        icon.addEventListener("click", function () {
            controls.style.display = (controls.style.display === "none" || controls.style.display === "") ? "flex" : "none";
        });
    }
});

// ---- INVENTORY PAGES ---- //
document.addEventListener('DOMContentLoaded', function () {
    const selectButtons = [
        'mainContentPlaceHolder_btnSelect1',
        'mainContentPlaceHolder_btnSelect2',
        'mainContentPlaceHolder_btnSelect3',
        'mainContentPlaceHolder_btnSelect4',
        'mainContentPlaceHolder_btnSelect5'
    ];

    const btnSell = document.getElementById('mainContentPlaceHolder_btnSell');
    const btnEquip = document.getElementById('mainContentPlaceHolder_btnEquip');

    selectButtons.forEach(id => {
        const button = document.getElementById(id);
        if (!button) return;

        button.addEventListener('click', () => {
            const isSelected = button.classList.contains('buttonSelected');

            // unselect all
            selectButtons.forEach(otherId => {
                const otherBtn = document.getElementById(otherId);
                if (otherBtn) otherBtn.classList.remove('buttonSelected');
            });

            if (!isSelected) {
                button.classList.add('buttonSelected');


                const colourNum = button.getAttribute('data-colour');


                document.getElementById('mainContentPlaceHolder_hfSelectedColourNum').value = colourNum;

                btnSell.style.display = 'inline-block';
                btnEquip.style.display = 'inline-block';
            }
            else
            {
                button.classList.remove('buttonSelected');
                document.getElementById('mainContentPlaceHolder_hfSelectedColourNum').value = "";

                btnSell.style.display = 'none';
                btnEquip.style.display = 'none';
            }
        });
    });
});

function playEquipSound(button) {
    const audio = document.getElementById("equipSound");
    if (audio) {
        audio.currentTime = 0;
        audio.play().catch(err => {
            console.warn("Audio play failed:", err);
        });
    }

    const selectedColour = document.getElementById('mainContentPlaceHolder_hfSelectedColourNum').value;
    const circles = document.querySelectorAll('.petCircle');
    circles.forEach(circle => circle.classList.remove('equipped'));

    const selectedCircle = document.getElementById(`mainContentPlaceHolder_circle${selectedColour}`);
    if (selectedCircle) {
        selectedCircle.classList.add('equipped');
    }

    setTimeout(() => {
        __doPostBack(button.name || button.id, '');
    }, 200);
    return false;
}

// OLD playEquipSound method:
/*function playEquipSound(button) {
    const audio = document.getElementById("equipSound");
    // SHOW BACKGROUND CHANGE OF EQUIPPED PET HAPPEN BEFORE FUNCTIONALITY
    const selectedColour = document.getElementById("mainContentPlaceHolder_hfSelectedColourNum").value;

    for (let i = 1; i <= 5; i++) {
        const circle = document.getElementById("circle" + i);
        if (circle) circle.classList.remove("equipped");
    }

    if (selectedColour) {
        const selectedCircle = document.getElementById("circle" + selectedColour);
        if (selectedCircle) selectedCircle.classList.add("equipped");
    }

    if (!audio) return false;

    try {
        audio.currentTime = 0;
        const playPromise = audio.play();

        if (playPromise !== undefined) {
            playPromise.then(() => {
                console.log("Equip sound played");

                // WAIT FOR SOUND TO FINISH BEFORE POSTBACK
                audio.onended = function () {
                    __doPostBack(button.name || button.id, '');
                };
            }).catch((err) => { // PROCEED WITH POSTBACK ANYWAY
                console.warn("Audio play failed:", err);
                __doPostBack(button.name || button.id, '');
            });
        } else {
            __doPostBack(button.name || button.id, '');
        }
    } catch (err) {
        console.warn("Error playing sound:", err);
        __doPostBack(button.name || button.id, '');
    }

    return false; // prevent default submit
}
*/

function showEquipSellButtons(colourNum) {
    document.getElementById('hfSelectedColourNum').value = colourNum;
    document.getElementById('<%= btnEquip.ClientID %>').style.display = 'inline-block';
    document.getElementById('<%= btnSell.ClientID %>').style.display = 'inline-block';
}

function sellPet() {
    const colourNum = document.getElementById('mainContentPlaceHolder_hfSelectedColourNum').value;

    __doPostBack('FetchSellPrice', colourNum); // a postback fetch of the sellPrice

    showPopup();
    return false; // don't allow postback
}

function confirmSell() {
    hidePopup();
    return true; // allow postback
}

// ---  REGISTRATION PAGE  ---
if (window.location.pathname.toLowerCase().includes("c100_register.aspx")) {
    let usernameCheckTimeout;

    function checkUsernameAvailability() {
        clearTimeout(usernameCheckTimeout);

        usernameCheckTimeout = setTimeout(function () {
            const usernameInput = document.getElementById(txtUsernameClientID);
            const username = usernameInput ? usernameInput.value : '';
            const lblAvailability = document.getElementById(lblUsernameAvailabilityClientID);

            if (lblAvailability) {
                lblAvailability.innerHTML = '';
                lblAvailability.style.display = 'none';
            }

            if (username.length >= 3) {
                if (typeof PageMethods !== 'undefined' && PageMethods.CheckUsernameExists) {
                    PageMethods.CheckUsernameExists(username, onCheckUsernameSuccess, onCheckUsernameError);
                } else {
                    console.error("PageMethods.CheckUsernameExists is not available.");
                }
            } else if (username.length > 0 && lblAvailability) {
                lblAvailability.innerHTML = 'Username too short.';
                lblAvailability.style.color = 'white';
                lblAvailability.style.display = 'block';
            }
        }, 700);
    }

    function onCheckUsernameSuccess(result) {
        const lblAvailability = document.getElementById(lblUsernameAvailabilityClientID);
        if (lblAvailability) {
            lblAvailability.innerHTML = result ? 'Username already taken.' : '';
            lblAvailability.style.color = result ? 'white' : '';
            lblAvailability.style.display = 'block';
        }
    }

    function onCheckUsernameError(error) {
        const lblAvailability = document.getElementById(lblUsernameAvailabilityClientID);
        if (lblAvailability) {
            lblAvailability.innerHTML = 'Error checking username.';
            lblAvailability.style.color = 'white';
            lblAvailability.style.display = 'block';
        }
        console.error("AJAX Error: ", error);
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (typeof pnlConfirmClientID !== 'undefined') hidePanel(pnlConfirmClientID);
        if (typeof pnlTutClientID !== 'undefined') hidePanel(pnlTutClientID);
        if (typeof pnlProfileExistsClientID !== 'undefined') hidePanel(pnlProfileExistsClientID);

        if (typeof btnUnderstandExistsClientID !== 'undefined') {
            const btnUnderstandExists = document.getElementById(btnUnderstandExistsClientID);
            if (btnUnderstandExists) {
                btnUnderstandExists.onclick = function () {
                    hidePanel(pnlProfileExistsClientID);
                    return false;
                };
            }
        }
    });
}

//adding next task to the to-do list
let taskId = 0;
function addNewTask() {
    const list = document.getElementById('taskList');

    const li = document.createElement('li');
    li.className = 'task';
    li.dataset.id = taskId++;

    const btn = document.createElemtn('button');
    btn.className = 'taskCheckBoxBtn';
    btn.onclick = () => toggleComplete(li);

    const input = document.createElement('input');
    input.type = 'text';
    input.className = 'taskText';
    input.placeholder = 'New Task';
    input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            input.blur();
        }
    });
    input.addEventListener('blur', () => {
        input.readOnly = true;
    });
    input.readOnly = false;
    input.focus();

    const editBtn = document.createElement('button');
    editBtn.className = 'editTaskBtn';
    editBtn.innerHTML = '<img src="Icons/icons8-edit-white-96.png" />';
    editBtn.onclick = () => {
        input.readOnly = false;
        input.focus();
    };

    const deleteBtn = document.createElement('button');
    deleteBtn.className = 'deleteTaskBtn';
    deleteBtn.innerHTML = '<img src="Icons/icons8-delete-white-96.png" />';
    deleteBtn.onclick = () => {
        li.remove();
    };

    li.appendChild(btn);
    li.appendChild(input);
    li.appendChild(editBtn);
    li.appendChild(deleteBtn);

    list.appendChild(li);
}

//completing a task on the to do list
function toggleComplete(task) {
    const list = document.getElementById('taskList');
    task.classList.toggle('completed');

    if (task.classList.contains('completed')) {
        list.appendChild(task);
    } else {
        list.insertBefore(task, list.firstChild);
    }
}