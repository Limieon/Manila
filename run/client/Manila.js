print('Name:', Manila.getProject().name)

task('compile').onExecute(() => {
	print('Compiling...')

	const config = Manila.getConfig()
	print('Platform:', config.platform)
	print('Config:', config.config)
	print('Arch:', config.arch)
})

task('run')
	.dependsOn(':client:compile')
	.onExecute(() => {
		print('Running...')
	})
