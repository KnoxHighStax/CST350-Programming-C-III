$(document).ready(function () {
    initializeGameHandlers();
});
function initializeGameHandlers() {
    // AJAX - Partail view updates for lft and right click events
    // Handle left-click to reveal the cell
    $('.game-cell-container').on('click', function (e) {
        e.preventDefault();

        // Check if cell is flagged to prevent left-click on flagged cells
        if ($(this).find('.cell-flag').length > 0) {
            // Stops that click from happens
            return false;
        }

        var row = $(this).data('row');
        var col = $(this).data('col');

        // AJAX - To Sned click coordinates to the server
        $.post('/Game/CellClick', { row: row, col: col })
            .done(function (data) {
                handleAjaxResponse(data, row, col);
            })
            .fail(function () {
                alert('Error processing cell click');
            });
    });

    // Handle right-click to toggle flag
    $('.game-cell-container').on('contextmenu', function (e) {
        e.preventDefault();
        var row = $(this).data('row');
        var col = $(this).data('col');

        $.post('/Game/ToggleFlag', { row: row, col: col })
            .done(function (data) {
                handleAjaxResponse(data, row, col);
            })
            .fail(function () {
                alert('Error toggling flag');
            });

        // Prevent default browser context menu
        return false;
    });
}

function handleAjaxResponse(data, row, col) {
    if (data.redirect) {
        window.location.href = data.redirect;
    } else {
        // Update the specific cell
        $(`[data-row="${row}"][data-col="${col}"]`).html(data);
        // Update game info with timestamp
        updateGameInfo();
        updateTimestamp();
    }
}

function updateGameInfo() {
    // Fetches and updates the game infor partial view 
    $.get('/Game/GetGameInfo')
        .done(function (data) {
            $('#game-info').html(data);
        })
        .fail(function () {
            console.log('Error updating game info');
        });
}

function updateTimestamp() {
    // Updates the timestamp to visually confirm AJAX updates
    const now = new Date();
    // Format the time to include milliseconds for visually proof for the user
    const timestamp = now.toLocaleTimeString() + '.' + now.getMilliseconds().toString().padStart(3, '0');
    $('#ajax-timestamp').text(timestamp);
}

function startNewGame() {
    window.location.href = '/Home/StartGame';
}

function goToMainMenu() {
    window.location.href = '/Home/Index';
}