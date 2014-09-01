'use strict';

module.exports = function (grunt) {

    grunt.initConfig({
		
			clean: {
				build: {
					src: ["**/bin/**", "**/obj/**", "build_output/**", "nuget_package/**", "!node_modules/**", "!Build/**", "!packages/**"]
				}
			},
			
			copy: {
				release: {
					files: [
						{
							expand: true,
							cwd: 'HobknobClientNet/bin/Release/',
              src: ['**'],
							dest: 'build_output/lib/'
						}
					]
				},
				nuspec: {
					files: [ { 
						src: "HobknobClientNet.nuspec",
						dest: "build_output/"
					}]
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
			},
			
			nugetpack: {
				dist: {
					src: 'build_output/HobknobClientNet.nuspec',
					dest: 'nuget_package/'
				}
			},
			
			nugetpush: {
				dist: {
					src: 'nuget_package/*.nupkg',
				}
			}
			
		});
		
		grunt.loadNpmTasks('grunt-contrib-clean');
		grunt.loadNpmTasks('grunt-contrib-copy');
		grunt.loadNpmTasks('grunt-msbuild');
		grunt.loadNpmTasks('grunt-exec');
		grunt.loadNpmTasks('grunt-nuget');

    grunt.registerTask('default', 'build');
    grunt.registerTask('build', [
        'clean:build',
        'msbuild:build',
        'test',
        'copy:release',
				'copy:nuspec',
				'nugetpack:dist'
    ]);
    grunt.registerTask('test', ['exec:runTests']);
};