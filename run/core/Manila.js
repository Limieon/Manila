const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

project.configure(_ => {
	_.fileSets(project.location, {
		src: _ => {
			_.include('src/**/*.cpp').include('src/**/*.hpp').include('src/**/*.c').include('src/**/*.h')
		},
		test: _ => {
			_.include('test/**/*.cpp').include('test/**/*.hpp').include('test/**/*.c').include('test/**/*.h')
		}
	})
})
