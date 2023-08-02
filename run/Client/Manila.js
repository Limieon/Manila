project('client')
namespace = 'Genesis.Client'
version = '1.0.0'

const lol = parameterString('lol', 'enter any string', 'abc')

task('compile')
	.dependsOn(':compile')
	.dependsOn(':core:compile')
	.executes(() => {
		print('Compiling Client...')
		print('Lol:', lol)
	})
