import FS from 'fs'
import Path from 'path'

const PLUGIN_INDEX_FILE_NAMES = ['index.manila', 'index.manila.json']

const SETTINGS_FILE_NAMES = ['settings.manila', 'settings.manila.yml', 'settings.manila.yaml']

const PLUGIN_FILE_NAMES = ['plugins.manila.json', 'plugins.manila']

export default class FileUtils {
	static pluginIndexFile(dir: string = './') {
		for (const f of PLUGIN_INDEX_FILE_NAMES) if (FS.existsSync(Path.join(dir, f))) return f
	}
	static settingsFile(dir: string = './') {
		for (const f of SETTINGS_FILE_NAMES) if (FS.existsSync(Path.join(dir, f))) return f
	}
	static pluginFile(dir: string = './') {
		for (const f of PLUGIN_FILE_NAMES) if (FS.existsSync(Path.join(dir, f))) return f
	}

	static getPluginIndexFileFromRoot(dir: string = './') {
		return this.pluginIndexFile(dir)
	}
	static getSettingsFileFromRootDir(dir: string = './') {
		const file = this.settingsFile(Path.join(dir, '.manila'))
		if (!file) return undefined
		return Path.join('.manila', file)
	}
	static getPluginFileFromRootDir(dir: string = './') {
		const file = this.pluginFile(Path.join(dir, '.manila'))
		if (!file) return undefined
		return Path.join('.manila', file)
	}
}
