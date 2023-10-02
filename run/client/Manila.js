const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const year = parameterInt('year', 'enter the current year')

const binDir = Manila.dir(workspace.location).join('bin').join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)
const objDir = Manila.dir(workspace.location)
	.join('bin-int')
	.join(config.platform)
	.join(`${config.config}-${config.arch}`)
	.join(project.name)

let binary = undefined

const srcFileSet = Manila.fileSet(project.location)
srcFileSet.include('src/**/*.c').include('src/**/*.cpp')

task('clean').onExecute(async () => {
	print('Deleting Bin Dir...')
	if (binDir.exists()) binDir.delete()
	print('Deleting Obj Dir...\n')
	if (objDir.exists()) objDir.delete()
})

task('recompile')
	.dependsOn(':client:clean')
	.dependsOn(':client:compile')
	.onExecute(async () => {})

task('compile').onExecute(async () => {
	const flags = ManilaCPP.clangFlags()
	flags.name = project.name
	flags.binDir = binDir
	flags.objDir = objDir
	flags.platform = config.platform

	let files = srcFileSet.files()

	const objFiles = []
	var numFile = 1

	for (const file of files) {
		Manila.markup(`[yellow]${numFile}[/][gray]/[/][green]${files.Length}[/] [gray]>[/] [magenta]${file.getFileName()}[/]`)
		let res = await ManilaCPP.clangCompile(flags, project, workspace, file)

		if (res.success) {
			if (res.skipped) Manila.appendMarkup(' [gray]-[/] [yellow]Skipped[/]')
			else Manila.appendMarkup(' [gray]-[/] [green]Success[/]')
		}

		objFiles.push(res.objFile)

		numFile++
	}
	Manila.markup(
		`\n[gray]Linking[/] [blue]${workspace.name}[/][gray]/[/][blue]${
			project.name
		}[/] [gray]=>[/] [magenta]${flags.binDir.getPath()}[/][gray]...[/]\n`
	)
	const linkerRes = ManilaCPP.clangLink(flags, project, workspace, objFiles)

	binary = linkerRes.binary
})

task('run')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		const app = Manila.application(binary)
		let exitCode = app.run(project.location, 'abc', 'def', 'ghi')
		Manila.print('App Exited with code:', exitCode)
	})
