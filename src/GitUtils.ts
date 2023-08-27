import { simpleGit } from 'simple-git'
const Git = simpleGit()

export default class GitUtils {
	static async clone(url: string, into: string) {
		await Git.clone(url, into)
	}
}
