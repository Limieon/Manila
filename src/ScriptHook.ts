import FS, { openAsBlob } from 'fs'
import Chalk from 'chalk'
import Path from 'path'
import { NodeVM, VM } from 'vm2'

import Utils from './Utils.js'
import BuildSystem, {
	Parameter,
	ParameterType,
	ModuleParameters,
	Plugin,
	ProjectDeclarator,
	ProjectDecleratorType,
	Project,
	ProjectParameters,
	PluginIndexFile,
	ScriptProperty,
	ScriptPropertyScope
} from './BuildSystem.js'
import FileUtils from './FileUtils.js'

import ImpManilaAPI from './api/Manila.js'

import TaskBuilder from './api/Task.js'

// Import API to make it available in the VM
import * as API from './api/API.js'
let VM_SANDBOX = {}

// Script Hook Class
export default class ScriptHook {
	static async run() {
		this.#tasks = {}
		this.#rootDir = process.cwd()
		this.#projects = []
		this.#plugins = BuildSystem.getPluginsConfig()
		this.#pluginModules = {}
		this.#parameters = []
		this.#lastFileNames = []
		this.#projectDeclarators = []
		this.#projectProperties = {}
		this.#secrets = {}

		this.#pendingProjectProperties = {}

		if (!FS.existsSync('./Manila.js')) {
			console.log(Chalk.red('Could not find Manila.js BuildScript!'))
			return -1
		}

		for (const k of Object.keys(this.#plugins)) {
			const plugin = this.#plugins[k]
			const file = `file://${Path.join(this.#rootDir, plugin.location, plugin.indexFile)}`
			this.#pluginModules[k] = await import(file)
		}

		for (const k of Object.keys(API)) {
			VM_SANDBOX[k] = API[k]
		}
		console.log(VM_SANDBOX)

		this.#scriptVM = new NodeVM({
			sandbox: VM_SANDBOX,
			allowAsync: true,
			strict: true
		})

		let secretsFile = FileUtils.getSecretFileFromRootDir()
		if (secretsFile) this.#secrets = JSON.parse(FS.readFileSync(secretsFile, { encoding: 'utf-8' }))

		await this.runFile(Path.join(process.cwd(), './Manila.js'))
		this.addMainProperties()

		ImpManilaAPI.setWorkspace(this.#projectProperties._['appName'], this.#rootDir)

		await this.runSubFiles(process.cwd(), true)
	}

	static async runSubFiles(dir: string, rootDir: boolean) {
		for (const f of FS.readdirSync(dir)) {
			if (FS.statSync(Path.join(dir, f)).isFile()) {
				if (f === 'Manila.js' && !rootDir) {
					let projectName = `:${Path.relative(this.#rootDir, dir).replaceAll('/', ':').replaceAll('\\', ':').toLowerCase()}`
					this.registerProject(projectName)
					await this.prepareForProjectScript(projectName)
					this.addProjectProperties(projectName)

					const props = this.#projectProperties[projectName]
					ImpManilaAPI.setProject(projectName, props['name'], props['namespace'], dir, props['author'])

					await this.runFile(Path.join(dir, 'Manila.js'))
					this.addProjectProperties(projectName)
				}
			} else {
				await this.runSubFiles(Path.join(dir, f), false)
			}
		}
	}

	static async runFile(file: string) {
		this.#fileIsProjectFile = false

		this.#lastFileNames.push(file)
		try {
			//await eval(`(async () => { ${FS.readFileSync(file, { encoding: 'utf-8' })} })()`)
			await this.#scriptVM.runFile(file)
		} catch (e) {
			throw e
		}
		this.#lastFileNames.pop()
	}

	static registerTask(task: TaskBuilder) {
		const name = task.getName()
		if (this.#tasks[name] != undefined) {
			throw new Error(`Task with name ${name} already has been defined!`)
		}

		this.#tasks[name] = task
	}

