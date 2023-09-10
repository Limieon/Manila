Manila.task('compile').onExecute(() => {
	Console.WriteLine('Compiling...')
})

Manila.task('run')
	.dependsOn('compile')
	.onExecute(() => {
		Console.WriteLine('Running...')
	})
