import FS from 'fs'
import Chalk from 'chalk'
import { Command } from 'commander'
import Logger from './Logger.js'
import Path from 'path'

class TaskBuilder {
	constructor(name) {
		const temp = Path.relative(ScriptHook.getRootDir(), ScriptHook.getLastFileName()).split('\\')
		if (temp.length > 1) {
			this.#name = `:${temp[0].toLowerCase()}:${name}`
		} else {
			this.#name = `:${name}`
		}

		this.#dependencies = []
		ScriptHook.registerTask(this)
	}

	dependsOn(...names) {
		if (typeof names === 'string') names = [names]
		this.#dependencies = new Set([...this.#dependencies, ...names])

		return this
	}
	executes(callback) {
		this.#callback = callback

		return this
	}

	getName() { return this.#name }
	getDependencies() { return this.#dependencies }
	getCallback() { return this.#callback }

	#name
	#dependencies
	#callback
}

// Global Script Functions
function task(name) {
	return new TaskBuilder(name)
}
function parameterBoolean(name, description) {
	return process.argv.indexOf(`--${name}`) > -1
}
function parameterString(name, description, vDefault) {
	const index = process.argv.indexOf(`--${name}`)
	if (index > -1) {
		const val = process.argv[index + 1]
		return val == undefined ? vDefault : val
	}

	return vDefault
}
function parameterNumber(name, description, vDefault) {
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

function print(...msg) {
	Logger.script(Path.relative(ScriptHook.getRootDir(), ScriptHook.getLastFileName()), msg.join('  '))
}

// Script Hook Class
export default class ScriptHook {
	static run(app) {
		ScriptHook.#app = app
		ScriptHook.#tasks = {}
		ScriptHook.#rootDir = process.cwd()
		ScriptHook.#lastFileNames = []

		if (!FS.existsSync('./Manila.js')) {
			console.log(Chalk.red('Could not find Manila.js BuildScript!'))
			return -1
		}

		this.runFile('./Manila.js')

		ScriptHook.runSubFiles(process.cwd(), true)
	}

	static runSubFiles(dir, rootDir) {
		FS.readdirSync(dir).forEach(f => {
			if (FS.statSync(Path.join(dir, f)).isFile()) {
				if (f === 'Manila.js' && !rootDir) {
					this.runFile(Path.join(dir, 'Manila.js'))
				}
			} else {
				this.runSubFiles(Path.join(dir, f), false)
			}
		})
	}

	static runFile(file) {
		Logger.info(`Running file ${Path.relative(this.#rootDir, file)}...`)

		this.#lastFileNames.push(file)
		eval(FS.readFileSync(file, { encoding: 'utf-8' }))
		this.#lastFileNames.pop()
	}

	static registerTask(task) {
		const name = task.getName()
		if (ScriptHook.#tasks[name] != undefined) {
			Logger.error(`Task with name ${name} already has been defined!`)
			process.exit(-1)
		}

		ScriptHook.#tasks[name] = task
		ScriptHook.#app.command(name).action(() => ScriptHook.runTask(name))
	}

	static runTask(name) {
		const task = ScriptHook.#tasks[name]
		const dependencies = task.getDependencies()

		dependencies.forEach(d => ScriptHook.runTask(d))
		task.getCallback()()
	}

	static getApp() {
		return ScriptHook.#app
	}
	static getLastFileName() {
		return ScriptHook.#lastFileNames[ScriptHook.#lastFileNames.length - 1]
	}
	static getRootDir() {
		return ScriptHook.#rootDir
	}

	static prettyPrintTasks() {
		const tasksNames = Object.keys(this.#tasks)
		const namespaces = ['']

		tasksNames.forEach(t => {
			const temp = t.split(':')
			if (temp.length > 2) {
				namespaces.push(temp[1])
			}
		})

		namespaces.forEach(n => {
			const nsName = n == '' ? 'global' : n

			console.log(Chalk.magenta('Available Tasks:'), Chalk.gray(`(${nsName})`))
			tasksNames.forEach(t => {
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
				if (dependencies.length < 1) {
					console.log(text)
				} else {
					let first = true
					task.getDependencies().forEach((d) => {

						if (first)
							console.log(`${text} ${Chalk.cyan('->')} ${Chalk.gray(d)}`)
						else
							console.log(' '.repeat(`  - ${t}   `.length), Chalk.gray(d))

						first = false
					})
				}
			})
			console.log()
		})
	}

	static prettyPrintParameters() {
	}

	static #app
	static #parameters
	static #tasks
	static #lastFileNames
	static #rootDir
}
