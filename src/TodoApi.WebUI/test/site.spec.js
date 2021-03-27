'use strict';

let expectedExceptionGetDataSucceeded;

// running on node?
if (typeof require !== 'undefined') {
    global.alert = function() {};
    var sinon = require('sinon');
    var assert = require('chai').assert;
    var site = require("../js/site");
    var get_Todos = site.get_Todos,
        set_Todos = site.set_Todos,
        getDataSucceeded = site.getDataSucceeded,
        getData = site.getData,
        getDataSuccess = site.getDataSuccess,
        addItem = site.addItem,
        addItemSucceeded = site.addItemSucceeded,
        addItemFailed = site.addItemFailed,
        deleteItem = site.deleteItem,
        editItem = site.editItem,
        updateCount = site.updateCount,
        closeInput = site.closeInput,
        onSubmitForm = site.onSubmitForm,
        onReady = site.onReady;
        expectedExceptionGetDataSucceeded = "Cannot read property \'length\' of undefined";
} else {
    get_Todos = function() { return todos };
    set_Todos = function(newTodos) { todos = newTodos };
    expectedExceptionGetDataSucceeded = 'data is undefined';
}
const sandbox = sinon.createSandbox();
const todoApi = 'api/todo';

describe('getDataSucceeded()', function() {
    it('can not be executed with null parameter, it throws "TypeError: ' + expectedExceptionGetDataSucceeded + '"', function() {
        assert.throws(function() { getDataSucceeded(); }, TypeError, expectedExceptionGetDataSucceeded);
    });
    it("can be created with an object as paraemter", function() {
        getDataSucceeded({});
    });
});

describe('getDataSuccess()', function () {
    const tests = [
        { data: [], expectedTodosLength: 0 },
        { data: [{ id: 1, name: 'me', isComplete: false}], expectedTodosLength: 1 },
        { data: [{ id: 1, name: 'me', isComplete: false},
                 { id: 2, name: 'you', isComplete: true}], expectedTodosLength: 2 }
    ];
    tests.forEach(function(test) {
        describe('with data = ' + JSON.stringify(test.data), function() {
            before(function() {
                $(document.body).append("<tbody id='todos'>");
                getDataSuccess(test.data, updateCount);
            });
            it('should update todos variable', function() {
                assert.isNotNull(get_Todos());
            });
            it('should have todos length equal ' + test.expectedTodosLength, function() {
                assert.equal(get_Todos().length, test.expectedTodosLength);
            });
            it('should add ' + test.expectedTodosLength + ' tr(s) to #todos', function() {
                assert.equal($("#todos").find("tr").length, test.expectedTodosLength);
            });
            after(function() {
            });
        });
    });
});

describe('getData()', function () {
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

describe('addItem()', function () {

    beforeEach(function () {
        sandbox.stub(jQuery, "ajax");
        addItem();
    });
    it('should execute ajax method once', function () {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    it('should have been called with todoApi url', function () {
        assert.equal(jQuery.ajax.getCall(0).args[0].url, todoApi);
    });
    it('should have been called with method POST', function () {
        assert.equal(jQuery.ajax.getCall(0).args[0].type, 'POST');
    });
    it('should have been called with accepts "application/json"', function () {
        assert.equal(jQuery.ajax.getCall(0).args[0].accepts, 'application/json');
    });
    it('should have been called with contentType "application/json"', function () {
        assert.equal(jQuery.ajax.getCall(0).args[0].contentType, 'application/json');
    });
    afterEach(function () {
        sandbox.restore();
    });
});

describe("addItemSucceeded()", function () {
    beforeEach(function () {
        sandbox.stub(jQuery, "ajax");
        addItemSucceeded();
    });
    it("should execute getData method once", function () {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    afterEach(function () {
        sandbox.restore();
    });
});

describe('addItemFailed()', function () {
    it('can be executed', function () {
        addItemFailed();
    });
})

describe('deleteItem()', function () {
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

describe('editItem()', function () {
    let element;
    let spoiler;
    let editName;
    let editId;
    let editIsComplete;
    before(function() {
        set_Todos([{ id: 1, name:'name', isComplete: true }]);
        spoiler = $("<div id='spoiler' style='display:hidden'>");
        editName = $("<input type='text' id='edit-name'>");
        editId = $("<input type='text' id='edit-id'>");
        editIsComplete = $("<input type='checkbox' id='edit-isComplete'>");
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
        set_Todos(null);
        editIsComplete.remove();
        editName.remove();
        editId.remove();
        spoiler.remove();
        sandbox.restore();
        element.off('click');
        element = null;
    });
});

describe('updateCount()', function () {
    const tests = [
        { arg: null, expected: { todo: 'No to-do', display: 'none'  } },
        { arg: 0,    expected: { todo: 'No to-do', display: 'none'  } },
        { arg: 1,    expected: { todo: '1 to-do',  display: 'table' } },
        { arg: 2,    expected: { todo: '2 to-dos', display: 'table' } },
    ];
    tests.forEach(function(data) {
        describe('calling updateCount(' + data.arg === null ? '' : data.arg + ')', function() {
            let p;
            let table;

            before(function () {
                p = $("<p id='counter'>");
                table = $("<table id='todo-list'>");

                $(document.body)
                    .append(p)
                    .append(table);
                updateCount(data.arg);
            });

            it('should display "' + data.expected.todo + '"', function () {
                assert.equal(p.text(), data.expected.todo);
            });
            it('should display todo list as ' + data.expected.display, function () {
                assert.equal(table.css('display'), data.expected.display);
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

describe('closeInput()', function () {
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

describe('onSubmitForm()', function () {
    var result;
    let editId;
    before(function() {
        editId = $("<input type='text' id='edit-id' value='1'>");
        $(document.body).append(editId);
        sandbox.stub(jQuery, "ajax");
        result = onSubmitForm();
    });
    it('should execute ajax method once', function() {
        assert.isTrue(jQuery.ajax.calledOnce);
    });
    it('should have been called with todoApi url id 1', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].url, todoApi + '/1');
    });
    it('should have been called with method PUT', function() {
        assert.equal(jQuery.ajax.getCall(0).args[0].type, 'PUT');
    });
    it('should return false', function () {
        assert.isFalse(result);
    })
    after(function() {
        editId.remove();
        sandbox.restore();
    });
});

describe('onReady()', function () {
    let jqueryfunction;
    beforeEach(function() {
        jqueryfunction = $;
        $ = sandbox.stub().returns({ 
            on: function() {}
        });
        $.ajax = sandbox.stub();
        onReady();
    });
    afterEach(function() {
        $ = jqueryfunction;
        sandbox.restore();
    });
    it('should execute $() three times and $.ajax one', function() {
        assert.equal(3, $.callCount);
        assert.isTrue($.ajax.calledOnce);
    });
});
