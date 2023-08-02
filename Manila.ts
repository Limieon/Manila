import FS from 'fs'
import Path from 'path'
import * as YAML from 'yaml'

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

const settingsTemplate = `# Here you can specify your local Manila settings
#
# Specify your target .NET version
dotnet: 7.0
`

export default class Manila {
	static init() {
		this.#settings = {}

		// Load settings file if it exists
		let fileName = this.#getSettingsFileName()
		if (fileName != undefined) this.#settings = YAML.parse(FS.readFileSync(fileName, { encoding: 'utf-8' }))
	}

	static #getSettingsFileName(): string {
		return FS.existsSync('./settings.manila')
			? 'settings.manila'
			: FS.existsSync('./settings.manila.yml')
			? 'settings.manila.yml'
			: FS.existsSync('./settings.manila.yaml')
			? 'settings.manila.yaml'
			: undefined
	}

	static getSetting(key: string, vDefault: any = undefined): any {
		return this.#settings[key] != undefined ? this.#settings[key] : vDefault
	}
	static getSetting_string(key: string, vDefault: string) {
		if (this.#settings[key] == undefined) return vDefault

		let setting: any = this.#settings[key]
		if (typeof setting !== 'string') throw new Error("Setting 'pluginMirros' has to be a string!")
		return setting
	}
	static getSetting_stringList(key: string, vDefault: string[]): string[] {
		if (this.#settings[key] == undefined) return vDefault
		let setting: any[] = this.#settings[key]

		if (typeof setting !== 'object') throw new Error("Setting 'pluginMirros' has to be a string list!")
		setting.forEach((s) => {
			if (typeof s !== 'string') throw new Error("Setting 'pluginMirros' has to be a string list!")
		})

		return setting
	}

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

	static createDefaultSettingsFile(name: string, namespace: string) {
		FS.writeFileSync('./settings.manila.yml', this.parseTemplate(settingsTemplate, {}), { encoding: 'utf-8' })
	}

	static createProject(p: ProjectParameters) {
		if (!FS.existsSync('.manila')) FS.mkdirSync('.manila')
		this.createDefaultManilaJSFile(p.name, p.namespace, '.', p.dummy)
		this.createDefaultSettingsFile(p.name, p.namespace)
	}
	static createModule(p: ModuleParameters) {
		if (!FS.existsSync(p.dir)) FS.mkdirSync(p.dir, { recursive: true })
		this.createDefaultManilaJSFile(p.name, `${p.namespace}.${p.name}`, p.dir)

		const srcDir = Path.join(p.dir, 'src')
		const binDir = Path.join(p.dir, 'bin')

		if (!FS.existsSync(srcDir)) FS.mkdirSync(srcDir, { recursive: true })
		if (!FS.existsSync(binDir)) FS.mkdirSync(binDir, { recursive: true })
	}

	static installPlugin(name: string, silent: boolean) {}

	static #settings: object
}
