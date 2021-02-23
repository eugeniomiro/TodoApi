function closeInput() {
    $('#spoiler').css({ 'display': 'none' });
}

// If we're running under Node, 
if(typeof exports !== 'undefined') {
    exports.closeInput = closeInput;
}
