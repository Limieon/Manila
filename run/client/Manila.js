const project = Manila.getProject()
const workspace = Manila.getWorkspace()

task('compile').onExecute(() => {
	print('Compiling Client...')

	const config = Manila.getConfig()
	print('Platform:', config.platform)
	print('Config:', config.config)
	print('Arch:', config.arch)

	const flags = ManilaCPP.clangFlags()
	flags.name = project.name
	flags.binDir = Manila.dir(project.location).join('bin').join(config.platform).join(`${config.name}-${config.arch}`).join(flags.name)
	flags.objDir = Manila.dir(project.location).join('bin-int').join(config.platform).join(`${config.name}-${config.arch}`).join(flags.name)

	flags.files = Manila.dir(project.location)
		.join('src')
		.files(f => f.endsWith('.cpp') || f.endsWith('.c'), true)

	print(flags.files)

	const res = ManilaCPP.clangCompile(flags, project, workspace)
})

task('run')
	.dependsOn(':client:compile')
	.dependsOn(':tests:client:run')
	.onExecute(() => {
		print('Running...')
	})
