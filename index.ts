import Chalk from 'chalk'
import Inquirer from 'inquirer'
import Gradient from 'gradient-string'
import { Command } from 'commander'
import Figlet from 'figlet'
import Path from 'path'
import Axios from 'axios'
import FS from 'fs'
import Semver, { SemVer } from 'semver'

import { simpleGit } from 'simple-git'
const Git = simpleGit()

import Manila, { PluginIndexFile } from './Manila.js'
import ScriptHook from './ScriptHook.js'
import Logger from './Logger.js'
import Utils from './Utils.js'

const manila = Gradient.vice('Manila')
const app = new Command('manila')
	.description('A BuildSystem for C# using JavaScript')
	.argument('[task]', 'the task to run')
	.version('1.0.0')
	.action((task, opts) => {
		ScriptHook.run(app)

		if (task != undefined) {
			if (ScriptHook.hasTask(task)) {
				console.log(`Running task ${Chalk.blue(task)} and its dependencies...\n`)

				const timeStart = Date.now()
				let success = false
				let errorMsg = ''

				try {
					success = ScriptHook.runTask(task)
				} catch (e) {
					errorMsg = e
					success = false
				}

				const timeElapsed = (Date.now() - timeStart) * 0.001

				if (success) {
					console.log()
					console.log(`${Chalk.green('TASK SUCCESSFUL!')}`)
					console.log(`Took ${(timeElapsed / 60).toFixed(0)}m ${timeElapsed % 60}s`)
					return
				} else {
					console.log()
					console.error(`${Chalk.red('TASK FAILED!')} ${Chalk.gray('-')} ${errorMsg}`)
					console.log(errorMsg)
					console.log()
					console.log(`Took ${(timeElapsed / 60).toFixed(0)}m ${timeElapsed % 60}s`)
					return
				}
			} else {
				Logger.error(`Task ${Chalk.blue(task)} was not found!\n`)
			}
		}

		ScriptHook.prettyPrintTasks()

		ScriptHook.prettyPrintParameters()
	})

app.command('init')
	.description('Intializes a new Manila Project')
	.action(async () => {
		console.log(Chalk.gray(`Intializing a new ${manila} project...`))
		const { name } = await Inquirer.prompt({
			name: 'name',
			type: 'input',
			message: 'Enter the name of your project',
			default: Path.basename(process.cwd())
		})
		const { namespace, modules } = await Inquirer.prompt([
			{
				name: 'namespace',
				type: 'input',
				message: 'Enter a namespace for your project',
				default: name
			},
			{
				name: 'modules',
				type: 'confirm',
				message: 'Will this project contain modules (sub-projects)?',
				default: true
			}
		])
		if (modules) {
			const { moduleName } = await Inquirer.prompt({
				name: 'moduleName',
				type: 'input',
				message: 'Enter the name of your first module',
				default: name
			})

			Manila.createModule({
				name: moduleName,
				namespace: namespace,
				dir: Path.join(process.cwd(), moduleName)
			})
		}

		Manila.createProject({
			name,
			namespace,
			dummy: modules
		})
	})

app.command('projects')
	.description('List all available projects')
	.action(() => {
		ScriptHook.run(app)

		console.log(Chalk.magenta('Available Projects:'))
		ScriptHook.prettyPrintProjects()
	})

app.command('parameters')
	.description('List all available parameters')
	.action(() => {
		ScriptHook.run(app)

		ScriptHook.prettyPrintParameters()
	})

app.command('tasks')
	.description('List all available tasks')
	.action(() => {
		ScriptHook.run(app)

		ScriptHook.prettyPrintTasks()
	})

app.command('plugins')
	.description('List available plugin repositories')
	.action(async (plugin) => {
		const repos = Manila.getSetting_stringList('pluginRepositories', [
			'https://raw.githubusercontent.com/Limieon/Manila-Plugins/main/index.manila.json'
		])

		await new Promise<void>((done, rej) => {
			repos.forEach(async (r, i, a) => {
				Axios.get(r)
					.then((res) => {
						const { data } = res

						console.log(
							`${Chalk.magenta(data.name)}: ${Chalk.yellow(Object.keys(data.plugins).length)} ${Chalk.gray('plugin(s)')}`
						)

						let table = []
						Object.keys(data.plugins).forEach((k) => {
							console.log(`  - ${Chalk.blue(data.plugins[k].name)} - ${Chalk.yellow(data.plugins[k].version)}`)
						})

						if (i == a.length - 1) done()
						else console.log()
					})
					.catch((err) => {
						let e = err.response
						console.log(e)
						throw new Error(`Could not fetch plugin index file (Error: ${e.status} ${e.statusText})\nURL: ${r}`)
					})
			})
		})
	})

