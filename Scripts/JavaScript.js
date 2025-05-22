//line-through on play page
$("#mustardChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#scarletChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#greenChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#peacockChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#whiteChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#plumChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#candlestickChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#knifeChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#leadPipeChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#revolverChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#ropeChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#wrenchChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#ballRoomChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#billiardRoomChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#conservatoryChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#diningRoomChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#hallChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#kitchenChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#libraryChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#loungeChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#studyChecked").click(function () {
    $(this).toggleClass("line-through");
});

$("#newGame").click(function () {
    clearForm();
});

//clearing the page for a new game
function clearForm() {
    document.getElementById("playForm").reset();
    $(".lineThroughItem").removeClass("line-through");
}

//saving the selection from final guess
$(document).ready(function () {
    $("#saveSelection").click(function () {
        const guessPerson = $("#guessSuspectedPersonOptionPlay").val();
        const guessWeapon = $("#guessProbableWeaponOptionPlay").val();
        const guessScene = $("#guessSuspectedScenesOfMurderOptionPlay").val();

        localStorage.setItem("guessPerson", guessPerson);
        localStorage.setItem("guessWeapon", guessWeapon);
        localStorage.setItem("guessScene", guessScene);
    });
});

//getting the selection from local storage
$(document).ready(function () {
    const guessPerson = localStorage.getItem("guessPerson");
    const guessWeapon = localStorage.getItem("guessWeapon");
    const guessScene = localStorage.getItem("guessScene");

    if (guessPerson) {
        $("#guessSuspectedPersonOptionEnvelope").val(guessPerson);
    }
    if (guessWeapon) {
        $("#guessProbableWeaponOptionEnvelope").val(guessWeapon);
    }
    if (guessScene) {
        $("#guessSuspectedScenesOfMurderOptionEnvelope").val(guessScene);
    }
});

//comparing guess with the envelope contents
$(document).ready(function () {
    $("#envelope").click(function () {
        compareSelections();
    });
});

function compareSelections() {
    const guessPerson = $("#guessSuspectedPersonOptionEnvelope").val();
    const guessWeapon = $("#guessProbableWeaponOptionEnvelope").val();
    const guessScene = $("#guessSuspectedScenesOfMurderOptionEnvelope").val();

    const answerPerson = $("#answerSuspectedPersonOption").val();
    const answerWeapon = $("#answerProbableWeaponOption").val();
    const answerScene = $("#answerSuspectedScenesOfMurderOption").val();

    let message = '';
    if (guessPerson === answerPerson && guessWeapon === answerWeapon && guessScene === answerScene) {
        message = "You solved the murder mystery!<br>You can now continue with a New Game!";
        confetti({
            particleCount: 200,
            spread: 150,
            origin: { y: 0.6 }
        });
    } else {
        message = "Your guess is incorrect.<br>You cannot continue playing until there is a winner.";
    }

    $("#resultMessage").html(message);
}