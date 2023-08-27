const ManilaCS = await importPlugin('manila.cs')

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
		print('Location:', workspace.location)
		print()
		print('--- Project ---')
		print('Name:', project.name)
		print('Location:', project.location)
		print('Namespace:', project.namespace)
		print('Author:', project.author)
	})

task('compile')
	.executes(() => {
		print('Bin Dir:', Manila.directory(workspace.location).concat('bin', `${config.config}-${config.arch}`, config.platform, project.name).getPath())
	})
