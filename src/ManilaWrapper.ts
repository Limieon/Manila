import Path from 'path'
import FS from 'fs'
import ScriptHook from './ScriptHook.js'

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

export class ManilaFile {
	constructor(...f: string[]) {
		const path = Path.join(...f)
		if (Path.isAbsolute(path)) this.#path = path
		else this.#path = Path.join(process.cwd(), path)

		if (FS.existsSync(this.#path) && FS.lstatSync(this.#path).isDirectory())
			throw new Error(`Path '${this.#path}' is a directory, not a file!`)
	}

	exists(): boolean {
		return FS.existsSync(this.#path)
	}
	write(data: string | Buffer): ManilaFile {
		FS.writeFileSync(this.#path, data)
		return this
	}
	read(encoding: BufferEncoding | null | undefined = 'utf-8'): string | Buffer {
		return FS.readFileSync(this.#path, { encoding: encoding })
	}

	delete(): ManilaFile {
		FS.rmSync(this.#path)
		return this
	}
	copy(...to: string[]): ManilaFile {
		FS.copyFileSync(this.#path, Path.join(...to))
		return this
	}
	move(...to: string[]): ManilaFile {
		FS.renameSync(this.#path, Path.join(...to))
		this.#path = Path.join(...to)
		return this
	}

	getPath(): string {
		return this.#path
	}
	getPathRelative(from: string): string {
		return Path.relative(from, this.#path)
	}
	getDir(): string {
		return Path.dirname(this.#path)
	}
	getFileName(): string {
		return Path.basename(this.#path)
	}

	#path: string
}

export class ManilaDirectory {
	constructor(...dir: string[]) {
		const path = Path.join(...dir)
		if (Path.isAbsolute(path)) this.#path = path
		else this.#path = Path.join(process.cwd(), path)

		if (FS.existsSync(this.#path) && FS.lstatSync(this.#path).isFile())
			throw new Error(`Path '${this.#path}' is a file, not a directory!`)
	}

	exists(): boolean {
		return FS.existsSync(this.#path)
	}
	create(recursive: boolean = true): ManilaDirectory {
		FS.mkdirSync(this.#path, { recursive })
		return this
	}
	delete(recursive: boolean = true): ManilaDirectory {
		FS.rmSync(this.#path, { recursive })
		return this
	}
	rename(to: string): ManilaDirectory {
		FS.renameSync(this.#path, Path.join(Path.dirname(this.#path), to))
		this.#path = Path.join(Path.dirname(this.#path), to)
		return this
	}
	move(to: string): ManilaDirectory {
		FS.renameSync(this.#path, Path.join(process.cwd(), to, Path.basename(this.#path)))
		this.#path = Path.join(process.cwd(), to, Path.basename(this.#path))
		return this
	}
	concat(...paths: string[]) {
		this.#path = Path.join(this.#path, ...paths)
		return this
	}
	files(recursive: boolean = false): string[] {
		if (!recursive) return FS.readdirSync(this.#path)

		const out = []
		function rec(root: string, path: string) {
			const files = FS.readdirSync(path)
			for (const file of files) {
				const dir = Path.join(path, file)
				const stat = FS.lstatSync(dir)
				if (stat.isDirectory()) rec(root, dir)
				else if (stat.isFile()) out.push(Path.relative(root, dir))
			}
		}

		rec(this.#path, this.#path)
		return out
	}

	getPath() {
		return this.#path
	}
	getPathRelative(from: string): string {
		return Path.relative(from, this.#path)
	}
	getBasename() {
		return Path.basename(this.#path)
	}

	#path: string
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

	// Utility Functions
	static file(...file: string[]): ManilaFile {
		return new ManilaFile(...file)
	}
	static directory(...dir: string[]): ManilaDirectory {
		return new ManilaDirectory(...dir)
	}

	static #conig: ManilaConfig
	static #workspace: ManilaWorkspace
	static #project: ManilaProject
}
