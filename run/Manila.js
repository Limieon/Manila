//const ManilaCS = await importPlugin('manila.cs')
//const ManilaCPP = importPlugin('manila.cpp')

const headless = parameterBoolean('headless', 'Enables compilation flags for headless running')
const graphicsApi = parameterString('gapi', 'Choose a graphics api', 'opengl')
const year = parameterNumber('year', 'Enter a year', 2023)
const test = parameterBoolean('test', 'Run tests')

properties({ appName: 'GenesisEngine' })

// Specify a project where you want to customize your settings
// use regex for multiple projects or use an array for multiple specific projects:
// 'project': Only project
// '/.*/': Only projects matching this regex
// ['project1', 'project2']: Every project included inside the array
project(/.*/, async () => {
	properties({
		author: 'Limieon',
		version: '1.0.0'
	})

	//// Here you can define your repositories containing your dependencies
	//repositories([
	//	git('https://github.com/Limieon/Manila-Repositories'),
	//	local('../../Manila-Repositories')
	//])
})

// You can have as many project declarators as you want
project(':core', () => {
	properties({
		name: 'GenesisCore',
		namespace: 'Genesis.Core'
	})

	//dependencies([
	//	testImplementation('BenchmarkDotNet')
	//])
})

project(':client', async () => {
	properties({
		name: 'GenesisClient',
		namespace: 'Genesis.Client'
	})

	//// Here you can declare dependencies
	//dependencies([
	//	implemenatation(project(':core'))
	//  implemenatation(nuget('Silk.NET.Core', '2.17.1'))
	//])
})
