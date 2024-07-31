const gulp = require('gulp');
const concat = require('gulp-concat');

// Task to compile GOV.UK Frontend SCSS
gulp.task('copy-govuk-styles', function () {
    return gulp.src('node_modules/govuk-frontend/dist/govuk/*.css')
        .pipe(concat('govuk-frontend.min.css'))
        .pipe(gulp.dest('wwwroot/css', { overwrite: true }));
});

// Task to copy GOV.UK Frontend JavaScript
gulp.task('copy-govuk-scripts', function () {
    return gulp.src('node_modules/govuk-frontend/dist/govuk/govuk-frontend.min.js')
        .pipe(concat('govuk-frontend.min.js'))
        .pipe(gulp.dest('wwwroot/js', { overwrite: true }));
});

// Task to copy images
gulp.task('copy-govuk-images', function () {
    return gulp.src('node_modules/govuk-frontend/dist/govuk/assets/images/*')
        .pipe(gulp.dest('wwwroot/assets/images', { overwrite: true }));
});

// Task to copy fonts
gulp.task('copy-govuk-fonts', function () {
    return gulp.src('node_modules/govuk-frontend/dist/govuk/assets/fonts/*')
        .pipe(gulp.dest('wwwroot/assets/fonts', { overwrite: true }));
});

// Task to copy manifest
gulp.task('copy-govuk-manifest', function () {
    return gulp.src('node_modules/govuk-frontend/dist/govuk/assets/manifest.json')
        .pipe(gulp.dest('wwwroot/assets', { overwrite: true }));

});

// Default task
gulp.task('build-frontend', gulp.series('copy-govuk-styles', 'copy-govuk-scripts', 'copy-govuk-images', 'copy-govuk-fonts','copy-govuk-manifest'));