function applyCommonConfig(project) {
	binDir(project.workspace.location.join('bin').join(project.name))
	objDir(project.workspace.location.join('bin-int').join(project.name))

	files(['src/**/*.cpp', 'src/**/*.hpp', 'src/**/*.c', 'src/**/*/.h'])

	project.on('prebuild', () => {
		Manila.println(`Pre Build Event! (${project.name})`)
	})

	project.on('postbuild', () => {
		Manila.println(`Post Build Event! (${project.name})`)
	})
}
