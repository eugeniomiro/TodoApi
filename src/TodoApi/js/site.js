'use strict';
const uri = 'api/todo';
let todos = null;

function getData() {
    $.ajax({
        type: 'GET',
        url: uri,
        success: function (data) { getDataSuccess(data, updateCount); }
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
            alert(textStatus);
        },
        success: function (result) {
            getData();
            $('#add-name').val('');
        }
    });
}

function deleteItem() {
    let id = $(this).data('id')
    $.ajax({
        url: uri + '/' + id,
        type: 'DELETE',
        success: getData
    });
}

function editItem() {
    let id = $(this).data('id')
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

function onReady() {
    $('.edit-form').on('submit', onSubmitForm);
    $('.add-form').on('submit', addItem);
    $('.close-spoiler').on('click', closeInput);
    getData();
}

function closeInput() {
    $('#spoiler').css({ 'display': 'none' });
}

function updateCount(data) {
    const el = $('#counter');
    const todoList = $("#todo-list");
    let name = 'to-do';

    if (data) {
        if (data > 1) {
            name = 'to-dos';
        }
        el.text(data + ' ' + name);
    } else {
        el.html('No ' + name);
    }

    if (data || 0 > 0) {
        todoList.show();
    } else {
        todoList.hide();
    }
}

function getDataSuccess(data, updateCount) {
    $('#todos').empty();
    updateCount(data.length);
    $.each(data, function (key, item) {
        let complete = $('<input disabled="true" type="checkbox">');
        $('#todos').append(
            $('<tr>').append($('<td>').append(complete))
                .append($('<td>').text(item.name))
                .append($('<td>').append($('<button>').text('Edit')
                    .data('id', item.id)
                    .on('click', editItem)))
                .append($('<td>').append($('<button>').text('Delete')
                    .data('id', item.id)
                    .on('click', deleteItem)))
        );
        complete.prop('checked', !!item.isComplete);
    });
    todos = data;
}

// If we're running under Node, 
if (typeof exports !== 'undefined') {
    exports.getDataSuccess = getDataSuccess;
    exports.getData = getData;
    exports.addItem = addItem;
    exports.deleteItem = deleteItem;
    exports.editItem = editItem;
    exports.updateCount = updateCount;
    exports.closeInput = closeInput;
    exports.onSubmitForm = onSubmitForm;
    exports.onReady = onReady;
    exports.get_Todos = function() { return todos; };
    exports.set_Todos = function(newTodos) { todos = newTodos; };
}
