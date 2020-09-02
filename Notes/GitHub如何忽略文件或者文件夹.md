##### GitHub如何忽略文件或者文件夹
1. 在根目录下创建.gitignore文件
2. GitHub Desktop中，Repository -> Repository Setting -> Ignored files
```
Library/*   // 忽略文件夹
log.txt     // 忽略文件
*.csproj	// 忽略某类文件

// unity项目的.gitignore
Library/*
Logs/*
Packages/*
Temp/*
obj/*
Cache/*
.vs/*
*.csproj
*.sln
ProjectSettings/ProjectVersion.txt
SimpleGameFrame.v12.suo
```