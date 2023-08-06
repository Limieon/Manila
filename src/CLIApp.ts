import Chalk from 'chalk'
import Utils from './Utils.js'

export type ParameterType = 'string' | 'boolean' | 'number'
export type OptionType = 'string' | 'flag' | 'number'

export type Parameter = {
	name: string
	description: string
	type: ParameterType
	default?: any
	optional?: boolean
}
export type Option = {
	name: string
	description: string
	alias?: string
	type: OptionType
	default?: any
}

export type Command = {
	name: string
	description: string
	parameters?: Parameter[]
	options?: Option[]
	callback: () => void
}

export default class CLIApp {
	constructor(name: string, description: string, version: string) {
		this.name = name
		this.description = description
		this.version = version

		this.#globalOptions = {}
		this.#commands = {}
	}

	registerGlobalOption(option: Option) {
		if (this.#globalOptions[option.name] != undefined) throw new Error(`Command ${option.name} has already been defined!`)
		this.#globalOptions[option.name] = option

		if (option.alias == undefined) return
		if (option.alias.length < 1 || option.alias.length > 1) throw new Error('Aliases must be one char long!')

		let aliases: string[] = []
		Object.keys(this.#globalOptions).forEach((k) => {
			if (aliases.includes(this.#globalOptions[k].alias))
				throw new Error(`Alias ${this.#globalOptions[k].alias} has already been defined for command ${k}!`)
		})
	}
	registerCommand(args: Command) {
		if (args.options == undefined) args.options = []
		if (args.parameters == undefined) args.parameters = []

		if (this.#commands[args.name] != undefined) throw new Error(`Command ${args.name} has already been defined!`)
		this.#commands[args.name] = args
	}

	printHelpText(command?: string) {
		if (command == undefined) {
			console.log(
				Chalk.magenta('Usage:'),
				Chalk.blue(this.name),
				`${Chalk.gray('[')}${Chalk.cyan('command')}${Chalk.gray(']')}`,
				`${Chalk.gray('[')}${Chalk.cyan('options')}${Chalk.gray(']')}`
			)
			console.log()
			console.log(this.description)
			console.log()

			console.log(Chalk.magenta('Available Commands:'))
			let table = []
			Object.keys(this.#commands).forEach((k) => {
				let c = this.#commands[k]
				let names: string[] = []
				c.parameters.forEach((p) => {
					names.push(this.stringifyParameter(p))
				})

				table.push([`  ${Chalk.blue(c.name)} ${names.join(' ')}`, c.description])
			})

			table.push([])
			table.push([Chalk.magenta('Globally available options:')])
			Object.keys(this.#globalOptions).forEach((k) => {
				let o = this.#globalOptions[k]
				if (o.alias != undefined)
					table.push([`  ${Chalk.gray('-')}${Chalk.cyan(o.alias)}, ${Chalk.gray('--')}${Chalk.cyan(o.name)}`, o.description])
				else table.push([`  ${Chalk.gray('--')}${Chalk.cyan(o.name)}`, o.description])
			})
			console.log(Utils.createTable(table, 1))
			return
		}

		const cmd = this.#commands[command]
		if (cmd == undefined) {
			console.log(Chalk.red(`Command ${command} could not be found!`))
			return
		}

		let params: string[] = []
		cmd.parameters.forEach((p) => {
			params.push(this.stringifyParameter(p))
		})

		console.log(Chalk.gray(cmd.description))
		console.log(Chalk.magenta('Usage:'), Chalk.blue(this.name), Chalk.yellow(cmd.name), params.join(' '))
		console.log()
		console.log(Chalk.magenta('Parameters:'))
		const table = []
		cmd.parameters.forEach((p) => {
			if (p.default)
				table.push([`  ${Chalk.cyan(p.name)}`, `${p.description} ${Chalk.gray(`(Default: ${Chalk.yellow(p.default)})`)}`])
			else table.push([`  ${Chalk.cyan(p.name)}`, p.description])
		})
		console.log(Utils.createTable(table, 1))
	}

	stringifyParameter(p: Parameter) {
		if (p.optional && p.default == undefined) return `${Chalk.gray('[')}${Chalk.cyan(p.name)}${Chalk.gray(']')}`
		else return `${Chalk.gray('<')}${Chalk.cyan(p.name)}${Chalk.gray('>')}`
	}

	parse(args: string[]) {}

	#globalOptions: { [key: string]: Option }
	#commands: { [key: string]: Command }

	name: string
	description: string
	version: string
}
