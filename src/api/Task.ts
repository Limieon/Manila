import ScriptHook from '../ScriptHook.js'
import { Project } from '../BuildSystem.js'

/**
 * A task that can be executed
 */
export default class TaskBuilder {
	/**
	 * @param name the name of the task
	 */
	constructor(name: string) {
		if (ScriptHook.getCurrentProject() == undefined) {
			this.#name = `:${name}`
			this.#project = undefined
		} else {
			this.#name = `${ScriptHook.getCurrentProject().name}:${name}`
			this.#project = ScriptHook.getCurrentProject()
		}

		this.#dependencies = new Set()
		ScriptHook.registerTask(this)
	}

	/**
	 * Sets dependencies for this task
	 * @param names the dependencies
	 * @returns TaskBuilder
	 */
	dependsOn(...names: string[]): TaskBuilder {
		if (typeof names === 'string') names = [names]
		this.#dependencies = new Set([...this.#dependencies, ...names])

		return this
	}
	/**
	 * Sets the callback function
	 * @param callback the export function that will be executed when the task is run
	 * @returns TaskBuilder
	 */
	executes(callback: () => {}): TaskBuilder {
		this.#callback = callback
		return this
	}

	/**
	 * Gets the name of the task
	 */
	getName(): string {
		return this.#name
	}
	/**
	 * Gets the dependencies of the task
	 */
	getDependencies(): Set<string> {
		return this.#dependencies
	}
	/**
	 * Gets the callback of the task
	 */
	getCallback(): () => void {
		return this.#callback
	}
	/**
	 * Gets the project that contains the task
	 */
	getProject(): Project {
		return this.#project
	}
	/**
	 * Gets the task name with the prefixed project
	 */
	getRealName(): string {
		let temp = this.#name.split(':')
		return temp[temp.length - 1]
	}

	#name: string
	#dependencies: Set<string>
	#project: Project
	#callback: () => void
}
