import FS, { Dirent, copyFileSync } from 'fs'
import Chalk from 'chalk'
import { Command } from 'commander'
import Path from 'path'

import Logger from './Logger.js'
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
import FileNames from './FileNames.js'

import ManilaWrapper from './ManilaWrapper.js'

// This exposes the api given from 'ManilaWrapper'
class Manila {
	static getProject() {
		return ManilaWrapper.getProject()
	}
	static getWorkspace() {
		return ManilaWrapper.getWorkspace()
	}
	static getConfig() {
		return ManilaWrapper.getConfig()
	}
}

class TaskBuilder {
	constructor(name: string) {
		this.#name = `${ScriptHook.getCurrentProject().name}:${name}`
		this.#project = ScriptHook.getCurrentProject()

		this.#dependencies = new Set()
		ScriptHook.registerTask(this)
	}

	dependsOn(...names: string[]) {
		if (typeof names === 'string') names = [names]
		this.#dependencies = new Set([...this.#dependencies, ...names])

		return this
	}
	executes(callback: () => {}) {
		this.#callback = callback

		return this
	}

	getName(): string {
		return this.#name
	}
	getDependencies(): Set<string> {
		return this.#dependencies
	}
	getCallback(): () => void {
		return this.#callback
	}
	getProject(): Project {
		return this.#project
	}
	getRealName(): string {
		let temp = this.#name.split(':')
		return temp[temp.length - 1]
	}

	#name: string
	#dependencies: Set<string>
	#project: Project
	#callback: () => void
}

// Global Script Functions
function task(name: string) {
	return new TaskBuilder(name)
}
function parameterBoolean(name: string, description: string) {
	ScriptHook.registerParameter(name, description, undefined, ParameterType.BOOLEAN)
	return process.argv.indexOf(`--${name}`) > -1
}
function parameterString(name: string, description: string, vDefault: string) {
	ScriptHook.registerParameter(name, description, vDefault, ParameterType.STRING)
	const index = process.argv.indexOf(`--${name}`)
	if (index > -1) {
		const val = process.argv[index + 1]
		return val == undefined ? vDefault : val
	}

	return vDefault
}
function parameterNumber(name: string, description: string, vDefault: number | string) {
	ScriptHook.registerParameter(name, description, vDefault, ParameterType.NUMBER)
	const index = process.argv.indexOf(`--${name}`)
	if (index > -1) {
		const val = process.argv[index + 1]
		if (val == undefined) return vDefault

		const num = Number(val)
		if (isNaN(num)) {
			Logger.error(`Parameter ${Chalk.gray('--')}${Chalk.blue(name)} was expected as a number, but got: ${Chalk.cyan(val)}!`)
			process.exit(-1)
		}

		return num
	}

	return vDefault
}

function print(...msg: string[]) {
	Logger.script(msg.join(' '))
}

async function importPlugin(name: string, dir?: string): Promise<any> {
	if (dir == undefined) return await ScriptHook.importPlugin(name)
	return await ScriptHook.importPluginFromFile(name, dir)
}

function getProperty(name: string) {
	return ScriptHook.getProperty(name)
}

function project(filter: RegExp | string | string[], func: () => void) {
	let type: ProjectDecleratorType = undefined

	if (filter instanceof RegExp) {
		type = ProjectDecleratorType.REGEXP
	}
	if (typeof filter === 'string') {
		type = ProjectDecleratorType.STRING
	}
	if (Array.isArray(filter) && filter.every(item => typeof item === 'string')) {
		type = ProjectDecleratorType.STRING_ARRAY
	}

	if (type == undefined) throw new Error('The filter attribute must either be a regexp, string, or string array!')

	ScriptHook.addProjectDeclerator({ filter, func, type })
}

// Script Hook Class
export default class ScriptHook {
	static async run() {
		this.#tasks = {}
		this.#rootDir = process.cwd()
		this.#projects = []
		this.#plugins = BuildSystem.getPluginsConfig()
		this.#parameters = []
		this.#lastFileNames = []
		this.#projectDeclarators = []
		this.#projectProperties = {}

		if (!FS.existsSync('./Manila.js')) {
			console.log(Chalk.red('Could not find Manila.js BuildScript!'))
			return -1
		}

		await this.runFile(Path.join(process.cwd(), './Manila.js'))
		this.addMainProperties()

		await this.runSubFiles(process.cwd(), true)
	}

