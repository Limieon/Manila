import Path from 'path'
import FS from 'fs'

import { ManilaDirectory, ManilaFile } from './FileSystem.js'
import ManilaTimer from './Utils.js'
import ScriptHook from '../ScriptHook.js'
import Utils from '../Utils.js'

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
	location: ManilaDirectory
	author: string
}

export class ManilaWorkspace {
	name: string
	location: ManilaDirectory
}

export default class ImplManilaAPI {
	static init(config: string = 'Debug') {
		this.#storages = FS.existsSync(Path.join(process.cwd(), '.manila', 'storage.manila'))
			? JSON.parse(FS.readFileSync(Path.join(process.cwd(), '.manila', 'storage.manila'), { encoding: 'utf-8' }))
			: {}

		this.#conig = new ManilaConfig(config)
	}
	static setWorkspace(name: string, location: string) {
		this.#workspace = { name, location: new ManilaDirectory(location) }
	}
	static setProject(id: string, name: string, namespace: string, location: string, author: string) {
		this.#project = {
			id,
			name,
			namespace,
			location: new ManilaDirectory(location),
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

	// Utility Functions
	static file(...file: string[]): ManilaFile {
		return new ManilaFile(...file)
	}
	static directory(...dir: string[]): ManilaDirectory {
		return new ManilaDirectory(...dir)
	}
	static timer(): ManilaTimer {
		return new ManilaTimer()
	}

	static formatDuration(duration: number): string {
		return Utils.stringifyDuration(duration)
	}

	static getStorage(id: string): object {
		if (this.#storages[id] == undefined) this.#storages[id] = {}
		return this.#storages[id]
	}
	static setStorage(id: string, data: object) {
		this.#storages[id] = { ...data }
		FS.writeFileSync(Path.join(ScriptHook.getRootDir(), '.manila', 'storage.manila'), JSON.stringify(this.#storages))
	}

	static #conig: ManilaConfig
	static #workspace: ManilaWorkspace
	static #project: ManilaProject
	static #storages: { [key: string]: object }
}
