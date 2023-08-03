import FS, { copyFile } from 'fs'
import Path from 'path'
import YAML from 'yaml'

export type ProjectParameters = {
	name: string
	namespace: string
	dummy: boolean
}

export type ModuleParameters = {
	name: string
	namespace: string
	dir: string
}

export enum ParameterType {
	BOOLEAN,
	STRING,
	NUMBER
}

export type Parameter = {
	name: string
	description: string
	vDefault: any
	type: ParameterType
	project: Project
}

export type Project = {
	name: string
	namespace?: string
	version?: string
}

export type PlujginConfig = { [key: string]: Plugin }

export type Plugin = {
	version: string
	indexFile: string
	location: string
}

export type PluginIndex = {
	name: string
	version: string
	index: string
}
export type PluginIndexFile = {
	name: string
	plugins: PluginIndex[]
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

const settingsTemplate = `# This is your manila settings file
# You can configure your Manila settings in this file

# Add plugin index file to install 3rd-Party plugins
pluginRepositories:
- https://raw.githubusercontent.com/Limieon/Manila-Plugins/main/index.manila.json

# Specify your target dotnet version
dotnet: 7.0
`

export default class Manila {
	static init() {
		let settingsFileName = this.#getSettingsFileName()
		this.#settings = settingsFileName != undefined ? YAML.parse(FS.readFileSync(settingsFileName, { encoding: 'utf-8' })) : {}
		this.#plugins = FS.existsSync('./.manila/plugins.manila.json')
			? JSON.parse(FS.readFileSync('./.manila/plugins.manila.json', { encoding: 'utf-8' }))
			: { plugins: {} }
	}

	static getSettingsConfig(): object {
		return this.#settings
	}

	static updatePluginsConfig(config: PlujginConfig) {
		this.#plugins = config
		FS.writeFileSync('./.manila/plugins.manila.json', JSON.stringify(config, null, 4))
	}
	static getPluginsConfig(): PlujginConfig {
		return this.#plugins
	}

	static #getSettingsFileName(): string {
		const fileNames = ['./.manila/settings.manila', './.manila/settings.manila.yaml', './.manila/settings.manila.yml']
		for (const f of fileNames) {
			if (FS.existsSync(f)) return f
		}
		return undefined
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

	static parseTemplate(template: string, replace: object = {}): string {
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
		FS.writeFileSync('./.manila/settings.manila', this.parseTemplate(settingsTemplate, {}), { encoding: 'utf-8' })
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
	static #plugins: { [key: string]: Plugin }
}
