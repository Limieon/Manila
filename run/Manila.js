Manila.project(regex('.*'), () => {
	properties({})
})

Manila.project(':client', () => {
	properties({
		name: 'Client',
		version: '1.0.0'
	})
})

Manila.project(':core', () => {
	properties({
		name: 'Core',
		version: '1.0.0'
	})
})

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
