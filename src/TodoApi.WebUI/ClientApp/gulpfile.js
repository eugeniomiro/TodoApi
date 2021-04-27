"use strict";

const arg = require('./js/cmdLineArgs')(process.argv);
const { clean, restore, build, test, publish, run } = require("gulp-dotnet-cli");
const { task, series, src, dest } = require("gulp");
const rimraf = require("rimraf");
const concat = require("gulp-concat");
const cssmin = require("gulp-cssmin");
const uglify = require("gulp-uglify");
const { Server, config } = require('karma');
const exec = require('child_process').exec;

let version = `1.0.` + (process.env.BUILD_NUMBER || '0');
let configuration = arg.config || process.env.BUILD_CONFIGURATION || 'Release';
let clientApp = './';
let serverApp = '../'
let webroot = clientApp + "public.g/";
let paths = {
    js: clientApp + "js/**/*.js",
    minJs: webroot + "js/**/*.min.js",
    cmdLineArgs: clientApp + "js/cmdLineArgs.js",
    css: clientApp + "css/**/*.css",
    minCss: webroot + "css/**/*.min.css",
    csProjs: serverApp + '**/*.csproj',
    csTestProjs: serverApp + '../**/*[Tt]est*.csproj',
    csWebProject: serverApp + 'TodoApi.WebUI.csproj',
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css",
};

task("clean:dotnet", function() {
    return src(paths.csProjs, {read: false})
               .pipe(clean());
});

task("restore:dotnet", function() {
    return src(paths.csProjs, {read: false})
               .pipe(restore());
});

task("run:build:dotnet", function() {
    return src(paths.csProjs, {read: false})
               .pipe(build({
                   configuration: configuration, 
                   version: version
                }));
})

task("build:dotnet", series("restore:dotnet", "run:build:dotnet"));

task("run:test:dotnet", function() {
    return src(paths.csTestProjs, {read: false})
               .pipe(test());
});

task("test:dotnet", series("build:dotnet", "run:test:dotnet"));

task("run:publish:dotnet", function() {
    return src(paths.csWebProject, {read: false})
               .pipe(publish({
                   configuration: configuration, 
                   version: version
                }));
});

task("publish:dotnet", series("test:dotnet", "run:publish:dotnet"));

task("clean:js", function(cb) {
    rimraf(paths.concatJsDest, cb);
});

task("clean:css", function(cb) {
    rimraf(paths.concatCssDest, cb);
});

task("clean", series("clean:js", "clean:css", "clean:dotnet"));

task("min:js", function() {
    return src([paths.js, "!" + paths.minJs, "!" + paths.cmdLineArgs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(dest("."));
});

task("min:css", function() {
    return src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(dest("."));
});

task("min", series("min:js", "min:css"));

task('test:js', function(done) {    
    new Server(config.parseConfig(__dirname + "/karma.conf.js", 
                                  {singleRun: true}), function() {
                                      done()
                                  }).start();
});

task("test", series("test:js", "test:dotnet"))

task('run:dotnet', series("test", ()=>{
    return src(paths.csWebProject, {read: false})
                .pipe(run());
}));

task("copy:legacy:assets", function(done){
    src(paths.minCss).pipe(dest(clientApp + "public/css"));
    src(paths.minJs).pipe(dest(clientApp + "public/js"));
    src(clientApp + "favicon.ico").pipe(dest(clientApp + "public/"))
    done();
});

task("publish", series("clean", "min", "publish:dotnet", "copy:legacy:assets"));

task('tdd', function(done) {
    new Server({
        configFile: __dirname + "/karma.conf.js",
        autoWatch: true,
        singleRun: false
    }, done).start();
});
