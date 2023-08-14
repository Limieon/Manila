import Manila from './Manila.js'
import ScriptHook from './ScriptHook.js'

export function init() {}
export function install() {}
export async function projects(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	Manila.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintProjects()
}
export async function parameters(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	Manila.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintParameters()
}
export async function tasks(opts: object) {
	if (opts['dir']) process.chdir(opts['dir'])

	Manila.init()
	await ScriptHook.run()

	ScriptHook.prettyPrintTasks()
}
export function link() {}
export function plugins() {}
export function help() {}
