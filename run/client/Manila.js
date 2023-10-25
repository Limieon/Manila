Manila.include('Defines.js')
apply('manilacpp/application')

const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

applyCommonConfig(project)

Manila.task('run')
	.tag('manila/run')
	.onExecute(async () => {
		Manila.println('Running...')
	})
