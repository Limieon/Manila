Manila.task('compile')
	.dependsOn(':core:compile')
	.onExecute(async () => {
		Manila.println('Compiling Core Tests...')
	})

Manila.task('run')
	.tag('manila/finalize')
	.dependsOn(':tests:core:compile')
	.onExecute(async () => {
		Manila.println('Running Core Tests...')
	})
