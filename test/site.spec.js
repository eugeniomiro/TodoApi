'use strict';

// running on node?
if (typeof require !== 'undefined') {
    var sinon = require('sinon');
    var assert = require('chai').assert;
    var { todoes, getData, getDataSuccess, closeInput, updateCount } = require("../js/site");
} else {
    todoes = () => todos;
}
const sandbox = sinon.createSandbox();
const todoApi = 'api/todo'

describe('getData()', function() {

    before(function() {
        sandbox.spy(jQuery, "ajax");
        getData();
    });
    it('should execute ajax method once', function() {
        assert(jQuery.ajax.calledOnce);
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
