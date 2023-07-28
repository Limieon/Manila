import Chalk from 'chalk'
import Gradient from 'gradient-string'

const LOG_PREFIX = Chalk.gray('>')
export default class Logger {
	static init(quiet) {
		Logger.#quiet = quiet
	}

	static info(...msg) {
		if (Logger.#quiet) return
		console.log(`${LOG_PREFIX}`, msg.join(' '))
	}
	static warning(...msg) {
		console.log(`${LOG_PREFIX}`, Chalk.yellow(msg.join(' ')))
	}
	static error(...msg) {
		console.log(`${LOG_PREFIX}`, Chalk.red(msg.join(' ')))
	}

	static script(file, ...msg) {
		console.log(`${Chalk.blue(file)} ${LOG_PREFIX}`, msg.join(' '))
	}

	static #quiet
}