	static registerParameter(name: string, description: string, vDefault: any, type: ParameterType) {
		if (!this.#fileIsProjectFile) this.#parameters.push({ name, description, vDefault, type, project: undefined })
		else this.#parameters.push({ name, description, vDefault, type, project: this.#projects[this.#projects.length - 1] })
	}

	static getDependencies(task: TaskBuilder, deps = new Set<string>()): Set<string> {
		if (deps.has(task.getName())) return new Set<string>()
		deps.add(task.getName())

		const dependencies = this.#tasks[task.getName()]?.getDependencies() || []
		let allDependencies = new Set<string>(dependencies)

		dependencies.forEach((d: string) => {
			const subDep = this.getDependencies(this.#tasks[d], deps)
			allDependencies = new Set<string>([...allDependencies, ...subDep])
		})

		return allDependencies
	}

	static hasTask(name: string): boolean {
		return this.#tasks[name] != undefined
	}
	static async runTask(name: string): Promise<boolean> {
		const task = this.#tasks[name]
		if (task == undefined) return false

		task.getDependencies().forEach(d => {
			if (this.#tasks[d] == undefined) throw new Error(`Dependant Task ${d} could not be found!`)
		})

		const dependencies = this.getDependencies(task)

		dependencies.add(task.getName())
		let taskNum = 1

		for (const t of dependencies) {
			if (taskNum == dependencies.size) {
				console.log(
					`${Chalk.green(taskNum++)}${Chalk.gray('/')}${Chalk.cyan(dependencies.size)} ${Chalk.gray('>')} ${Chalk.blue(t)}`
				)
			} else {
				console.log(
					`${Chalk.yellow(taskNum++)}${Chalk.gray('/')}${Chalk.cyan(dependencies.size)} ${Chalk.gray('>')} ${Chalk.blue(t)}`
				)
			}
			try {
				await this.#tasks[t].getCallback()()
			} catch (e) {
				throw new Error(`Could not execute task ${Chalk.blue(name)}! (${e})`)
			}
		}

		return true
	}

	static async importPlugin(name: string): Promise<any> {
		const plugin = this.#plugins[name]
		if (plugin == undefined) throw new Error(`Plugin ${name} could not be found!`)
		let path = `file://${Path.join(this.#rootDir, plugin.location, plugin.indexFile)}`
		return (await import(path)).default
	}
	static async importPluginFromFile(name: string, dir: string): Promise<any> {
		const indexFile = FileUtils.getPluginIndexFileFromRoot(dir)

		if (!FS.existsSync(dir)) throw new Error(`Directory ${dir} does not exist!`)
		if (indexFile == undefined)
			throw new Error(`Cannot import plugin ${name}!\nDirectory ${dir} does not contain a valid plugin index file!`)
		const data: PluginIndexFile = JSON.parse(FS.readFileSync(Path.join(dir, indexFile), { encoding: 'utf-8' }))

		const plugin = data.plugins[name]
		if (plugin == undefined) throw new Error(`Plugin index file does not contain a plugin named ${name}!`)

		const path = `file://${Path.join(this.#rootDir, dir, name, plugin.index)}`
		return (await import(path)).default
	}

	static getLastFileName(): string {
		return this.#lastFileNames[this.#lastFileNames.length - 1]
	}
	static getLastFileNames(): string[] {
		return this.#lastFileNames
	}
	static getRootDir(): string {
		return this.#rootDir
	}

	static prettyPrintTasks() {
		const tasksObj: { [key: string]: TaskBuilder[] } = {}
		Object.keys(this.#tasks).forEach(key => {
			const t = this.#tasks[key]
			if (tasksObj[t.getProject().name] == undefined) tasksObj[t.getProject().name] = []
			tasksObj[t.getProject().name].push(t)
		})

		Object.keys(tasksObj).forEach(k => {
			console.log()

			const tasks = tasksObj[k]

			console.log(Chalk.magenta('Available Tasks:'), Chalk.gray(`(${k})`))
			tasks.forEach(t => {
				let hasDeps = t.getDependencies().size > 0
				let deps = [...t.getDependencies()]
				if (hasDeps) {
					console.log(`  ${Chalk.blue('-')} ${Chalk.gray(t.getName())} ${Chalk.cyan('->')} ${Chalk.gray(deps[0])}`)
				} else {
					console.log(`  ${Chalk.blue('-')} ${Chalk.gray(t.getName())}`)
				}

				for (let i = 1; i < deps.length; ++i) {
					console.log(' '.repeat(t.getName().length + 7), Chalk.gray(deps[i]))
				}
			})
		})
	}

	static prettyPrintParameters() {
		const projects: { [key: string]: string[][] } = {}

		this.#parameters.forEach(p => {
			let projectName = p.project == undefined ? 'global' : p.project.name
			if (projects[projectName] == undefined) projects[projectName] = []

			let type = ''
			switch (p.type) {
				case ParameterType.NUMBER:
					type = 'number'
					break
				case ParameterType.STRING:
					type = 'string'
			}

			if (p.type == ParameterType.BOOLEAN) {
				projects[projectName].push([`  ${Chalk.gray('--' + p.name)}`, p.description])
			} else {
				projects[projectName].push([
					`  ${Chalk.gray('--' + p.name)} <${Chalk.blue(type)}>   `,
					p.description + ' ' + Chalk.gray(`(Default: ${p.vDefault})   `)
				])
			}
		})

		Object.keys(projects).forEach(k => {
			console.log(Chalk.magenta('Available Parameters:'), Chalk.gray(`(${k})`))
			console.log(Utils.createTable(projects[k]))
		})
	}

