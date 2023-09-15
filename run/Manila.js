/*Manila.project('core', () => {
	print('Execute!')
})*/

properties({
	name: 'TestWorkspace'
})

task('compile').onExecute(() => {
	print('Compiling...')
})

task('run')
	.dependsOn('compile')
	.onExecute(() => {
		print('Running...')
	})
