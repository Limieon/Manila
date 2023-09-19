const project = Manila.getProject()
const workspace = Manila.getWorkspace()

let binary = undefined

task('compile').onExecute(() => {
	Manila.print('Compiling Client...')

	const config = Manila.getConfig()
	Manila.print('Platform:', config.platform)
	Manila.print('Config:', config.config)
	Manila.print('Arch:', config.arch)

	const flags = ManilaCPP.clangFlags()
	flags.name = project.name
	flags.binDir = Manila.dir(project.location).join('bin').join(config.platform).join(`${config.config}-${config.arch}`).join(flags.name)
	flags.objDir = Manila.dir(project.location)
		.join('bin-int')
		.join(config.platform)
		.join(`${config.config}-${config.arch}`)
		.join(flags.name)

	flags.files = Manila.dir(project.location)
		.join('src')
		.files(f => f.endsWith('.cpp') || f.endsWith('.c'), true)

	const res = ManilaCPP.clangCompile(flags, project, workspace)
	binary = res.binary
})

task('run')
	.dependsOn(':client:compile')
	.onExecute(() => {
		Manila.print('Executing', binary.getPath())
	})
