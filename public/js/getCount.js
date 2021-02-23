function getCount(data) {
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

    if (data || 0 > 0)
        todoList.show();

    else
        todoList.hide();
}

// If we're running under Node, 
if(typeof exports !== 'undefined') {
    exports.getCount = getCount;
}