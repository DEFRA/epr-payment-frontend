const gulp = require('gulp');
const concat = require('gulp-concat');
const sass = require('gulp-sass')(require('sass'))

// Define paths to the SCSS files
const paths = {
    scss: [
        'assets/scss/components/_language-toggle.scss',
        'assets/scss/components/_crown-footer.scss',
        'assets/scss/components/_cookies-media.scss'
    ],
    output: 'wwwroot/css'
};

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

// Task to copy images to /payment base folder
gulp.task('copy-govuk-crown-images', function () {
    return gulp.src('wwwroot/assets/images/*') // Source folder
        .pipe(gulp.dest('wwwroot/payment/assets/images', { overwrite: true })); // Destination folder
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

// Task to compile and bundle SCSS into a single CSS file
gulp.task('compile-scss', function () {
    return gulp.src(paths.scss)
        .pipe(concat('application.css'))
        .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
        .pipe(gulp.dest(paths.output));
});

// Default task
gulp.task('build-frontend', gulp.series('copy-govuk-styles', 'copy-govuk-scripts', 'copy-govuk-images', 'copy-govuk-crown-images',
    'copy-govuk-fonts', 'copy-govuk-manifest', 'compile-scss')); 

