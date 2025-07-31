const gulp = require('gulp');
const concat = require('gulp-concat');
const sass = require('gulp-sass')(require('sass'));
const path = require('path');

// Define paths to the SCSS files
const paths = {
    govuk: "node_modules/govuk-frontend/dist/govuk/",
    scss: [
        'assets/scss/all.scss'
    ],
    output: 'wwwroot/css'
};

const deprecationSuppressions = ["import", "mixed-decls", "global-builtin"];

let loadPaths = [
    path.join(__dirname, "node_modules"),
    path.join(__dirname, paths.govuk)
];

const sassOptions = {
    loadPaths: loadPaths,
    outputStyle: 'compressed',
    quietDeps: true,
    silenceDeprecations: deprecationSuppressions
};

//// Task to compile GOV.UK Frontend SCSS
//gulp.task('copy-govuk-styles', function () {
//    return gulp.src(path.join(paths.govuk, '*.css'))
//        .pipe(concat('govuk-frontend.min.css'))
//        .pipe(gulp.dest('wwwroot/css', { overwrite: true }));
//});

// Task to copy GOV.UK Frontend JavaScript
gulp.task('copy-govuk-scripts', function () {
    return gulp.src(path.join(paths.govuk, 'govuk-frontend.min.js'))
        .pipe(concat('govuk-frontend.min.js'))
        .pipe(gulp.dest('wwwroot/js', { overwrite: true }));
});

// Task to copy images
gulp.task('copy-govuk-images', function () {
    return gulp.src(path.join(paths.govuk, 'assets/images/*'))
        .pipe(gulp.dest('wwwroot/images', { overwrite: true }));
});

// Task to copy images to /payment base folder
gulp.task('copy-govuk-crown-images', function () {
    return gulp.src('wwwroot/rebrand/images/*') // Source folder
        .pipe(gulp.dest('wwwroot/payment/assets/images', { overwrite: true })); // Destination folder
});

// Task to copy fonts
gulp.task('copy-govuk-fonts', function () {
    return gulp.src(path.join(paths.govuk, 'assets/fonts/*'))
        .pipe(gulp.dest('wwwroot/fonts', { overwrite: true }));
});

// Task to copy manifest
gulp.task('copy-govuk-manifest', function () {
    return gulp.src(path.join(paths.govuk, 'assets/manifest.json'))
        .pipe(gulp.dest('wwwroot/', { overwrite: true }));

});

// Task to compile and bundle SCSS into a single CSS file
gulp.task('compile-scss', function () {
    return gulp.src(paths.scss)
        .pipe(concat('application.css'))
        .pipe(sass(sassOptions).on('error', sass.logError))
        .pipe(gulp.dest(paths.output));
});

gulp.task('copy-rebrand', () => {
    return gulp.src(path.join(paths.govuk, '/assets/rebrand/**/*'), { base: path.join(paths.govuk, '/assets/rebrand') })
        .pipe(gulp.dest('wwwroot/rebrand'));
});

// Default task
gulp.task('build-frontend', gulp.series('copy-govuk-scripts', 'copy-govuk-images', 'copy-rebrand', 'copy-govuk-crown-images',
    'copy-govuk-fonts', 'copy-govuk-manifest', 'compile-scss')); 