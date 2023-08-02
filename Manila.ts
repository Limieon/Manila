import FS from 'fs'
import Path from 'path'

type ProjectParameters = {
	name: string
	namespace: string
	dummy: boolean
}

type ModuleParameters = {
	name: string
	namespace: string
	dir: string
}

const dummyProjectTemplate = `// This is your main Manila BuildScipt
// You can create Taks using the task function
// and can accept user parameters
//
// Example:
// const year = parameterNumber('year', 'Enter a year', 2023)
//
// task('foo').executes(() => {
//     print('Bar!')
//     print('Current Year:', year)
// })
//
`

const projectTemplate = `// This is your Manila BuildScipt for {name}
// You can create Taks using the task function
// and can accept user parameters.
// Unlike your main Manila.js file, those can only
// be used by this project.
//
// Tasks are prefixed with :{name}
//
// Example:
// const year = parameterNumber('year', 'Enter a year', 2023)
//
// task('foo').executes(() => {
//     print('Bar!')
//     print('Current Year:', year)
// })
//

project('{name}')
namespace = '{namespace}'
version = '1.0.0'
`

export default class Manila {
	static parseTemplate(template: string, replace: object): string {
		let out = template.replaceAll('{\\t}', '\t')

		Object.keys(replace).forEach((k) => {
			out = out.replaceAll(`{${k}}`, replace[k])
		})
		return out
	}

	static createDefaultManilaJSFile(name: string, namespace: string, dir: string = '.', dummy: boolean = false) {
		if (dummy) {
			FS.writeFileSync(`${dir}/Manila.js`, this.parseTemplate(dummyProjectTemplate, {}))
		} else {
			FS.writeFileSync(`${dir}/Manila.js`, this.parseTemplate(projectTemplate, { name, namespace }))
		}
	}

	static createProject(p: ProjectParameters) {
		if (!FS.existsSync('.manila')) FS.mkdirSync('.manila')
		this.createDefaultManilaJSFile(p.name, p.namespace, '.', p.dummy)
	}
	static createModule(p: ModuleParameters) {
		if (!FS.existsSync(p.dir)) FS.mkdirSync(p.dir, { recursive: true })
		this.createDefaultManilaJSFile(p.name, `${p.namespace}.${p.name}`, p.dir)

		const srcDir = Path.join(p.dir, 'src')
		const binDir = Path.join(p.dir, 'bin')

		if (!FS.existsSync(srcDir)) FS.mkdirSync(srcDir, { recursive: true })
		if (!FS.existsSync(binDir)) FS.mkdirSync(binDir, { recursive: true })
	}

	static installPlugin(name: string) {}
}
