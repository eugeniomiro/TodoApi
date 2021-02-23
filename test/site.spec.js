'use strict';

// running on node?
if (typeof require !== 'undefined') {
    var assert = require('chai').assert;
    var { getCount } = require('../js/getCount');
    var { closeInput } = require("../js/closeInput");
}

describe('getCount()', function () {
    const tests = [
        { arg: null, expected: { todo: 'No to-do', display: 'none' } },
        { arg: 0,    expected: { todo: 'No to-do', display: 'none' } },
        { arg: 1,    expected: { todo: '1 to-do',  display: 'table' } },
        { arg: 2,    expected: { todo: '2 to-dos', display: 'table' } },
    ];
    tests.forEach(({ arg, expected }) => {
        describe(`calling getCount(${arg === null ? '' : arg})`, () => {
            let p;
            let table;

            before(function () {
                p = $("<p id='counter'>");
                table = $("<table id='todo-list'>");

                $(document.body)
                    .append(p)
                    .append(table);
                getCount(arg);
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
