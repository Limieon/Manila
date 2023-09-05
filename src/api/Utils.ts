import Utils from '../Utils.js'

export default class ManilaTimer {
	constructor() {
		this.#start = Date.now()
	}

	/**
	 * Gets the amount of time elapsed since this object was created
	 *
	 * @returns time in ms
	 */
	time(): number {
		return Date.now() - this.#start
	}

	format(): string {
		return Utils.stringifyDuration(Date.now() - this.#start)
	}

	#start: number
}
