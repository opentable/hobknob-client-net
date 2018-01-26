'use strict';
var package_json = require('./package.json');

module.exports = function (grunt) {

    grunt.initConfig({
			nugetpush: {
				dist: {
					src: 'deploy/*.nupkg',
				}
			}
		});
		
		grunt.loadNpmTasks('grunt-nuget');

    grunt.registerTask('default', 'nugetpush');
};