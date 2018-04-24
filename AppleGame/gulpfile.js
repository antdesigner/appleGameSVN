
var gulp = require('gulp');
var del = require('del');
var uglify = require("gulp-uglify");
var paths = {
    scripts: ['ts/**/*.js', 'ts/**/*.ts', 'ts/**/*.map']
};
gulp.task('clean', function () {
    return del(['wwwroot/scripts/**/*']);
});
gulp.task('default', function () {
    gulp.src(paths.scripts).pipe(gulp.dest('wwwroot/scripts'));
});
gulp.task('compressJs', function () {
    gulp.src(['wwwroot/js/**/*.js'])
        .pipe(uglify())
        .pipe(gulp.dest('wwwroot/lib'));
});