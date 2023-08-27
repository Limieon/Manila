// This class is used to hide away the api implementation

export const OS_NAMES = {
	aix: 'aix',
	darwin: 'MacOS',
	freebsd: 'FreeBSD',
	linux: 'Linux',
	openbsd: 'OpenBSD',
	sunos: 'SunOS',
	win32: 'Windows',
	android: 'Android'
}

export type ManilaConfig_OS = {
	name: string
	nodeName: string
}
export class ManilaConfig {
	constructor(config: string = 'Debug') {
		this.config = config
		this.platform = OS_NAMES[process.platform]
		this.nodePlatform = process.platform
		this.arch = process.arch
	}

	arch: string
	config: string
	platform: string
	nodePlatform: string
}

export class ManilaProject {
	name: string
	id: string
	namespace: string
	location: string
	author: string
}

export class ManilaWorkspace {
	name: string
	location: string
}

export default class ManilaWrapper {
	static init(config: string = 'Debug') {
		this.#conig = new ManilaConfig(config)
	}
	static setWorkspace(name: string, location: string) {
		this.#workspace = { name, location }
	}
	static setProject(id: string, name: string, namespace: string, location: string, author: string) {
		this.#project = {
			id,
			name,
			namespace,
			location,
			author
		}
	}

	static getProject() {
		return this.#project
	}
	static getWorkspace() {
		return this.#workspace
	}
	static getConfig() {
		return this.#conig
	}

	static #conig: ManilaConfig
	static #workspace: ManilaWorkspace
	static #project: ManilaProject
}
