import ScriptHook from '../ScriptHook.js'
import ImpManilaAPI, { ManilaProject, ManilaWorkspace, ManilaConfig } from './Manila.js'
import Logger from '../Logger.js'
import { ParameterType, ProjectDecleratorType } from '../BuildSystem.js'
import TaskBuilder from './Task.js'

import { ManilaDirectory, ManilaFile } from './FileSystem.js'

import Chalk from 'chalk'

export default {}

// This exposes the api given from 'ImpManilaAPI'
export class Manila {
	/**
	 * Returns the current project
	 */
	static getProject(): ManilaProject {
		return ImpManilaAPI.getProject()
	}
	/**
	 * Returns the current workspace
	 */
	static getWorkspace(): ManilaWorkspace {
		return ImpManilaAPI.getWorkspace()
	}
	/**
	 * Returns the current build configuration
	 */
	static getConfig(): ManilaConfig {
		return ImpManilaAPI.getConfig()
	}

	/**
	 * Returns a new file instance
	 * @param path the path of the file
	 * @returns ManilaFile
	 */
	static file(...path: string[]): ManilaFile {
		return ImpManilaAPI.file(...path)
	}

	/**
	 * Returns a new directory instance
	 * @param path the path of the directory
	 * @returns ManilaDirectory
	 */
	static dir(...path: string[]): ManilaDirectory {
		return ImpManilaAPI.directory(...path)
	}
	/**
	 * Returns a new directory instance
	 * @param path the path of the directory
	 * @returns ManilaDirectory
	 */
	static directory(...path: string[]): ManilaDirectory {
		return ImpManilaAPI.directory(...path)
	}
}

/**
 * Create a new task
 * @param name name of the task
 * @returns TaskBuilder
 */
export function task(name: string): TaskBuilder {
	return new TaskBuilder(name)
}
/**
 * Creates a new boolean typed parameter
 * @param name the name
 * @param description the description
 * @returns the value of the parameter or false
 */
export function parameterBoolean(name: string, description: string): boolean {
	ScriptHook.registerParameter(name, description, undefined, ParameterType.BOOLEAN)
	return process.argv.indexOf(`--${name}`) > -1
}
/**
 * Creates a new string typed parameter
 * @param name the name
 * @param description the description
 * @returns the value of the parameter or the default
 */
export function parameterString(name: string, description: string, vDefault: string): string {
	ScriptHook.registerParameter(name, description, vDefault, ParameterType.STRING)
	const index = process.argv.indexOf(`--${name}`)
	if (index > -1) {
		const val = process.argv[index + 1]
		return val == undefined ? vDefault : val
	}

	return vDefault
}
/**
 * Creates a new number typed parameter
 * @param name the name
 * @param description the description
 * @returns the value of the parameter or the default
 */
export function parameterNumber(name: string, description: string, vDefault: number): number {
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

/**
 * Prints to the console
 * @param msg the msg to print
 */
export function print(...msg: any[]) {
	if (msg.length < 1 || (msg.length == 1 && !msg[0])) {
		console.log()
		return
	}

	for (let i = 0; i < msg.length; ++i) {
		if (`${msg[i]}` == '[object Object]') msg[i] = `${Chalk.yellow(msg[i].constructor.name)}`
	}
	Logger.script(msg.join(' '))
}

/**
 * Import a external plugin into this buildscript
 * @param name the name of the plugin
 * @param dir the directory containing the plugin
 * @returns the default export of the plugin
 */
export function importPlugin(name: string, ...members: string[]): any {
	const plugin = ScriptHook.getPlugin(name)
	console.log(members)

	if (members.length > 1) {
		const obj = {}
		for (const m of members) {
			obj[m] = plugin[m]
		}
		return obj
	}
	if (members == undefined || members.length < 1) {
		return plugin.default
	}

	return plugin[members[0]]
}

/**
 * Get a project or workspace property
 * @param name the key of the property
 * @returns the value of the property
 */
export function getProperty(name: string) {
	return ScriptHook.getProperty(name)
}

/**
 * A project declerator to set properties on a specific project
 * @param filter the project to filter for
 * @param func the funct to set properties for the project
 */
export function project(filter: RegExp | string | string[], func: () => void) {
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

/**
 * Sets properties for the current scope
 * @param values the properties to set
 */
export function properties(values: object) {
	ScriptHook.setProperties(values)
}
