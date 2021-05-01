// @ts-check
// Protractor configuration file, see link for more information
// https://github.com/angular/protractor/blob/master/lib/config.ts

// see: https://gist.github.com/arranbartish/defc43ae628af01d13e68c85aef38ce3

exports.config = {
    allScriptsTimeout: 11000,
    specs: [
        './e2e/**/*.e2e-spec.ts'
    ],
    baseUrl: 'http://localhost:4200/',
    capabilities: {
        browserName: 'chrome'
    },
    directConnect: true,
    framework: 'mocha',
    mochaOpts: {
        reoirter: "spec",
        slow: 3000,
        ui: 'bdd',
        timeout: 30000
    },
    beforeLaunch: function () {
        require('ts-node').register({
            project: require('path').join(__dirname, './tsconfig.json')
        })
    },
    onPrepare: function () {
        var chai = require("chai");
        var chaiAsPromised = require("chai-as-promised");
        chai.use(chaiAsPromised);
        global.Chai = chai;
    }
};