	static async runSubFiles(dir: string, rootDir: boolean) {
		for (const f of FS.readdirSync(dir)) {
			if (FS.statSync(Path.join(dir, f)).isFile()) {
				if (f === 'Manila.js' && !rootDir) {
					let projectName = `:${Path.relative(this.#rootDir, dir).replaceAll('/', ':').replaceAll('\\', ':').toLowerCase()}`
					this.registerProject(projectName)
					await this.prepareForProjectScript(projectName)
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
			await eval(`(async () => { ${FS.readFileSync(file, { encoding: 'utf-8' })} })()`)
		} catch (e) {
			throw e
		}
		this.#lastFileNames.pop()

		if (this.#fileIsProjectFile) this.setProjectParameters(namespace, version)
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
	static runTask(name: string): boolean {
		const task = this.#tasks[name]
		if (task == undefined) return false

		task.getDependencies().forEach(d => {
			if (this.#tasks[d] == undefined) throw new Error(`Dependant Task ${d} could not be found!`)
		})

		const dependencies = this.getDependencies(task)

		dependencies.add(task.getName())
		let taskNum = 1

		dependencies.forEach(t => {
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
				this.#tasks[t].getCallback()()
			} catch (e) {
				throw new Error(`Could not execute task ${Chalk.blue(name)}! (${e})`)
			}
		})

		return true
	}

	static async importPlugin(name: string): Promise<any> {
		const plugin = this.#plugins[name]
		if (plugin == undefined) throw new Error(`Plugin ${name} could not be found!`)
		let path = `file://${Path.join(this.#rootDir, plugin.location, plugin.indexFile)}`
		return (await import(path)).default
	}
	static async importPluginFromFile(name: string, dir: string): Promise<any> {
		const indexFile = FileNames.pluginIndexFile(dir)

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

	// __name__ to not overwrite any script property that is named 'name'
	static addProjectProperties(__name__: string) {
		let obj = {}
		this.#scriptProperties.forEach(p => {
			if (p.scope === ScriptPropertyScope.PROJECT || p.scope === ScriptPropertyScope.COMMON) eval(`obj['${p.name}'] = ${p.name}`)
		})
		this.#projectProperties[__name__] = obj
	}
	static addMainProperties() {
		let obj = {}
		this.#scriptProperties.forEach(p => {
			if (p.scope === ScriptPropertyScope.MAIN || p.scope === ScriptPropertyScope.COMMON) eval(`obj['${p.name}'] = ${p.name}`)
		})
		this.#projectProperties['_'] = obj
	}

	static registerScriptProperty(name: string, scope: ScriptPropertyScope) {
		console.log(this.#scriptProperties)
		this.#scriptProperties.push({ name, scope })
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
		this.#projects.forEach(p => {
			if (p.name == id) return p
		})
		return undefined
	}

	static #currentProject: string = undefined
	static #plugins: { [key: string]: Plugin }
	static #parameters: Parameter[]
	static #projects: Project[]
	static #fileIsProjectFile: boolean
	static #tasks: { [key: string]: TaskBuilder }
	static #lastFileNames: string[]
	static #rootDir: string
	static #projectDeclarators: ProjectDeclarator[]
	static #scriptProperties: ScriptProperty[] = []
	static #projectProperties: { [key: string]: object }
}

// Global Variables that have to be defined by scripts

// Projects
ScriptHook.registerScriptProperty('namespace', ScriptPropertyScope.PROJECT)
let namespace = undefined

ScriptHook.registerScriptProperty('version', ScriptPropertyScope.PROJECT)
let version = undefined

ScriptHook.registerScriptProperty('author', ScriptPropertyScope.PROJECT)
let author = undefined

// Common
ScriptHook.registerScriptProperty('name', ScriptPropertyScope.COMMON)
let name = undefined

// Main
ScriptHook.registerScriptProperty('appName', ScriptPropertyScope.MAIN)
let appName = undefined
