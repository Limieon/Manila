const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const binDir = Manila.dir(workspace.location).join('bin').join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)
const objDir = Manila.dir(workspace.location)
	.join('bin-int')
	.join(config.platform)
	.join(`${config.config}-${config.arch}`)
	.join(project.name)

const srcFileSet = Manila.fileSet(project.location)
srcFileSet.include('src/**/*.c').include('src/**/*.cpp')

task('clean').onExecute(async () => {
	Manila.println('Deleting Bin Dir...')
	if (binDir.exists()) binDir.delete()
	Manila.println('Deleting Obj Dir...\n')
	if (objDir.exists()) objDir.delete()
})

task('recompile')
	.tag('manila/rebuild')
	.dependsOn(':client:clean')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.println('Recompiling...')
	})

task('compile')
	.tag('manila/build')
	.onExecute(async () => {
		Manila.println('Building...')
	})

task('run')
	.tag('manila/run')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.println('Running...')
	})
