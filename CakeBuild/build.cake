// load up common tools
#load tools\SubPointSolutions.CakeBuildTools\Scripts\SubPointSolutions.CakeBuild.Core.cake

defaultActionGitHubReleaseNotes.Task.Actions.Clear();

// default targets
RunTarget(target);