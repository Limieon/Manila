const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

const baseBinDir = Manila.dir(workspace.location).join('bin')
const baseObjDir = Manila.dir(workspace.location).join('bin-int')

const binDir = baseBinDir.join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)
const objDir = baseObjDir.join(config.platform).join(`${config.config}-${config.arch}`).join(project.name)

const srcFileSet = Manila.fileSet(project.location)
srcFileSet.include('src/**/*.c').include('src/**/*.cpp')

Manila.task('clean').onExecute(async () => {
	Manila.println('Deleting Bin Dir...')
	if (baseBinDir.exists()) baseBinDir.delete()
	Manila.println('Deleting Obj Dir...')
	if (baseObjDir.exists()) baseObjDir.delete()
})

Manila.task('recompile')
	.tag('manila/rebuild')
	.dependsOn(':client:clean')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.println('Recompiling...')
	})

Manila.task('compile')
	.tag('manila/build')
	.onExecute(async () => {
		Manila.println('Building...')
	})

Manila.task('run')
	.tag('manila/run')
	.dependsOn(':client:compile')
	.onExecute(async () => {
		Manila.println('Running...')
		const res = Manila.http.get('https://www.timeapi.io/api/Time/current/zone?timeZone=Asia/Manila')
		Manila.println(res['dateTime'])

		const res2 = Manila.http.post('http://127.0.0.1:20176/post', {
			data: {
				username: 'Limieon',
				password: '1234'
			}
		})
		Manila.println(res2)

		const res3 = Manila.http.put('http://127.0.0.1:20176/put', {
			data: {
				password: '5678'
			}
		})
		Manila.println(res3)

		const res4 = Manila.http.delete('http://127.0.0.1:20176/delete')
		Manila.println(res4)
	})

Manila.task('test').onExecute(async () => {
	Manila.println('Test Task')
	Manila.runTask(':client:compile')
})
