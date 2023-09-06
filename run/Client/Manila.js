//const ManilaCS = importPlugin('manila.cs')
const ManilaCPP = importPlugin('manila.cpp')
const ManilaDC = importPlugin('maniladc')

const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getConfig()

task('print')
	.executes(() => {
		print('--- Config ---')
		print('Config:', config.config)
		print('Platform:', config.platform)
		print('Architecture:', config.arch)
		print()
		print('--- Workspace ---')
		print('Name:', workspace.name)
		print('Location:', workspace.location.getPath())
		print()
		print('--- Project ---')
		print('Name:', project.name)
		print('Location:', project.location.getPath())
		print('Namespace:', project.namespace)
		print('Author:', project.author)
	})

task('compile')
	.executes(async () => {
		const srcDir = Manila.dir(project.location.getPath()).concat('src')

		const wh = ManilaDC.webhook(Manila.getSecrets().discord.webhook)
		await wh.send('Compiling...')
		
		const flags = ManilaCPP.clangFlags()
			.outDir(Manila.dir(workspace.location.getPath()).concat('bin', config.platform, `${config.config}-${config.arch}`, project.name))
			.objDir(Manila.dir(workspace.location.getPath()).concat('bin-int', config.platform, `${config.config}-${config.arch}`, project.name))
			.files(srcDir.files(true, f => f.endsWith('.cpp') || f.endsWith('.c')))
			.force()
			.silent()

		const result = ManilaCPP.clangCompile(project, workspace, flags, (e) => {
			if(e.type === 'compileFile') {
				print(e.current, '/', e.total, e.in.getPathRelative(project.location), '=>', e.out.getPathRelative(project.location))
			} else if(e.type === 'compileBinary') {
				print('Linking', e.in.length, 'Object Files into', e.out.getPathRelative(project.location))
			}
		})
		print('Compiled as', result.binaryFile.getPath())

		if(result.success) {
			await wh.send(`Build Successful! Took ${Manila.formatDuration(result.duration)}`)
			await wh.send(`Total: ${result.fileResult.total}\nCompiled: ${result.fileResult.compiled}\nSkipped: ${result.fileResult.skipped}`)
			return
		}

		await wh.send('Build Failed!')
	})
