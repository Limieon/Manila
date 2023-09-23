const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const binDir = Manila.dir(workspace.location).join('bin').join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)
const objDir = Manila.dir(workspace.location)
	.join('bin-int')
	.join(config.platform)
	.join(`${config.config}-${config.arch}`)
	.join(project.name)

let binary = undefined

task('clean').onExecute(async () => {
	print('Deleting Bin Dir...')
	if (binDir.exists()) binDir.delete()
	print('Deleting Obj Dir...\n')
	if (objDir.exists()) objDir.delete()
})

task('compile')
	.dependsOn(':client:clean')
	.onExecute(async () => {
		const flags = ManilaCPP.clangFlags()
		flags.name = project.name
		flags.binDir = binDir
		flags.objDir = objDir
		flags.platform = config.platform

		const files = Manila.dir(project.location)
			.join('src')
			.files(f => f.endsWith('.cpp') || f.endsWith('.c'), true)

		const objFiles = []
		var numFile = 1

		for (const file of files) {
			Manila.markup(`[yellow]${numFile}[/][gray]/[/][green]${files.Length}[/] [gray]>[/] [magenta]${file.getFileName()}[/]`)

			objFiles.push(await ManilaCPP.clangCompile(flags, project, workspace, file).objFile)

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
		Manila.print('Executing', binary.getPath())
	})
