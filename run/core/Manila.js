const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const baseBinDir = Manila.dir(workspace.location).join('bin')
const baseObjDir = Manila.dir(workspace.location).join('bin-int')

const binDir = baseBinDir.join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)
const objDir = baseObjDir.join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)

const srcFileSet = Manila.fileSet(project.location)
srcFileSet.include('src/**/*.c').include('src/**/*.cpp')

Manila.task('clean').onExecute(async () => {
	Manila.println('Deleting Bin Dir...')
	if (baseBinDir.exists()) baseBinDir.delete()
	Manila.println('Deleting Obj Dir...')
	if (baseObjDir.exists()) baseObjDir.delete()
})

Manila.task('recompile')
	.tag('manila/rebuild')
	.dependsOn(':core:clean')
	.dependsOn(':core:compile')
	.onExecute(async () => {
		Manila.println('Recompiling...')
	})

Manila.task('compile')
	.tag('manila/build')
	.onExecute(async () => {
		Manila.println('Building...')

		const flags = MSBuild.flags()
		flags.binDir = binDir
		flags.objDir = objDir
		flags.srcFiles = srcFileSet.files()
		flags.includeDirs.Add(Manila.dir('include'))
		flags.libDirs.Add(Manila.dir('lib').join(`${config.arch}`))
		flags.binaryType = MSBuild.staticLib()

		//MSBuild.project(workspace, project, config, flags)
		Manila.println('Generating project file...')
	})
