'use strict';

module.exports = function (grunt) {

    grunt.initConfig({
		
			clean: {
				build: {
					src: ["**/bin/**", "**/obj/**", "buildOutput/**", "!node_modules/**", "!Build/**", "!packages/**"]
				}
			},
			
			copy: {
				release: {
					files: [
						{
							expand: true,
							cwd: 'HobknobClientNet/bin/Release/',
              src: ['**'],
							dest: 'buildOutput/'
						}
					]
				}
			},
			
			msbuild: {
				build: {
					src: ['hobknob-client-net.sln'],
					options: {
						projectConfiguration: 'Release',
						targets: ['Clean', 'Rebuild'],
						stdout: true,
						buildParameters: {
							WarningLevel: 2
						},
						verbosity: 'quiet'
					}
				}
			},
			
			exec: {
				runTests: {
					cmd: "Build\\nunit\\bin\\nunit-console.exe --xml=UnitTestsResult.xml HobknobClientNet.Tests\\bin\\Release\\HobknobClientNet.Tests.dll"
				}
			}
			
		});
		
		grunt.loadNpmTasks('grunt-contrib-clean');
		grunt.loadNpmTasks('grunt-contrib-copy');
		grunt.loadNpmTasks('grunt-msbuild');
		grunt.loadNpmTasks('grunt-exec');

    grunt.registerTask('default', 'build');
    grunt.registerTask('build', [
        'clean:build',
        'msbuild:build',
        'test',
        'copy:release'
    ]);
    grunt.registerTask('test', ['exec:runTests']);
};