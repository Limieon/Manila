
task('compile')
	.executes(() => {
		print('Author:', getProperty('author'))
		print('Compiling Core...')
	})
