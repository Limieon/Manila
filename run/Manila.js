Manila.project(['core', 'abc'], () => {
	print('Executing...')
})

task('compile').onExecute(() => {
	print('Compiling...')
})

task('run')
	.dependsOn('compile')
	.onExecute(() => {
		print('Running...')
	})
