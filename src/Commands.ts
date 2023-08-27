import Chalk from 'chalk'
import BuildSystem, { PluginConfig, PluginIndexFile } from './BuildSystem.js'
import ScriptHook from './ScriptHook.js'
import Axios from 'axios'
import GitUtils from './GitUtils.js'
import Path from 'path'
import FS from 'fs'
import FileUtils from './FileUtils.js'
import SemVer from 'semver'
import Gradient from 'gradient-string'
import * as Prompts from '@clack/prompts'

export async function init(opts: object) {
	Prompts.intro(Gradient.vice('Init Manila App'))

	const { name, dir, namespace, modules } = await Prompts.group({
		name: () =>
			Prompts.text({
				message: 'Enter the name of your project',
				placeholder: Path.basename(process.cwd())
			}),
		dir: () =>
			Prompts.text({
				message: 'Enther the root of your project',
				validate: val => {
					if (FS.existsSync(Path.join(process.cwd(), val))) return 'Path does already exist'
				}
			}),
		namespace: () =>
			Prompts.text({
				message: 'Enter the prefix of your namespace'
			}),
		modules: () =>
			Prompts.confirm({
				message: 'Do you want to create a module (sub-project)?'
			})
	})

	if (!FS.existsSync(dir)) FS.mkdirSync(dir)
	process.chdir(dir)

	if (modules) {
		const { moduleName } = await Prompts.group({
			moduleName: () =>
				Prompts.text({
					message: 'Etner the name of your module'
				})
		})

		BuildSystem.createModule({
			name: moduleName,
			namespace: `${namespace}`,
			dir: Path.join(moduleName)
		})
	}

	BuildSystem.createProject({
		name,
		namespace,
		dummy: modules
	})
}

export async function install(pluginName: string, opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	BuildSystem.init()

	const indices: PluginIndexFile[] = []

	const pluginRepos = BuildSystem.getSetting_stringList('pluginRepositories', [
		'https://raw.githubusercontent.com/Limieon/Manila-Plugins/main/index.manila.json'
	])

	const installedPluginFileDir = FileUtils.getPluginFileFromRootDir()
	if (!installedPluginFileDir) FS.writeFileSync('./.manila/plugins.manila', '{}')
	const iPlugins: PluginConfig = JSON.parse(FS.readFileSync(FileUtils.getPluginFileFromRootDir(), { encoding: 'utf-8' }))

	const pluginFileName = FileUtils.getPluginFileFromRootDir()

	if (pluginRepos.length < 1) {
		console.log(Chalk.red('There are no plugin repositories defined!'))
		process.exit(-1)
	}

	for (const repo of pluginRepos) {
		indices.push((await Axios.get(repo)).data)
	}

	for (const index of indices) {
		for (const pluginKey of Object.keys(index.plugins)) {
			if (pluginKey.toLowerCase() != pluginName.toLowerCase()) continue

			const { gitRepo, name } = index
			const plugin = index.plugins[pluginKey]

			if (iPlugins[pluginKey] != undefined) {
				if (SemVer.gte(iPlugins[pluginKey].version, plugin.version) && !opts['force']) {
					console.log(
						Chalk.red(
							`Plugin ${Chalk.blue(plugin.name)} ${Chalk.blue(plugin.version)} already installed as ${Chalk.blue(
								plugin.name
							)} ${Chalk.blue(iPlugins[pluginKey].version)}`
						)
					)
					process.exit(-1)
				}
			}

			const repoPath = Path.join(process.cwd(), '.manila', 'temp', `${Date.now()}`)
			if (!FS.existsSync(repoPath)) FS.mkdirSync(repoPath, { recursive: true })

			const pluginPath = Path.join('.manila', 'plugins', pluginKey)
			if (!FS.existsSync(pluginPath)) FS.mkdirSync(pluginPath, { recursive: true })

			const pluginInRepoPath = Path.join(repoPath, pluginKey)

			console.log(Chalk.gray(`Installing ${Chalk.blue(plugin.name)} from ${Chalk.magenta(name)}...`))
			console.log(Chalk.gray('Cloning Git Repo...'))
			await GitUtils.clone(gitRepo, repoPath)

			console.log(Chalk.gray('Extracting Plugin...'))
			FS.cpSync(pluginInRepoPath, pluginPath, { recursive: true })

			console.log(Chalk.gray('Deleting Temp Files...'))
			FS.rmSync(repoPath, { recursive: true })

			console.log(Chalk.green(`Successfully installed ${Chalk.blue(plugin.name)}`))

			iPlugins[pluginKey] = {
				indexFile: plugin.index,
				location: pluginPath,
				version: plugin.version
			}

			FS.writeFileSync(pluginFileName, JSON.stringify(iPlugins, null, 4))
			process.exit(0)
		}
	}
	console.log(Chalk.red(`Could not find plugin ${Chalk.blue(pluginName)}!`))
	process.exit(-1)
}

export async function projects(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	BuildSystem.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintProjects()
}

export async function parameters(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	BuildSystem.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintParameters()
}

export async function tasks(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	BuildSystem.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintTasks()
}

export function link() {}

export function plugins() {}
