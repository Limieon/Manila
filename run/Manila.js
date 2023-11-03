properties({
	name: 'TestWorkspace'
})

Manila.project(regex('.*'), () => {
	properties({})

	Manila.getProject().configure(_ => {
		_.arch('x64')
		_.cppdialect('C++20')
	})
})

Manila.project(':client', () => {
	properties({
		name: 'Client',
		version: '1.0.0'
	})

	dependencies([project(':core')])
})

Manila.project(':core', () => {
	properties({
		name: 'Core',
		version: '1.0.0'
	})
})

Manila.project(':tests:client', () => {
	properties({
		name: 'Client-Tests',
		version: '1.0.0'
	})

	dependencies([project(':client')])
})

Manila.project(':tests:core', () => {
	properties({
		name: 'Core-Tests',
		version: '1.0.0'
	})

	dependencies([project(':core')])
})

Manila.on('manila/finalize', () => {
	MSBuild.generate(Manila.getWorkspace(), Manila.getConfig())
})
