'use strict';

// running on node?
if (typeof require !== 'undefined') {
    global.alert = () => {};
    var sinon = require('sinon');
    var assert = require('chai').assert;
    var { todoes, getData, getDataSuccess, 
          closeInput, updateCount, addItem,
          deleteItem, editItem, setTodos } = require("../js/site");
} else {
    todoes = () => todos;
    setTodos = newTodos => { todos = newTodos }
}
const sandbox = sinon.createSandbox();
const todoApi = 'api/todo';

describe('getData()', function() {
    before(function() {        
        sandbox.stub(jQuery, "ajax");
        getData();
    });
    it('should execute ajax method once', function() {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    it('should have been called with todoApi url', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].url, todoApi);
    });
    it('should have been called with method GET', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].type, 'GET');
    });
    after(function() {
        sandbox.restore();
    });
});

describe('addItem()', function() {
    before(function() {
        sandbox.stub(jQuery, "ajax");
        addItem();
    });
    it('should execute ajax method once', function() {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    it('should have been called with todoApi url', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].url, todoApi);
    });
    it('should have been called with method POST', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].type, 'POST');
    });
    after(function() {
        sandbox.restore();
    });
});

describe('deleteItem()', function() {
    let element;
    before(function() {
        element = $('<div data-id="1">');
        element.on('click', deleteItem);
        sandbox.stub(jQuery, "ajax");
        element.trigger('click');
    });
    it('should execute ajax method once', function() {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    it('should have been called with todoApi url id 1', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].url, todoApi + '/1');
    });
    it('should have been called with method DELETE', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].type, 'DELETE');
    });
    after(function() {
        sandbox.restore();
        element.off('click');
        element = null;
    });
});

describe('editItem()', function() {
    let element;
    before(function() {
        setTodos([{ id: 1, name:'name', isComplete: true }]);
        let spoiler = $("<div id='spoiler' style='display:hidden'>");
        let editName = $("<input type='text' id='edit-name'>");
        let editId = $("<input type='text' id='edit-id'>");
        let editIsComplete = $("<input type='checkbox' id='edit-isComplete'>");
        $(document.body).append(spoiler);
        $(document.body).append(editName);
        $(document.body).append(editId);
        $(document.body).append(editIsComplete);
        element = $('<div data-id="1">');
        element.on('click', editItem);
        element.trigger('click');
    });
    it('should execute show spoiler element', function() {
        assert.equal($('#spoiler').css('display'), 'block');
    });
    it('should have edit-id textbox value set to "1"', function() {
        assert.equal($('#edit-id').val(), '1');
    });
    it('should have edit-name textbox value set to "name"', function() {
        assert.equal($('#edit-name').val(), 'name');
    });
    it('should have edit-isComplete checkbox checked', function() {
        assert.equal($('#edit-isComplete').prop('checked'), true);
    });
    after(function() {
        sandbox.restore();
        element.off('click');
        element = null;
    });
});

describe('getDataSuccess()', function() {
    const tests = [
        { data: [], expectedTodosLength: 0 },
        { data: [{ id: 1, name: 'me', isComplete: false}], expectedTodosLength: 1 },
        { data: [{ id: 1, name: 'me', isComplete: false},
                 { id: 2, name: 'you', isComplete: true}], expectedTodosLength: 2 }
    ];
    tests.forEach(test => {
        describe(`with data = ${JSON.stringify(test.data)}`, function() {
            before(function() {
                $(document.body).append("<tbody id='todos'>");
                getDataSuccess(test.data, updateCount);
            });
            it('should update todos variable', function() {
                assert.isNotNull(todoes());
            });
            it(`should have todos length equal ${test.expectedTodosLength}`, function() {
                assert.equal(todoes().length, test.expectedTodosLength);
            });
            it(`should add ${test.expectedTodosLength} tr(s) to #todos`, () => {
                assert.equal($("#todos").find("tr").length, test.expectedTodosLength);
            });
            after(function() {
            });
        });
    });
});

describe('updateCount()', function () {
    const tests = [
        { arg: null, expected: { todo: 'No to-do', display: 'none'  } },
        { arg: 0,    expected: { todo: 'No to-do', display: 'none'  } },
        { arg: 1,    expected: { todo: '1 to-do',  display: 'table' } },
        { arg: 2,    expected: { todo: '2 to-dos', display: 'table' } },
    ];
    tests.forEach(({ arg, expected }) => {
        describe(`calling updateCount(${arg === null ? '' : arg})`, () => {
            let p;
            let table;

            before(function () {
                p = $("<p id='counter'>");
                table = $("<table id='todo-list'>");

                $(document.body)
                    .append(p)
                    .append(table);
                updateCount(arg);
            });

            it(`should display '${expected.todo}'`, function () {
                assert.equal(p.text(), expected.todo);
            });
            it(`should display todo list as ${expected.display}`, function () {
                assert.equal(table.css('display'), expected.display);
            });

            after(function () {
                p.remove();
                p = null;
                table.remove();
                table = null;
            });
        });
    });
});

describe('closeInput()', function() {
    let spoiler;
    before(function() {
        spoiler = $("<div id='spoiler'>");
        $(document.body)
            .append(spoiler);
    });
    it('leaves the spoiler element hidden', function() {
        assert.equal($("#spoiler").css('display'), 'block');
        closeInput();
        assert.equal($("#spoiler").css('display'), 'none');
    });

    after(function() {
        spoiler.remove();
        spoiler = null;
    });
});
