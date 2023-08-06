import Chalk from 'chalk'
import Utils from './Utils.js'
import minimist from 'minimist'

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
	parmaters?: Parameter[]
}

export type Command = {
	name: string
	description: string
	parameters?: Parameter[]
	options?: Option[]
	callback: (args: object, opts: object) => void
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

		let aliases: string[] = []
		args.options.forEach((o) => {
			if (this.#globalOptions[o.name] != undefined) throw new Error(`Option ${o.name} already existing in global options!`)
			if (aliases.includes(o.alias)) throw new Error(`Alias of option ${o.name} uses an already defined alias!`)
			aliases.push(o.alias)

			Object.keys(this.#globalOptions).forEach((k) => {
				if (o.alias == this.#globalOptions[k].alias) throw new Error(`Alias for option ${o.name} already exists on option ${k}!`)
			})
		})

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
				if (o.alias != undefined) table.push([this.stringifyOption(o), o.description])
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
		console.log(
			Chalk.magenta('Usage:'),
			Chalk.blue(this.name),
			Chalk.yellow(cmd.name),
			`${params.join(' ')}${Chalk.gray('[')}${Chalk.cyan('options')}${Chalk.gray(']')}`
		)
		console.log()

		const table = []
		if (cmd.parameters.length > 0) {
			console.log(Chalk.magenta('Parameters:'))
			cmd.parameters.forEach((p) => {
				if (p.default)
					table.push([`  ${Chalk.cyan(p.name)}`, `${p.description} ${Chalk.gray(`(Default: ${Chalk.yellow(p.default)})`)}`])
				else table.push([`  ${Chalk.cyan(p.name)}`, p.description])
			})
			table.push([])
		}
		table.push([Chalk.magenta('Options:')])
		Object.keys(this.#globalOptions).forEach((k) => {
			table.push([this.stringifyOption(this.#globalOptions[k]), this.#globalOptions[k].description])
		})
		cmd.options.forEach((o) => {
			table.push([this.stringifyOption(o), o.description])
		})

		console.log(Utils.createTable(table, 1))
	}

	stringifyParameter(p: Parameter) {
		if (p.optional && p.default == undefined) return `${Chalk.gray('[')}${Chalk.cyan(p.name)}${Chalk.gray(']')}`
		else return `${Chalk.gray('<')}${Chalk.cyan(p.name)}${Chalk.gray('>')}`
	}
	stringifyOption(o: Option) {
		let parameters: string[] = []

		if (o.parmaters != undefined && o.parmaters.length > 0) o.parmaters.forEach((p) => parameters.push(this.stringifyParameter(p)))

		if (o.alias != undefined)
			return `  ${Chalk.gray('-')}${Chalk.yellow(o.alias)}, ${Chalk.gray('--')}${Chalk.yellow(o.name)} ${parameters.join(' ')}`
		return `  ${Chalk.gray('--')}${Chalk.yellow(o.name)} ${parameters.join(' ')}`
	}

	parse(args: string[]) {
		const argv = minimist(args)
		let subcmd = argv._[0]
		if (subcmd == undefined) this.printHelpText()
		else this.runCommand(subcmd, args)
	}
	runCommand(command: string, args: string[]) {
		let cmd = this.#commands[command]
		const pargs = minimist(args)

		if (cmd == undefined) {
			console.log(Chalk.red('Sub Command named'), Chalk.blue(command), Chalk.red('could not be found!'))
			return
		}

		let argsObject = {}
		for (let i = 0; i < cmd.parameters.length; ++i) {
			let p = cmd.parameters[i]
			if (args[i + 1] == undefined) {
				if (p.default != undefined) {
					argsObject[p.name] = p.default
				} else if (p.optional) {
					argsObject[p.name] = undefined
				} else {
					console.log(Chalk.red('Missing required argument'), Chalk.blue(p.name))
					return
				}
			} else {
				argsObject[p.name] = args[i + 1]
			}
		}

		console.log(pargs)
		cmd.callback(argsObject, pargs)
	}

	#globalOptions: { [key: string]: Option }
	#commands: { [key: string]: Command }

	name: string
	description: string
	version: string
}
