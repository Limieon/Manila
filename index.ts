import Chalk from 'chalk'
import Inquirer from 'inquirer'
import Gradient from 'gradient-string'
import { Command } from 'commander'
import Figlet from 'figlet'
import Path from 'path'

import Manila from './Manila.js'
import ScriptHook from './ScriptHook.js'
import Logger from './Logger.js'

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
					console.error(`${Chalk.red('TASK FAILED!')}${Chalk.gray(':')} ${errorMsg}`)
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

		console.log(Chalk.magenta('Available Parameters:'))
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

console.log(Gradient.vice.multiline(Figlet.textSync('Manila', 'Doom')))

app.parse()
