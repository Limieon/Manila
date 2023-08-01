type TableData = string[][]

export default class Utils {
	static createTable(data: TableData): string {
		let out = ''

		let maxWidths = []
		data.forEach((a, j) => {
			a.forEach((s, i) => {
				if (maxWidths[i] == undefined) maxWidths[i] = 0
				maxWidths[i] = Math.max(maxWidths[i], s.replaceAll(/\x1B\[[0-9;]*m/g, '').length)
			})
		})

		data.forEach((a) => {
			a.forEach((s, i) => {
				out += s
				out += ' '.repeat(maxWidths[i] - s.replaceAll(/\x1B\[[0-9;]*m/g, '').length + 1)
			})
			out += '\n'
		})

		return out
	}
}
