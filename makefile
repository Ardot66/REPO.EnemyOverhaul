SHELL = cmd

LOCAL = Local
REPO_PATH := $(file < $(LOCAL)/REPOPath.txt)
PUBLISH_TOKEN := $(file < $(LOCAL)/PublishToken.txt)
NAME = EnemyOverhaul
DLL = bin\Debug\netstandard2.1\$(NAME).dll

All: Compile Push Debug

Compile: $(DLL) 

$(DLL): Source/*.cs Source/*/*.cs $(NAME).csproj
	dotnet build
	copy $(DLL) "$(REPO_PATH)\BepInEx\plugins\$(NAME).dll" /B
	copy $(DLL) ..\Libraries\$(NAME).dll

Debug: 
	$(REPO_PATH)\REPO.exe $(ARGS)

Push:
	cd /D "$(REPO_PATH)\BepInEx\plugins" &&\
		git add . && \
		git commit -m "Recompiled plugins" &&\
		git push origin main

Publish:
	tcli build
	tcli publish --token $(PUBLISH_TOKEN)

Clean:
	del $(DLL)
	
.IGNORE: Push

.PHONY: Compile Debug Push 