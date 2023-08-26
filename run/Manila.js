const ManilaCS = await importPlugin('manila.cs')

const headless = parameterBoolean('headless', 'Enables compilation flags for headless running')
const graphicsApi = parameterString('gapi', 'Choose a graphics api', 'opengl')
const year = parameterNumber('year', 'Enter a year', 2023)
const test = parameterBoolean('test', 'Run tests')

async function sleep(dur) {
	return new Promise((res, rej) => {
		setTimeout(res, dur)
	})
}

// Specify a project where you want to customize your settings
// use regex for multiple projects or use an array for multiple specific projects:
// 'project': Only project
// '/.*/': Only projects matching this regex
// ['project1', 'project2']: Every project included inside the array
project(/.*/, async () => {
	author = 'Limieon'
	version = '1.0.0'
	//
	//// Those properties will be available in all projects
	//properties({
	//	appName: 'GenesisEngine'
	//})
	//
	//// Here you can define your repositories containing your dependencies
	//repositories([
	//	git('https://github.com/Limieon/Manila-Repositories'),
	//	local('../../Manila-Repositories')
	//])
})

// You can have as many project declarators as you want
project(':core', () => {
	namespace = 'Genesis.Core'

	//dependencies([
	//	testImplementation('BenchmarkDotNet')
	//])
})

project(':client', async () => {
	namespace = 'Genesis.Client'

	// This will add the graphicsApi property into the client project
	// Properties can override each other
	// The order will be the execution order (top to bottom)
	// That means bottom properties will override top properties
	//properties({
	//	graphicsApi: graphicsApi
	//})
	//
	//// Here you can declare dependencies
	//dependencies([
	//	implemenatation(project(':core'))
	//])
})
