const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const binDir = project.location.join('bin')
const objDir = project.location.join('bin-int')

project.configure(_ => {
	_.fileSets({
		src: Manila.fileSet(project.location)
			.include('src/**/*.cpp')
			.include('src/**/*.hpp')
			.include('src/**/*.c')
			.include('src/**/*.h')
			.files(),

		test: Manila.fileSet(project.location)
			.include('test/**/*.cpp')
			.include('test/**/*.hpp')
			.include('test/**/*.c')
			.include('test/**/*.h')
			.files()
	})
	_.binDir(binDir)
	_.objDir(objDir)
	_.workingDir(binDir)
})

Manila.task('run')
	.tag('manila/run')
	.onExecute(async () => {
		const app = Manila.app(project.getBinary())
		app.run()
	})
