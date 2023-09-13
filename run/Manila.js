task('compile').onExecute(() => {
})

task('run')
	.dependsOn('compile')
	.onExecute(() => {
		scriptExec()
	})
