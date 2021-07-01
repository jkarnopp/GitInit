/// <binding BeforeBuild='default' Clean='clean' ProjectOpened='watch' />
let gulp = require('gulp'),
    rimraf = require('rimraf'),
    concat = require('gulp-concat'),
    sass = require('gulp-sass'),
    sourcemaps = require('gulp-sourcemaps'),
    minify = require('gulp-minify'),
    cleanCss = require('gulp-clean-css'),
    rename = require('gulp-rename'),
    postcss = require('gulp-postcss'),
    autoprefixer = require('autoprefixer'),
    del = require('del'),
    paths = {
        nodeModuleRoot: './node_modules/',
        stylesRoot: './assets/styles/',
        scriptsRoot: './assets/scripts/',
        imagesRoot: './assets/images/',
        webRoot: './wwwroot/'
    },
    // Set the browser that you want to support
    AUTOPREFIXER_BROWSERS = [
        'ie >= 10',
        'ie_mob >= 10',
        'ff >= 30',
        'chrome >= 34',
        'safari >= 7',
        'opera >= 23',
        'ios >= 7',
        'android >= 4.4',
        'bb >= 10'
    ];

//Font awesome has an issue with reletive paths, so it is recommended not to bundle it with other css files
function copyFontAwesome() {
    return gulp.src(paths.nodeModuleRoot + '@fortawesome/fontawesome-free/**/*')
        .pipe(gulp.dest(paths.webRoot + 'lib/font-awesome/'));
};

function cleanFontAwesome(callback) {
    rimraf(paths.webRoot + 'lib/font-awesome', callback);
};

//function copyBootstrapFonts() {
//    return gulp.src(paths.nodeModuleRoot + 'bootstrap-sass/assets/fonts/**/*')
//        .pipe(gulp.dest(paths.webRoot + 'fonts/'));
//};

function cleanBootstrapFonts(callback) {
    rimraf(paths.webRoot + 'fonts', callback);
};

//Process SASS files and save the css versions to the css folder.
function buildSassToCss() {
    return gulp.src([paths.stylesRoot + 'scss/*.scss'])
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(postcss([
            autoprefixer({
                browsers: AUTOPREFIXER_BROWSERS
            })
        ]))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.stylesRoot + 'css/'));
}

//Build Global CSS
//paths.nodeModuleRoot + "bootstrap/dist/css/bootstrap.css",
function buildGlobalCss() {
    return gulp.src([paths.stylesRoot + 'css/*.css'])
        .pipe(concat('global.css'))
        //.pipe(autoprefixer({ browsers: AUTOPREFIXER_BROWSERS }))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.webRoot + 'css/'))
        .pipe(cleanCss())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.webRoot + 'css/'));
}

//Build Global JS
function buildGlobalJs() {
    return gulp.src([
        paths.nodeModuleRoot + "jquery/dist/jquery.js",
        paths.nodeModuleRoot + "popper.js/dist/umd/popper.js",
        paths.nodeModuleRoot + "tooltip.js/dist/umd/tooltip.js",
        //paths.nodeModuleRoot + "bootstrap/dist/js/bootstrap.js",
        //paths.nodeModuleRoot + "bootstrap-sass/assets/javascripts/bootstrap.js",
        paths.nodeModuleRoot + "bootstrap/dist/js/bootstrap.bundle.js",
        paths.scriptsRoot + "global/site.js"
    ], { allowEmpty: true })
        .pipe(concat('global.js'))
        .pipe(minify())
        .pipe(gulp.dest(paths.webRoot + 'js'));
}

//Build Validation and Edit Extensions JS
function buildValidationJs() {
    return gulp.src([
        paths.nodeModuleRoot + "jquery-validation/dist/jquery.validate.js",
        paths.nodeModuleRoot + "jquery-validation/dist/additional-methods.js",
        paths.nodeModuleRoot + "jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
        paths.scriptsRoot + "jquery.autogrow-textarea.js"
    ], { allowEmpty: true })
        .pipe(concat('validationAndEdit.js'))
        .pipe(minify())
        .pipe(gulp.dest(paths.webRoot + 'js'));
}

//Build PageScripts
function buildPageScriptsJs() {
    return gulp.src(paths.scriptsRoot + "PageScripts/*.js")
        .pipe(minify())
        .pipe(gulp.dest(paths.webRoot + 'js/PageScripts/'));
}

//Copy Images
function copyImages() {
    return gulp.src(paths.imagesRoot + "*.*")
        .pipe(gulp.dest(paths.webRoot + 'images/'));
}

function watch() {
    gulp.watch(paths.scriptsRoot + "PageScripts/*.js", gulp.series(buildPageScriptsJs));
    gulp.watch(paths.stylesRoot + 'scss/*.scss', buildPageStyleSassToCss);
    gulp.watch(paths.stylesRoot + 'css/*.css', buildGlobalCss);
    gulp.watch(paths.imagesRoot + '*.*', copyImages);
}

//Process SASS files and save the css versions to the css folder.
function buildPageStyleSassToCss() {
    return gulp.src(paths.stylesRoot + 'PageStyles/scss/*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(postcss([
            autoprefixer({
                browsers: AUTOPREFIXER_BROWSERS
            })
        ]))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.stylesRoot + 'PageStyles/css/'));
}

//Build PageStyles
function buildPageStylesCss() {
    return gulp.src(paths.stylesRoot + "PageStyles/css/*.css")
        .pipe(minify())
        .pipe(gulp.dest(paths.webRoot + 'css/PageStyles/'));
}

function cleanJsFiles() {
    return del([paths.webRoot + 'js/*.js', paths.webRoot + 'js/PageScripts/*.js']);
}

function cleanCssFiles() {
    return del([paths.webRoot + 'css/*.css', paths.webRoot + 'css/PageStyles/*.css']);
}

function cleanImageFiles() {
    return del([paths.webRoot + 'images/*.*']);
}

exports.default = gulp.series(buildSassToCss, buildPageStyleSassToCss, gulp.parallel(copyFontAwesome, buildGlobalCss, buildPageStylesCss, buildGlobalJs, buildValidationJs, buildPageScriptsJs, copyImages));

exports.clean = gulp.series(cleanFontAwesome, cleanBootstrapFonts, cleanJsFiles, cleanCssFiles, cleanImageFiles);

exports.watch = gulp.series(watch);