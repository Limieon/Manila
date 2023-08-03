import FS, { copyFileSync } from 'fs'
import Chalk from 'chalk'
import { Command } from 'commander'
import Path from 'path'

import Logger from './Logger.js'
import Utils from './Utils.js'
import Manila, { Parameter, ParameterType, ModuleParameters, Plugin, Project, ProjectParameters } from './Manila.js'

class TaskBuilder {
	constructor(name: string) {
		const temp = Path.relative(ScriptHook.getRootDir(), ScriptHook.getLastFileName()).split('\\')
		if (temp.length > 1) {
			this.#name = `:${temp[0].toLowerCase()}:${name}`
		} else {
			this.#name = `:${name}`
		}

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

	getName() {
		return this.#name
	}
	getDependencies() {
		return this.#dependencies
	}
	getCallback() {
		return this.#callback
	}

	#name: string
	#dependencies: Set<string>
	#callback: () => void
}

// Global Script Variables
let namespace = undefined
let version = undefined

// Global Script Functions
function task(name: string) {
	return new TaskBuilder(name)
}
function project(name: string) {
	ScriptHook.registerProject(name)
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

async function importPlugin(name: string): Promise<any> {
	return await ScriptHook.importPlugin(name)
}

// Script Hook Class
export default class ScriptHook {
	static run(app: Command) {
		this.#app = app
		this.#tasks = {}
		this.#rootDir = process.cwd()
		this.#projects = []
		this.#plugins = Manila.getPluginsConfig()
		this.#parameters = []
		this.#lastFileNames = []

		if (!FS.existsSync('./Manila.js')) {
			console.log(Chalk.red('Could not find Manila.js BuildScript!'))
			return -1
		}

		this.runFile(Path.join(process.cwd(), './Manila.js'))

		this.runSubFiles(process.cwd(), true)
	}

	static runSubFiles(dir: string, rootDir: boolean) {
		FS.readdirSync(dir).forEach((f) => {
			if (FS.statSync(Path.join(dir, f)).isFile()) {
				if (f === 'Manila.js' && !rootDir) {
					this.runFile(Path.join(dir, 'Manila.js'))
				}
			} else {
				this.runSubFiles(Path.join(dir, f), false)
			}
		})
	}

	static async runFile(file: string) {
		namespace = undefined
		version = undefined
		this.#fileIsProjectFile = false

		this.#lastFileNames.push(file)
		try {
			eval(`(async () => { ${FS.readFileSync(file, { encoding: 'utf-8' })} })()`)
		} catch (e) {
			throw e
		}
		this.#lastFileNames.pop()

		if (this.#fileIsProjectFile) this.setProjectParameters(namespace, version)
	}

	static registerTask(task: TaskBuilder) {
		const name = task.getName()
		if (this.#tasks[name] != undefined) {
			Logger.error(`Task with name ${name} already has been defined!`)
			process.exit(-1)
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

		const dependencies = this.getDependencies(task)

		dependencies.add(task.getName())
		let taskNum = 1

		dependencies.forEach((t) => {
			if (taskNum == dependencies.size) {
				console.log(
					`${Chalk.green(taskNum++)}${Chalk.gray('/')}${Chalk.cyan(dependencies.size)} ${Chalk.gray('>')} ${Chalk.blue(t)}`
				)
			} else {
				console.log(
					`${Chalk.yellow(taskNum++)}${Chalk.gray('/')}${Chalk.cyan(dependencies.size)} ${Chalk.gray('>')} ${Chalk.blue(t)}`
				)
			}
			this.#tasks[t].getCallback()()
		})

		return true
	}

	static async importPlugin(name: string): Promise<any> {
		const plugin = this.#plugins[name]
		if (plugin == undefined) throw new Error(`Plugin ${name} could not be found!`)
		let path = `file://${Path.join(this.#rootDir, '.manila', 'plugins', name, plugin.indexFile)}`
		return (await import(path)).default
	}

	static getApp(): Command {
		return this.#app
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
		const tasksNames = Object.keys(this.#tasks)
		const namespaces = ['']

		tasksNames.forEach((t) => {
			const temp = t.split(':')
			if (temp.length > 2) {
				namespaces.push(temp[1])
			}
		})

		namespaces.forEach((n) => {
			const nsName = n == '' ? 'global' : n

			console.log(Chalk.magenta('Available Tasks:'), Chalk.gray(`(${nsName})`))
			tasksNames.forEach((t) => {
				// Check that the task (t) is inside the namespace (n)
				const temp = t.split(':')
				let ns = ''
				if (temp.length > 2) {
					ns = temp[1]
				}

				if (ns != n) return

				const task = this.#tasks[t]
				const dependencies = task.getDependencies()

				const text = `  ${Chalk.blue('-')} ${Chalk.gray(t)}`
				if (dependencies.size < 1) {
					console.log(text)
				} else {
					let first = true
					task.getDependencies().forEach((d) => {
						if (first) console.log(`${text} ${Chalk.cyan('->')} ${Chalk.gray(d)}`)
						else console.log(' '.repeat(`  - ${t}   `.length), Chalk.gray(d))

						first = false
					})
				}
			})
			console.log()
		})
	}

	static prettyPrintParameters() {
		const projects: { [key: string]: string[][] } = {}

		this.#parameters.forEach((p) => {
			let projectName = p.project == undefined ? 'global' : p.project.name
			if (projects[projectName] == undefined) projects[projectName] = []

			let type = ''
			switch (p.type) {
				case ParameterType.NUMBER:
					type = 'number'
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

		Object.keys(projects).forEach((k) => {
			console.log(Chalk.magenta('Available Parameters:'), Chalk.gray(`(${k})`))
			console.log(Utils.createTable(projects[k]))
		})
	}

	static prettyPrintProjects() {
		const table = [[Chalk.blue('Project'), Chalk.blue('Namespace'), Chalk.blue('Version')]]

		this.#projects.forEach((p) => {
			table.push([`${Chalk.gray('-')} ${p.name}`, p.namespace, p.version])
		})

		console.log(Utils.createTable(table, 3))
	}

	static registerProject(name: string) {
		this.#projects.push({ name })
		this.#fileIsProjectFile = true
	}
	static setProjectParameters(namespace: string, version: string) {
		let i = this.#projects.length - 1
		this.#projects[i].namespace = namespace
		this.#projects[i].version = version
	}

	static #app: Command
	static #plugins: { [key: string]: Plugin }
	static #parameters: Parameter[]
	static #projects: Project[]
	static #fileIsProjectFile: boolean
	static #tasks: { [key: string]: TaskBuilder }
	static #lastFileNames: string[]
	static #rootDir: string
}
