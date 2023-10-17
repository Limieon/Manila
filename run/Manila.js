properties({
	name: 'TestWorkspace'
})

Manila.project(regex('.*'), () => {
	properties({})
})

Manila.project(':client', () => {
	properties({
		name: 'Client',
		version: '1.0.0'
	})

	dependencies([
		project(':core')
	])
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
})
