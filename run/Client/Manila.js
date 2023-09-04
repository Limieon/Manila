//const ManilaCS = importPlugin('manila.cs')
//const ManilaCPP = importPlugin('manila.cpp')
const ManilaDC = importPlugin('maniladc')

const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getConfig()

async function sleep(ms) {return new Promise((res, rej) => {setTimeout(res, ms)})}

task('print')
	.executes(() => {
		print('--- Config ---')
		print('Config:', config.config)
		print('Platform:', config.platform)
		print('Architecture:', config.arch)
		print()
		print('--- Workspace ---')
		print('Name:', workspace.name)
		print('Location:', workspace.location.getPath())
		print()
		print('--- Project ---')
		print('Name:', project.name)
		print('Location:', project.location.getPath())
		print('Namespace:', project.namespace)
		print('Author:', project.author)
	})

task('compile')
	.executes(async () => {
		const wh = ManilaDC.webhook(Manila.getSecrets().discord.webhook)
		await wh.send('Compiling...')
		await sleep(2000)
		await wh.send('Compiled Successfully!')
	})
