'use strict';

describe('calling getCount with no argument', function () {
    var p;
    var table;

    before(function () {
        p = $("<p id='counter'>");
        table = $("<table id='todo-list'>");
        $(document.body).append(p)
            .append(table);
        getCount();
    });

    it('should display \'No to-do\'', function () {
        assert.equal(p.text(), 'No to-do');
    });
    it('should show todo list', function () {
        assert.equal(table.css('display'), 'none');
    });

    after(function () {
        p.remove();
        p = null;
        table.remove();
        table = null;
    });
});

describe('calling getCount with argument 0', function () {
    var p;
    var table;

    before(function () {
        p = $("<p id='counter'>");
        table = $("<table id='todo-list'>");
        $(document.body).append(p)
            .append(table);
        getCount(0);
    });

    it('should display \'No to-do\'', function () {
        assert.equal(p.text(), 'No to-do');
    });
    it('should hide todo list', function () {
        assert.equal(table.css('display'), 'none');
    });

    after(function () {
        p.remove();
        p = null;
        table.remove();
        table = null;
    });
});

describe('calling getCount with argument 1', function () {
    var p;
    var table;

    before(function () {
        p = $("<p id='counter'>");
        table = $("<table id='todo-list'>");
        $(document.body).append(p)
            .append(table);
        getCount(1);
    });

    it('should display \'1 to-do\'', function () {
        assert.equal(p.text(), '1 to-do');
    });
    it('should show todo list', function () {
        assert.equal(table.css('display'), 'table');
    });

    after(function () {
        p.remove();
        p = null;
        table.remove();
        table = null;
    });
});

describe('calling getCount with argument 2', function () {
    var p;
    var table;

    before(function () {
        p = $("<p id='counter'>");
        table = $("<table id='todo-list'>");
        $(document.body).append(p)
            .append(table);
        getCount(2);
    });

    it('should display \'2 to-dos\'', function () {
        assert.equal(p.text(), '2 to-dos');
    });
    it('should show todo list', function () {
        assert.equal(table.css('display'), 'table');
    });

    after(function () {
        p.remove();
        p = null;
        table.remove();
        table = null;
    });
});
