task('compile')
	.dependsOn(':client:compile')
	.onExecute(() => {
		print('Compiling Client Tests...')
	})

task('run')
	.dependsOn(':tests:client:compile')
	.onExecute(() => {
		print('Running client Tests...')
	})
