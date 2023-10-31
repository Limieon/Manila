const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

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
			.files(),
		abc: 'lol'
	})
})
