task('compile')
	.dependsOn(':compile')
	.executes(() => {
		print('Compiling Core...')
	})
