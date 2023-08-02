project('core')
namespace = 'Genesis.Core'
version = '1.0.0'

task('compile')
	.dependsOn(':compile')
	.executes(() => {
		print('Compiling Core...')
	})
