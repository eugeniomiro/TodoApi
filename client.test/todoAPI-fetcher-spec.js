// todoAPI-fetcher-spec.js
'use strict';

var expect = require('chai').expect;

describe('TodoAPI fetcher', function() {
    it ('should exist', function() {
        var todoApiFetcher = require('../public/js/todoAPI-fetcher.js');
        expect(todoApiFetcher).to.not.be.undefined;
    })
});