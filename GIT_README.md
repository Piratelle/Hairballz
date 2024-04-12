# Git Setup
Basic steps to get started with Unity, Git, and GitHub!

## Before You Start
+ Make sure that you have both **Unity** and **Git** installed in your local environment. These instructions are written using *Git CMD*, not the GUI version, but if you are more comfortable with the GUI version the same Git actions will work with that version too.
+ Everyone on the team needs to have the same version of Unity installed before getting started!
+ When you set up your local Git environment, you will need to make sure to set up GitHub authentication in order to have write access to the repositories. More info on that can be found [on GitHub](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git).
+ A good overview of the most common Git commands is also available [on GitHub](https://education.github.com/git-cheat-sheet-education.pdf).

## Starting the Project (Repository Creator)
+ Create your Unity project with the desired Unity version.
+ Open local Git CMD and navigate to the project folder that you just created. Use **git init** to create a new repository.
	++ *(Optional)* GitHub uses "main" as the main branch name, but local Git is usually set up with the more antiquated "master". You can rename your local branch by using **git branch -m main** to smooth the process. To set this for all future local projects, you can use **git config --global init.defaultBranch main**. If you don't change the branch name, make sure to use your actual local branch name instead of *main* in the remaining instructions.
+ On GitHub, create an empty repository with the same name as your project. Make sure to set the **Add .gitignore** setting to **Unity**!
+ Copy your new repository's .git url (found in the **Code** dropdown on the main repository page). Back in Git CMD, use that url as the **[url]** argument in **git remote add origin [url]**.
+ In Git CMD, use **git pull origin main** to pull down the GitHub commit - and most importantly, that .gitignore file! This has to happen *before* you add any local files, or the .gitignore will not work correctly.
+ In Git CMD, use **git add .** to stage all of the un-ignored local files. Then use **git commit -am "Initial local commit."** to create a complete *local* commit.
+ Still in Git CMD, use **git push --set-upstream origin main** to push that local commit back to GitHub and finish linking your local branch to your GitHub branch. If you refresh the GitHub repository page, you should see more folders (particularly **Assets**) and your commit message.

## Cloning the Project (Collaborators)
+ Make sure that you have access to the GitHub repository - the Repository Creator will need to add you as a Collaborator before you can push commits.
+ Open local Git CMD and navigate into your main Unity projects directory. You don't need to make a Unity project or folder.
+ Use **git clone [url]** with the repository url. This will create a the project directory and pull in the most recent commit that has been pushed to the repository. Make sure to navigate into the project directory once it has been created.
## Working Tips
+ Whenever working on the project, open local Git CMD and navigate into the project directory.
+ You can use **git status** to check on both your local file status (are there files that need to be added? are there changes that need to be committed?) and how your local commit compares to the repository.
+ Use **git pull** before you get started to make sure that you have the most up to date files.
+ Commit regularly - that way, you are less likely to lose work!
+ Remember when committing that a **git add** will need to happen whenever a new file is added or created - **git commit -am "Message"** will only commit changes to files that have already been staged.
+ All commits are local first, so make sure to use **git push** to send them to the repository and give all collaborators access to them.
+ Conflicts in code files can be resolved by comparing the text - context in Unity-specific files (like changes to assets) are a lot harder to resolve manually. Sometimes, the best option is to roll back to a previous commit. You can use **git reset** commands with various arguments to roll back changes, but be careful with them, as they can wipe out unsaved changes and connections to downstream commits! There are some good tips [here](https://jwiegley.github.io/git-from-the-bottom-up/3-Reset/4-doing-a-hard-reset.html).