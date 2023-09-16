Manila.project(regex('.*'), () => {
	properties({})
})

Manila.project(':client', () => {
	properties({
		name: 'Client',
		version: '1.0.0'
	})
})

Manila.project(':core', () => {
	properties({
		name: 'Core',
		version: '1.0.0'
	})
})

properties({
	name: 'TestWorkspace'
})
