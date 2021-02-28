"use strict";

const arg = require('./js/cmdLineArgs')(process.argv);
const { clean, restore, build, test, publish, run } = require("gulp-dotnet-cli");
const { task, series, src, dest } = require("gulp");
const rimraf = require("rimraf");
const concat = require("gulp-concat");
const cssmin = require("gulp-cssmin");
const uglify = require("gulp-uglify");
const Server  = require('karma').Server;

let version = `1.0.` + (process.env.BUILD_NUMBER || '0');
let configuration = arg.config || process.env.BUILD_CONFIGURATION || 'Release';
let webroot = "./public/";
let paths = {
    js: "./js/**/*.js",
    minJs: webroot + "js/**/*.min.js",
    cmdLineArgs: "./js/cmdLineArgs.js",
    css: "./css/**/*.css",
    minCss: webroot + "css/**/*.min.css",
    csProjs: '**/*.csproj',
    csTestProjs: '**/*test*.csproj',
    csWebProject: 'TodoApi.csproj',
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

task("build:dotnet", series("restore:dotnet", function() {
    return src(paths.csProjs, {read: false})
               .pipe(build({
                   configuration: configuration, 
                   version: version
                }));
}));

task("test:dotnet", series("build:dotnet", function() {
    return src(paths.csTestProjs, {read: false})
               .pipe(test());
}));

task("publish:dotnet", series("test:dotnet", function() {
    return src(paths.csWebProject, {read: false})
               .pipe(publish({
                   configuration: configuration, 
                   version: version
                }));
}));
////convert a project to a nuget package
//gulp.task('pack', series('build:dotnet', () => {
//    return src('**/TestLibrary.csproj', {read: false})
//                .pipe(pack({
//                            output: path.join(process.cwd(), 'nupkgs') , 
//                            version: version
//                            }));
//}));
////push nuget packages to a server
//gulp.task('push', series('pack', () => {
//    return src('nupkgs/*.nupkg', {read: false})
//                .pipe(push({
//                    apiKey: process.env.NUGET_API_KEY, 
//                    source: 'https://myget.org/f/myfeedurl'}));
//}));

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
    new Server({
        configFile: __dirname + "/karma.conf.js",
        singleRun: true,
    }, done).start();
});

task("test", series("test:js", "test:dotnet"))

task('run:dotnet', series("test", ()=>{
    return src(paths.csWebProject, {read: false})
                .pipe(run());
}));

task("publish", series("clean", "min", "publish:dotnet"));

task('tdd', function(done) {
    new Server({
        configFile: __dirname + "/karma.conf.js",
        autoWatch: true,
        singleRun: false
    }, done).start();
});