	static prettyPrintProjects() {
		const table = [[Chalk.blue('Project'), Chalk.blue('Namespace'), Chalk.blue('Version'), Chalk.blue('ID')]]

		this.#projects.forEach(p => {
			table.push([
				`${Chalk.gray('-')} ${this.#projectProperties[p.name]['name']}`,
				this.#projectProperties[p.name]['namespace'],
				this.#projectProperties[p.name]['version'],
				p.name
			])
		})

		console.log(Utils.createTable(table, 3))
	}

	static registerProject(name: string) {
		this.#currentProject = name
		this.#projects.push({ name })
		this.#fileIsProjectFile = true
	}
	static setProjectParameters(namespace: string, version: string) {
		let i = this.#projects.length - 1
		this.#projects[i].namespace = namespace
		this.#projects[i].version = version
	}

	static addProjectDeclerator(declerator: ProjectDeclarator) {
		this.#projectDeclarators.push(declerator)
	}

	static async prepareForProjectScript(name: string) {
		return new Promise<void>((res, rej) => {
			this.#projectDeclarators.forEach(async (p, i) => {
				if (p.type === ProjectDecleratorType.REGEXP) {
					if ((p.filter as RegExp).test(name)) await p.func()
				}
				if (p.type === ProjectDecleratorType.STRING_ARRAY) {
					if ((p.filter as string[]).includes(name)) await p.func()
				}
				if (p.type === ProjectDecleratorType.STRING) {
					if ((p.filter as string) == name) await p.func()
				}

				// Resolve the promise after the last element has been iterated
				if (this.#projectDeclarators.length == i + 1) res()
			})
		})
	}

	static addProjectProperties(name: string) {
		if (this.#projectProperties[name] == undefined) this.#projectProperties[name] = {}

		for (const key of Object.keys(this.#pendingProjectProperties)) {
			let prop: ScriptProperty = undefined
			for (const p of this.#scriptProperties) {
				if (p.name == key) {
					prop = p
					break
				}
			}

			if (prop == undefined) throw new Error(`Property '${key}' is not registered!`)
			if (prop.scope != ScriptPropertyScope.PROJECT && prop.scope != ScriptPropertyScope.COMMON)
				throw new Error(`Property '${key}' is not avilable in project or common scope!`)

			this.#projectProperties[name][key] = this.#pendingProjectProperties[key]
		}

		this.#pendingProjectProperties = {}
	}
	static addMainProperties() {
		if (this.#projectProperties['_'] == undefined) this.#projectProperties['_'] = {}

		for (const key of Object.keys(this.#pendingProjectProperties)) {
			let prop: ScriptProperty = undefined
			for (const p of this.#scriptProperties) {
				if (p.name == key) {
					prop = p
					break
				}
			}

			if (prop == undefined) throw new Error(`Property '${key}' is not registered!`)
			if (prop.scope != ScriptPropertyScope.MAIN && prop.scope != ScriptPropertyScope.COMMON)
				throw new Error(`Property '${key}' is not avilable in main or common scope!`)

			this.#projectProperties['_'][key] = this.#pendingProjectProperties[key]
		}

		this.#pendingProjectProperties = {}
	}

	static setProperties(values: object) {
		for (const k of Object.keys(values)) this.#pendingProjectProperties[k] = values[k]
	}

	static registerScriptProperty(name: string, description: string, scope: ScriptPropertyScope) {
		this.#scriptProperties.push({ name, description, scope })
	}

	static getProperty(name: string) {
		let prop = undefined
		for (const p of this.#scriptProperties) {
			if (p.name == name) {
				prop = p
				break
			}
		}

		if (prop == undefined) throw new Error(`Cannot find property named '${name}'!`)
		if (prop.scope != ScriptPropertyScope.PROJECT && prop.scope != ScriptPropertyScope.COMMON)
			throw new Error(`Property named '${name}' does not exist in project or common scope!`)

		return this.#projectProperties[this.#currentProject][name]
	}

	static getCurrentProject(): Project {
		return this.getProjectByID(this.#currentProject)
	}

	static getProjectByID(id: string) {
		for (const p of this.#projects) {
			if (p.name == id) return p
		}
		return undefined
	}

	static getPlugin(name: string) {
		if (this.#pluginModules[name] == undefined) throw new Error(`Could not find plugin '${name}'!`)
		return this.#pluginModules[name]
	}

	static getSecrets(): object {
		return this.#secrets
	}

	static #currentProject: string = undefined
	static #plugins: { [key: string]: Plugin }
	static #pluginModules: { [key: string]: any }
	static #parameters: Parameter[]
	static #projects: Project[]
	static #fileIsProjectFile: boolean
	static #tasks: { [key: string]: TaskBuilder }
	static #lastFileNames: string[]
	static #rootDir: string
	static #projectDeclarators: ProjectDeclarator[]
	static #scriptProperties: ScriptProperty[] = []
	static #projectProperties: { [key: string]: object }
	static #secrets: object

	static #scriptVM: NodeVM

	// Properties are written to this and then copied to the project
	static #pendingProjectProperties: object
}

// Projects
ScriptHook.registerScriptProperty('namespace', 'the default namespace', ScriptPropertyScope.PROJECT)
ScriptHook.registerScriptProperty('version', 'the version', ScriptPropertyScope.PROJECT)
ScriptHook.registerScriptProperty('author', 'the name of the author', ScriptPropertyScope.PROJECT)
ScriptHook.registerScriptProperty('name', 'name of the project', ScriptPropertyScope.PROJECT)

// Main
ScriptHook.registerScriptProperty('appName', 'the workspace name', ScriptPropertyScope.MAIN)

// Common
