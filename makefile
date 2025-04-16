SHELL = cmd

LOCAL_INFO = LocalInfo.txt
REPO_PATH := $(strip $(file < $(LOCAL_INFO)))
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

Clean:
	del $(DLL)
	
.IGNORE: Push

.PHONY: Compile Debug Push 