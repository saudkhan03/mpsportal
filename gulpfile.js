var gulp = require("gulp");
var uglify = require("gulp-uglify");
var gutil = require("gulp-util");
//var concat = require("gulp-concat");

gulp.task('default',['minify']);

gulp.task('minify',function(){
    gulp.src('wwwroot/js/**/*.js')
    .pipe(uglify().on('error',gutil.log))
    //.pipe(concat())
    .pipe(gulp.dest("wwwroot/minjs"))
});