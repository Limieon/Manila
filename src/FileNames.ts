import FS from 'fs'
import Path from 'path'

const PLUGIN_INDEX_FILE_NAMES = ['index.manila', 'index.manila.json']

const SETTINGS_FILE_NAMES = ['settings.manila', 'settings.manila.yml', 'settings.manila.yaml']

export default class FileNames {
	static pluginIndexFile(dir?: string) {
		if (dir == undefined) dir = './'
		for (const f of PLUGIN_INDEX_FILE_NAMES) if (FS.existsSync(Path.join(dir, f))) return f
	}
	static settingsFile(dir?: string) {
		if (dir == undefined) dir = './'
		for (const f of SETTINGS_FILE_NAMES) if (FS.existsSync(Path.join(dir, f))) return f
	}
}
