import Chalk from 'chalk'
import Inquirer from 'inquirer'
import Gradient from 'gradient-string'
import { Command } from 'commander'
import Figlet from 'figlet'
import ScriptHook from './ScriptHook.js'

const app = new Command('manila')
	.description('A BuildSystem for C# using JavaScript')
	.version('1.0.0')
	.action((opts) => {
		ScriptHook.prettyPrintTasks()

		console.log(Chalk.magenta('Available Parameters:'))
		ScriptHook.prettyPrintParameters()
	})

console.log(Gradient.vice.multiline(Figlet.textSync('Manila', 'Doom')))
ScriptHook.run(app)

app.parse()
