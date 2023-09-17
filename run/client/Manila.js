task('compile').onExecute(() => {
	print('Compiling Client...')

	const config = Manila.getConfig()
	print('Platform:', config.platform)
	print('Config:', config.config)
	print('Arch:', config.arch)
})

task('run')
	.dependsOn(':client:compile')
	.dependsOn(':tests:client:run')
	.onExecute(() => {
		print('Running...')
	})
