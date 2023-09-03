import Path from 'path'
import FS from 'fs'

export class ManilaFile {
	constructor(...f: string[]) {
		const path = Path.join(...f)
		if (Path.isAbsolute(path)) this.#path = path
		else this.#path = Path.join(process.cwd(), path)

		if (FS.existsSync(this.#path) && FS.lstatSync(this.#path).isDirectory())
			throw new Error(`Path '${this.#path}' is a directory, not a file!`)
	}

	/**
	 * Checks if the file exists
	 */
	exists(): boolean {
		return FS.existsSync(this.#path)
	}
	/**
	 * Write data into a file
	 * @param data the data to write
	 * @param encoding the encoding the file will be saved in
	 */
	write(data: string | Buffer, encoding: BufferEncoding | null | undefined = 'utf-8'): ManilaFile {
		FS.writeFileSync(this.#path, data, { encoding })
		return this
	}
	/**
	 * Read data from a file
	 * @param encoding the encoding of the file
	 */
	read(encoding: BufferEncoding | null | undefined = 'utf-8'): string | Buffer {
		return FS.readFileSync(this.#path, { encoding: encoding })
	}

	/**
	 * Deletes the file (not this handle)
	 */
	delete(): ManilaFile {
		FS.rmSync(this.#path)
		return this
	}
	/**
	 * Copies the file into another location
	 * @param to the destination directory
	 */
	copy(...to: string[]): ManilaFile {
		FS.copyFileSync(this.#path, Path.join(...to))
		return this
	}
	/**
	 * Moves or renames the file
	 * @param to the destination
	 */
	move(...to: string[]): ManilaFile {
		FS.renameSync(this.#path, Path.join(...to))
		this.#path = Path.join(...to)
		return this
	}

	/**
	 * Gets the full path of the file
	 */
	getPath(): string {
		return this.#path
	}
	/**
	 * Gets the relative path to this file from a given origin
	 * @param from origin of your path
	 */
	getPathRelative(from: string): string {
		return Path.relative(from, this.#path)
	}
	/**
	 * Gets the directory this file is placed in
	 */
	getDir(): string {
		return Path.dirname(this.#path)
	}
	/**
	 * Gets the name of the file
	 */
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

	/**
	 * Checks if the directory exists
	 */
	exists(): boolean {
		return FS.existsSync(this.#path)
	}
	/**
	 * Creates the directory
	 * @param recursive create sub directories
	 */
	create(recursive: boolean = true): ManilaDirectory {
		FS.mkdirSync(this.#path, { recursive })
		return this
	}
	/**
	 * Deletes the directory
	 * @param recursive delete sub directories
	 */
	delete(recursive: boolean = true): ManilaDirectory {
		FS.rmSync(this.#path, { recursive })
		return this
	}
	/**
	 * Renames the directory basename
	 * @param to the new name
	 */
	rename(to: string): ManilaDirectory {
		FS.renameSync(this.#path, Path.join(Path.dirname(this.#path), to))
		this.#path = Path.join(Path.dirname(this.#path), to)
		return this
	}
	/**
	 * Moves the directory into another location
	 * @param to the destination
	 */
	move(to: string): ManilaDirectory {
		FS.renameSync(this.#path, Path.join(process.cwd(), to, Path.basename(this.#path)))
		this.#path = Path.join(process.cwd(), to, Path.basename(this.#path))
		return this
	}
	/**
	 * Concats a path to the current directory
	 * @param paths the paths to conact
	 */
	concat(...paths: string[]) {
		this.#path = Path.join(this.#path, ...paths)
		return this
	}
	/**
	 * Gets the files contained in the directory
	 * @param recursive also searches in sub-directories
	 */
	files(recursive: boolean = false, predicate: (f: string) => boolean): ManilaFile[] {
		const out: ManilaFile[] = []
		if (!recursive) {
			for (const f of FS.readdirSync(this.#path)) {
				out.push(new ManilaFile(this.#path, f))
			}
			return out
		}

		function rec(root: string, path: string) {
			const files = FS.readdirSync(path)
			for (const file of files) {
				const dir = Path.join(path, file)
				const stat = FS.lstatSync(dir)
				if (stat.isDirectory()) rec(root, dir)
				else if (stat.isFile()) {
					if (predicate != undefined) {
						if (predicate(Path.relative(root, dir))) out.push(new ManilaFile(dir))
					} else {
						out.push(new ManilaFile(dir))
					}
				}
			}
		}

		rec(this.#path, this.#path)
		return out
	}

	/**
	 * Returns the full path of the directory
	 */
	getPath() {
		return this.#path
	}
	/**
	 * Gets the relative path to this directory from a given origin
	 * @param from origin of your path
	 */
	getPathRelative(from: string): string {
		return Path.relative(from, this.#path)
	}
	/**
	 * Gets the last directory name (basename)
	 */
	getBasename() {
		return Path.basename(this.#path)
	}

	#path: string
}
