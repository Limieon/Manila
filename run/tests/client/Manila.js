Manila.task('compile')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.println('Compiling Client Tests...')
	})

Manila.task('run')
	.tag('manila/finalize')
	.dependsOn(':tests:client:compile')
	.onExecute(async () => {
		Manila.println('Running Client Tests...')
	})
