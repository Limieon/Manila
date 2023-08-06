import Chalk from 'chalk'
import Gradient from 'gradient-string'

const LOG_PREFIX = Chalk.gray('>')
export default class Logger {
	static init(quiet: boolean) {
		Logger.#quiet = quiet
	}

	static info(...msg: string[]) {
		if (Logger.#quiet) return
		console.log(`${LOG_PREFIX}`, msg.join(' '))
	}
	static warning(...msg: string[]) {
		console.log(`${LOG_PREFIX}`, Chalk.yellow(msg.join(' ')))
	}
	static error(...msg: string[]) {
		console.log(`${LOG_PREFIX}`, Chalk.red(msg.join(' ')))
	}

	static script(...msg: string[]) {
		console.log(`${LOG_PREFIX}`, msg.join(' '))
	}

	static #quiet
}
