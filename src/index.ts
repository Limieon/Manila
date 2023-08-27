import Chalk from 'chalk'
import Inquirer from 'inquirer'
import Gradient from 'gradient-string'
import Figlet from 'figlet'
import Path from 'path'
import Axios from 'axios'
import FS, { utimes } from 'fs'
import Semver from 'semver'

import minimist from 'minimist'

import BuildSystem, { PluginIndexFile } from './BuildSystem.js'
import ScriptHook from './ScriptHook.js'
import Logger from './Logger.js'
import Utils from './Utils.js'
import CLIApp from './CLIApp.js'
import * as Commands from './Commands.js'

const args = minimist(process.argv.slice(2))
let subCommand = args._[0]

const manila = Gradient.vice('Manila')
const app = new CLIApp('manila', 'A BuildSystem for C# using JavaScript', '1.0.0')

app.registerCommand({
	name: 'init',
	description: 'Initializes a new Manila project',
	options: [
		{
			name: 'default',
			alias: 'd',
			description: 'Initializes a default project without prompting'
		}
	],
	callback: Commands.init
})
app.registerCommand({
	name: 'projects',
	description: 'List available projects',
	callback: (args, opts) => Commands.projects(opts)
})
app.registerCommand({
	name: 'tasks',
	description: 'List available tasks',
	callback: (args, opts) => Commands.tasks(opts)
})
app.registerCommand({
	name: 'parameters',
	description: 'List available parameters',
	callback: (args, opts) => Commands.parameters(opts)
})
app.registerCommand({
	name: 'plugins',
	description: 'List available plugins',
	callback: Commands.plugins
})
app.registerCommand({
	name: 'install',
	description: 'Installs a plugin',
	parameters: [
		{
			name: 'plugin',
			description: 'the plugin to install',
			type: 'string'
		}
	],
	options: [
		{
			name: 'force',
			alias: 'f',
			description: 'Forcefully installs a plugin'
		}
	],
	callback: (args, opts) => Commands.install(args['plugin'], opts)
})
app.registerCommand({
	name: 'link',
	description: 'Links a locally installed plugin',
	parameters: [
		{
			name: 'dir',
			description: 'the root directory of your plugin',
			type: 'string'
		},
		{
			name: 'name',
			description: 'the name of your plugin (will be used for importing)',
			type: 'string'
		},
		{
			name: 'indexFile',
			description: 'the name of your index file',
			type: 'string',
			default: 'index.ts'
		}
	],
	callback: Commands.link
})

app.registerCommand({
	name: 'help',
	description: 'Shows information to a specific command',
	parameters: [
		{
			name: 'command',
			description: 'the command to get information for',
			type: 'string',
			optional: true,
			default: null
		}
	],
	callback: (args: object) => {
		app.printHelpText(args['command'])
	}
})

app.registerGlobalOption({
	name: 'help',
	alias: 'h',
	description: 'Prints this help text'
})
app.registerGlobalOption({
	name: 'dir',
	alias: 'd',
	description: 'Changes working directory',
	parmater: {
		name: 'dir',
		description: 'the directory to change into',
		type: 'string',
		default: '.'
	}
})
app.registerGlobalOption({
	name: 'verbose',
	alias: 'v',
	description: 'Enables verbose logging'
})

console.log(Gradient.vice.multiline(Figlet.textSync('Manila', { font: 'Doom' })))
if (subCommand.startsWith(':')) {
	// Subcommand is a task
	if (args['dir']) process.chdir(args['dir'])

	const task = subCommand

	BuildSystem.init()
	await ScriptHook.run()

	if (ScriptHook.hasTask(task)) {
		let start = Date.now()
		try {
			const res = await ScriptHook.runTask(task)
			const duration = Date.now() - start

			if (res) {
				console.log()
				console.log(Chalk.green(`Task Successful! ${Chalk.gray('Took')} ${Chalk.cyan(Utils.stringifyDuration(duration))}`))
				process.exit(0)
			} else {
				console.log()
				console.log(Chalk.red(`Task Failed! ${Chalk.gray('Took')} ${Chalk.cyan(Utils.stringifyDuration(duration))}`))
				process.exit(-1)
			}
		} catch (e) {
			console.log()
			const duration = Date.now() - start
			console.log(Chalk.red(`Task Failed! ${Chalk.gray('Took')} ${Chalk.cyan(Utils.stringifyDuration(duration))}`))
			console.log(e)
			process.exit(-1)
		}
	} else {
		console.log(Chalk.red(`Cannot find task named ${Chalk.blue(task)}!`))
		process.exit(-1)
	}
}

app.parse(process.argv.slice(2))
