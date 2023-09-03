//const ManilaCS = importPlugin('manila.cs')
const ManilaCPP = importPlugin('manila.cpp')

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
	.executes(() => {
		const srcDir = Manila.dir(project.location.getPath()).concat('src')

		const flags = ManilaCPP.clangFlags()
			.outDir(Manila.dir(workspace.location.getPath()).concat('bin', config.platform, `${config.config}-${config.arch}`, project.name))
			.objDir(Manila.dir(workspace.location.getPath()).concat('bin-int', config.platform, `${config.config}-${config.arch}`, project.name))
			.files(srcDir.files(true, f => f.endsWith('.cpp') || f.endsWith('.c')))

		const result = ManilaCPP.clangCompile(project, workspace, flags)
	})
