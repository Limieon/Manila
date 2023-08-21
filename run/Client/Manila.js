const ManilaCS = await importPlugin('manila.cs')

project('client')
namespace = 'Genesis.Client'
version = '1.0.0'

const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getConfig()

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