app.command('install')
	.description('Install a plugin')
	.argument('<plugin>', 'the plugin to install')
	.option('-f, --force', 'force install a plugin')
	.action(async (plugin, opts) => {
		let force = opts.force == undefined ? false : opts.force
		let pluginsConfig = Manila.getPluginsConfig()

		console.log(Chalk.gray('Parsing plugin indices...'))

		const repos = Manila.getSetting_stringList('pluginRepositories', [
			'https://raw.githubusercontent.com/Limieon/Manila-Plugins/main/index.manila.json'
		])

		let plugins: { [key: string]: { name: string; version: string; index: string; repo: string } } = {}
		await new Promise<void>((done, rej) => {
			repos.forEach(async (r, i, a) => {
				Axios.get(r)
					.then((res) => {
						const index: PluginIndexFile = res.data

						Object.keys(index.plugins).forEach((k) => {
							plugins[k] = index.plugins[k]
							plugins[k].repo = index.name
						})

						if (i == a.length - 1) done()
					})
					.catch((err) => {
						let e = err.response
						console.log(e)
						throw new Error(`Could not fetch plugin index file (Error: ${e.status} ${e.statusText})\nURL: ${r}`)
					})
			})
		})

		let ts = Date.now()

		let p = plugins[plugin]
		if (p == undefined) throw new Error(`Could not find plugin ${plugin}!`)

		// Check if newer or same version in installed
		let installedVersion = pluginsConfig[plugin] != undefined ? pluginsConfig[plugin].version : undefined
		if (installedVersion != undefined && !force) {
			if (!Semver.gt(p.version, installedVersion)) {
				console.log(Chalk.yellow(`Plugin ${Chalk.blue(plugin)} is already installed!`))
				process.exit(0)
			}
		}

		console.log(Chalk.gray('Installing'), Chalk.blue(p.name), Chalk.gray('from'), `${Chalk.magenta(p.repo)}${Chalk.gray('...')}`)

		if (!FS.existsSync('./.manila/tmp')) FS.mkdirSync('./.manila/tmp', { recursive: true })

		console.log(Chalk.gray('Cloning git repo...'))
		await Git.clone('https://github.com/Limieon/Manila-Plugins.git', `./.manila/tmp/${ts}`)

		if (FS.existsSync(`./.manila/plugins/${plugin}`)) {
			console.log(Chalk.gray('Removing old installation...'))
			FS.rmSync(`./.manila/plugins/${plugin}`, { recursive: true })
		}
		FS.cpSync(`./.manila/tmp/${ts}/${plugin}`, `./.manila/plugins/${plugin}`, { recursive: true })

		pluginsConfig[plugin] = {
			indexFile: p.index,
			version: p.version,
			location: `/.manila/plugins/${plugin}`
		}

		console.log(Chalk.green('Successfully installed'), Chalk.blue(plugin))

		Manila.updatePluginsConfig(pluginsConfig)
	})

app.command('link')
	.description('Links a locoally installed plugin')
	.argument('<dir>', 'the directory to your plugin')
	.argument('<name>', 'the name of your plugin')
	.argument('[indexFile]', 'the name of your index file', 'index.ts')
	.action((dir, name, indexFile) => {
		let plugins = Manila.getPluginsConfig()

		if (!FS.existsSync(dir)) {
			console.log(Chalk.red(`Specified plugin directory does not exist!`))
			process.exit(-1)
		}
		if (plugins[name] != undefined) {
			console.log(Chalk.red(`Plugin ${name} already exists in your plugins list!`))
			process.exit(-1)
		}

		plugins[name] = {
			indexFile: indexFile,
			location: Path.relative(process.cwd(), dir),
			version: '1.0.0'
		}

		Manila.updatePluginsConfig(plugins)

		console.log(Chalk.green('Successfully linked plugin!'))
	})

console.log(Gradient.vice.multiline(Figlet.textSync('Manila', 'Doom')))
if (process.argv.indexOf(`--no-logo`) < 0) Manila.init()

app.parse()
