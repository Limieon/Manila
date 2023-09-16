print('Name:', Manila.getProject().name)

task('compile').onExecute(() => {
	print('Compiling...')
})

task('run')
	.dependsOn(':client:compile')
	.onExecute(() => {
		print('Running...')
	})
