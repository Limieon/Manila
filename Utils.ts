type TableData = string[][]

export default class Utils {
	static createTable(data: TableData, minMargin: number = 0): string {
		let out = ''

		let maxWidths = []
		data.forEach((a, j) => {
			a.forEach((s, i) => {
				if (maxWidths[i] == undefined) maxWidths[i] = 0
				maxWidths[i] = Math.max(maxWidths[i], s.replaceAll(/\x1B\[[0-9;]*m/g, '').length)
			})
		})

		for (let i = 0; i < maxWidths.length; ++i) maxWidths[i] = maxWidths[i] + minMargin

		data.forEach((a) => {
			a.forEach((s, i) => {
				out += s
				out += ' '.repeat(maxWidths[i] - s.replaceAll(/\x1B\[[0-9;]*m/g, '').length)
			})
			out += '\n'
		})

		return out
	}

	static getRepoKey(url: string) {
		let res = /\/\/(.*?).git/.exec(url)[1].split('/')
		return `${res[0]}/${res[1]}/${res[2]}`
	}
}
