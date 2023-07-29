task('compile')
	.dependsOn(':compile')
	.dependsOn(':core:compile')
	.executes(() => {
		print('Compiling Client...')
	})
