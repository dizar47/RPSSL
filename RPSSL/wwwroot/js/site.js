var choicesDict;

function getChoiceNameById(id) {
    return choicesDict.find(x => x.id === id).name;
}

function getPlayerName() {
    return localStorage.getItem('RPSSL_userName');
}

function setPlayerName(value) {
    return localStorage.setItem('RPSSL_userName', value);
}

function refreshPlayerName() {
    let playerName = getPlayerName();

    if (playerName == undefined) {
        playerName = 'Player';
        setPlayerName(playerName);
    }

    $('#playerName span').text(playerName);
    $('#playerEditValue').val(playerName);
}

function saveEditedName() {
    setPlayerName($('#playerEditValue').val());
    hideEditNameWindow();
    refreshPlayerName();
}

function discardEditedNameChaneges() {
    hideEditNameWindow();
    refreshPlayerName();
}

function hideEditNameWindow() {
    $('#playerName').show();
    $('#playerEdit').hide();
}

function showAlert(text, type) {
    let div = $('<div>')
        .addClass('alert')
        .addClass(type)
        .attr('role', 'alert')
        .attr('style', 'display: none')
        .html(text);

    $('#history').prepend(div);

    div.fadeTo(2000, 500)
        .fadeOut(500, function () {
            div.remove();
        });
}

function addRecordIntoTheHistory(text, type) {
    showAlert(text, type);
}

function showError(text) {
    showAlert(text, 'alert-danger');
}

$(document).ready(function () {

    refreshPlayerName();

    $(document)
        .off('click', '#playerName')
        .on('click', '#playerName', function () {
            $('#playerName').hide();
            $('#playerEdit').show();
        })
        .off('click', '#choicesList > button')
        .on('click', '#choicesList > button', function () {
            let el = $(this);

            $.ajax({
                url: "/play",
                type: "POST",
                data: JSON.stringify({ Player: parseInt(el.attr('data-choice-id')), PlayerName: getPlayerName() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.isSuccessful) {
                        let cc = $('#computerChoice > div');
                        let pc = $('#playerChoice > div');
                        let computerChoiceName = getChoiceNameById(response.data.computer);
                        let playerChoiceName = getChoiceNameById(response.data.player);

                        if (response.data.results === 'win') {
                            cc.removeClass().addClass('lose');
                            pc.removeClass().addClass('win');
                            addRecordIntoTheHistory('<b>You Win !</b> ' + playerChoiceName + ' beat ' + computerChoiceName, 'alert-success');
                        }
                        else if (response.data.results === 'lose') {
                            cc.removeClass().addClass('win');
                            pc.removeClass().addClass('lose');
                            addRecordIntoTheHistory('<b>You Lose !</b> ' + computerChoiceName + ' beat ' + playerChoiceName , 'alert-danger');
                        }
                        else {
                            cc.removeClass().addClass('tie');
                            pc.removeClass().addClass('tie');
                            addRecordIntoTheHistory('<b>Tie</b> ! Both chose ' + playerChoiceName, 'alert-secondary');
                        }

                        cc.text(computerChoiceName);
                        pc.text(playerChoiceName);
                    }
                    else {
                        showError(response.error.message);
                    }
                },
                error: function (xhr, status, error) {
                    showError('Something went wrong.');
                }
            })
        });

    $.ajax({
        url: "/choices",
        type: "GET",
        success: function (response) {
            if (response.isSuccessful) {
                choicesDict = response.data;
                $('#choicesList')
                    .empty()
                    .append(
                        response
                            .data
                            .map(x => $('<button type="button">')
                                .addClass('btn btn-primary btn-lg')
                                .attr('data-choice-id', x.id)
                                .text(x.name)
                            )
                    );
            }
            else {
                showError(response.error.message);
            }
        },
        error: function (xhr, status, error) {
            showError('Something went wrong.');
        }
    });
})