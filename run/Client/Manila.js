const ManilaCS = await importPlugin('manila.cs')

const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getConfig()

// print('Workspace:', workspace.name)
// print('Project:', project.name)
// print('Config:', config.config)

task('compile')
	.executes(() => {
		ManilaCS.compile({
			workspace: workspace,
			project: project,
			config: config,
			srcDirs: [
				'./src/'
			],
			binDir: `${workspace.getRootDir()}/bin/${config.getConfig()}/${config.getOS()}/${project.getName()}/`
		})
	})
