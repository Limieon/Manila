
task('compile')
	.dependsOn(':core:compile')
	.executes(() => {
		print('Author:', getProperty('author'))
		print('Compiling Server Core...')
	})
