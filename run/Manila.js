const headless = parameterBoolean('headless', 'Enables compilation flags for headless running')
const graphicsApi = parameterString('gapi', 'Choose a graphics api', 'opengl')
const year = parameterNumber('year', 'Enter a year', 2023)
const test = parameterBoolean('test', 'Run tests')

task('clean').executes(() => {
	print('Cleaning...')
})

task('compile').executes(() => {
	print('Compiling...')

	if (headless) {
		print('Compiling Headless...')
	}

	print(`Compiling for ${graphicsApi}`)
	print('Year:', year)
})

task('run')
	.dependsOn(':compile')
	.dependsOn(':clean')
	.executes(() => {
		print('Running...')
	})

task('tests')
	.dependsOn(':compile')
	.executes(() => {
		print('Running Tests...')
	})
