const uri = 'api/todo';
let todos = null;

function getData() {
    $.ajax({
        type: 'GET',
        url: uri,
        success: function (data) {
            $('#todos').empty();
            getCount(data.length);
            $.each(data, function (key, item) {
                const checked = item.isComplete ? 'checked' : '';

                $('<tr><td><input disabled="true" type="checkbox" ' + checked + '></td>' +
                    '<td>' + item.name + '</td>' +
                    '<td><button onclick="editItem(' + item.id + ')">Edit</button></td>' +
                    '<td><button onclick="deleteItem(' + item.id + ')">Delete</button></td>' +
                    '</tr>').appendTo($('#todos'));
            });

            todos = data;
        }
    });
}

function addItem() {
    const item = {
        'name': $('#add-name').val(),
        'isComplete': false
    };

    $.ajax({
        type: 'POST',
        accepts: 'application/json',
        url: uri,
        contentType: 'application/json',
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            alert('here');
        },
        success: function (result) {
            getData();
            $('#add-name').val('');
        }
    });
}

function deleteItem(id) {
    $.ajax({
        url: uri + '/' + id,
        type: 'DELETE',
        success: function (result) {
            getData();
        }
    });
}

function editItem(id) {
    $.each(todos, function (key, item) {
        if (item.id === id) {
            $('#edit-name').val(item.name);
            $('#edit-id').val(item.id);
            $('#edit-isComplete')[0].checked = item.isComplete;
        }
    });
    $('#spoiler').css({ 'display': 'block' });
}

function onSubmitForm() {
    const item = {
        'name': $('#edit-name').val(),
        'isComplete': $('#edit-isComplete').is(':checked'),
        'id': new Number($('#edit-id').val())
    };

    $.ajax({
        url: uri + '/' + $('#edit-id').val(),
        type: 'PUT',
        accepts: 'application/json',
        contentType: 'application/json',
        data: JSON.stringify(item),
        success: function (result) {
            getData();
        }
    });

    closeInput();
    return false;
}


function closeInput() {
    $('#spoiler').css({ 'display': 'none' });
}

function addEventHandlers() {
    $('.edit-form').on('submit', onSubmitForm);
    $('.add-form').on('submit', addItem);
    $('.close-spoiler').on('click', closeInput);
}

$(function () {
    addEventHandlers();
    getData();
});

// If we're running under Node, 
if(typeof exports !== 'undefined') {
    exports.closeInput = closeInput;
}