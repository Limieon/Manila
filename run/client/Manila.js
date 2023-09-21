const project = Manila.getProject()
const workspace = Manila.getWorkspace()

let binary = undefined

task('compile').onExecute(async () => {
	const config = Manila.getConfig()
	const flags = ManilaCPP.clangFlags()
	flags.name = project.name
	flags.binDir = Manila.dir(project.location).join('bin').join(config.platform).join(`${config.config}-${config.arch}`).join(flags.name)
	flags.objDir = Manila.dir(project.location)
		.join('bin-int')
		.join(config.platform)
		.join(`${config.config}-${config.arch}`)
		.join(flags.name)

	const files = Manila.dir(project.location)
		.join('src')
		.files(f => f.endsWith('.cpp') || f.endsWith('.c'), true)

	const objFiles = []
	var numFile = 1

	Manila.markup(
		`[gray]Compiling[/] [blue]${workspace.name}[/][gray]/[/][blue]${
			project.name
		}[/] [gray]=>[/] [magenta]${flags.binDir.getPath()}[/][gray]...[/]`
	)
	for (const file of files) {
		Manila.markup(`[yellow]${numFile}[/][gray]/[/][green]${files.Length}[/] [gray]>[/] [magenta]${file.getFileName()}[/]`)

		objFiles.push(ManilaCPP.clangCompile(flags, project, workspace, file).objFile)
	}
	const linkerRes = ManilaCPP.clangLink(flags, project, workspace, objFiles)

	binary = linkerRes.binary
})

task('run')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.print('Executing', binary.getPath())
	})
