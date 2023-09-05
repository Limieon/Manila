import Path from 'path'
import FS from 'fs'

import { ManilaDirectory, ManilaFile } from './FileSystem.js'
import ManilaTimer from './Utils.js'
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

	static #conig: ManilaConfig
	static #workspace: ManilaWorkspace
	static #project: ManilaProject
}
