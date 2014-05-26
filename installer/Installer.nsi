;NSIS Script for installation of the FlowExchange unit operation suite
;Written by Jasper van Baten, AmsterCHEM

; definitions

!define instKey               "Software\Microsoft\Windows\CurrentVersion\Uninstall\FlowExchange"
!define progKey               "Software\FlowExchange"
!define version 	      "1.0.6"

; variables

Var /GLOBAL startMenuFolder
VAR /GLOBAL typeLibFolder
VAR /GLOBAL arg1
Var /GLOBAL SOURCEINSTALLED
Var /GLOBAL IGNOREMODULES

!define LANG_ENGLISH "1033-English"

;--------------------------------

;General

  ;Name 
  Name "FlowExchange"
  OutFile "FlowExchange.${version}.exe"
  SetCompressor LZMA

  ;version info
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "FlowExchange"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments" "FlowExchange CAPE-OPEN Unit Operations"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "http://www.amsterchem.com/"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "(c) 2013 http://www.amsterchem.com/"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "FlowExchange Unit Operation Installer"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${version}"
  VIProductVersion "${version}.0"

  ;Default installation folder (to be overridden)
  ;InstallDir "$PROGRAMFILES64\FlowExchange"

  ;icon
  Icon "..\Icon\flowexchange.ico"
  UninstallIcon "..\Icon\flowexchange.ico"
  UninstallText "FlowExchange (Remove only)"

  ;branding
  BrandingText "FlowExchange ${version}"

;--------------------------------

; Multi-user stuff

!define MULTIUSER_EXECUTIONLEVEL Highest
!define MULTIUSER_MUI
!define MULTIUSER_INSTALLMODE_COMMANDLINE
!define MULTIUSER_INSTALLMODE_FUNCTION SelectInstallMode
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_KEY "${progKey}"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_VALUENAME ""
!include MultiUser.nsh

;--------------------------------
;Includes

  !include "MUI2.nsh"
  !include "x64.nsh"

;--------------------------------
;Macros

Function IsDotNETInstalled
 Push $0
 Push $1
 StrCpy $0 1
 System::Call "mscoree::GetCORVersion(w, i ${NSIS_MAX_STRLEN}, *i) i .r1"
 StrCmp $1 0 +2
 StrCpy $0 0
 Pop $1
 Exch $0
FunctionEnd

Function SelectInstallMode
StrCmp "$MultiUser.InstallMode" "AllUsers" AllUsers 0
;current user
${if} ${AtLeastWin2000}
  StrCpy $INSTDIR "$LOCALAPPDATA\FlowExchange"
  StrCpy $typeLibFolder "$LOCALAPPDATA"
${else}
 StrCpy $INSTDIR "$PROGRAMFILES\FlowExchange"
 ${If} ${RunningX64}
  StrCpy $typeLibFolder "$COMMONFILES64"
 ${Else}
  StrCpy $typeLibFolder "$COMMONFILES"
 ${Endif}
${endif}
ReadRegStr $5 HKCU "${progKey}" ""
${if} $5 != ""
  StrCpy $INSTDIR "$5"
${endif}
Goto done
AllUsers:
${If} ${RunningX64}
 StrCpy $INSTDIR "$PROGRAMFILES64\FlowExchange"
 StrCpy $typeLibFolder "$COMMONFILES64"
${Else}
 StrCpy $INSTDIR "$PROGRAMFILES\FlowExchange"
 StrCpy $typeLibFolder "$COMMONFILES"
${Endif}
ReadRegStr $5 HKLM "${progKey}" ""
${if} $5 != ""
  StrCpy $INSTDIR "$5"
${endif}
done:
FunctionEnd

!macro CheckModule Checker Prefix Module
StrCmp $IGNOREMODULES "1" done${Module}${Prefix}
retry${Module}${Prefix}:
ClearErrors
ExecWait '"${Checker}" ${Module}'
IfErrors 0 +9
MessageBox MB_ICONINFORMATION|MB_ABORTRETRYIGNORE "Module ${Module} is loaded." IDABORT abort${Module}${Prefix} IDRETRY retry${Module}${Prefix}
ignore${Module}${Prefix}:
StrCpy $IGNOREMODULES "1" 
Goto done${Module}${Prefix}
abort${Module}${Prefix}:
MessageBox MB_ICONINFORMATION "FlowExchange Installation aborted. Please try again later" 
Abort
Goto done${Module}${Prefix}
;errors
MessageBox MB_ICONINFORMATION|MB_ABORTRETRYIGNORE "Failed to check whether ${Module} is loaded." IDABORT abort${Module}${Prefix} IDRETRY retry${Module}${Prefix}
Goto ignore${Module}${Prefix}
done${Module}${Prefix}:
!macroend

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING
  !define MUI_ICON "..\Icon\flowexchange.ico"
  !define MUI_UNICON "..\Icon\flowexchange.ico"
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "branding.bmp" 


;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "..\License\License.rtf"
  !insertmacro MULTIUSER_PAGE_INSTALLMODE
  !insertmacro MUI_PAGE_COMPONENTS

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "SHELL_CONTEXT"
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "${progKey}"
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $startMenuFolder

  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "FlowExchange" secFlowExchange
  SetOverwrite on
  StrCpy $SOURCEINSTALLED "false"


  ;main files
  SetOutPath "$INSTDIR" 
  File "..\license\license.pdf"
  File "..\bin\Release\FlowExchange.chm"
  File "..\bin\Release\FlowExchange.dll"
  File "..\bin\Release\CAPEOPEN110.dll"
  File "..\bin\Release\properties.conf"
  ${If} ${RunningX64}
   File "..\bin\Release\RegisterFlowExchange_x64.exe"
  ${EndIf}
  File "..\bin\Release\RegisterFlowExchange_x86.exe"
  File "..\bin\Release\XFlowViewer.exe"

  ;CAPE-OPEN type libaries
  SetOutPath "$typeLibFolder\CAPE-OPEN"
  File "TLB\CAPE-OPENv1-0-0.tlb"
  File "TLB\CAPE-OPENv1-1-0.tlb"
  SetOutPath "$INSTDIR"
  File "Utils\RegTypeLib.exe"
  StrCpy $arg1 ""
  StrCmp $MultiUser.InstallMode "AllUsers" +2 0
  StrCpy $arg1 "/c"
  ClearErrors
  ExecWait '"$INSTDIR\RegTypeLib.exe" $arg1 "$typeLibFolder\CAPE-OPEN\CAPE-OPENv1-0-0.tlb"'
  IfErrors 0 +2
  DetailPrint "Registration of $typeLibFolder\CAPE-OPEN\CAPE-OPENv1-0-0.tlb failed..."
  ClearErrors
  ExecWait '"$INSTDIR\RegTypeLib.exe" $arg1 "$typeLibFolder\CAPE-OPEN\CAPE-OPENv1-1-0.tlb"'
  IfErrors 0 +2
  DetailPrint "Registration of $typeLibFolder\CAPE-OPEN\CAPE-OPENv1-1-0.tlb failed..."
  ClearErrors
  Delete "$INSTDIR\RegTypeLib.exe"
  ${If} ${RunningX64}
   SetOutPath "$INSTDIR"
   File "Utils\RegTypeLib64.exe"
   ClearErrors
   ExecWait '"$INSTDIR\RegTypeLib64.exe" $arg1 "$typeLibFolder\CAPE-OPEN\CAPE-OPENv1-0-0.tlb"'
   IfErrors 0 +2
   DetailPrint "Registration (x64) of $typeLibFolder\CAPE-OPEN\CAPE-OPENv1-0-0.tlb failed..."
   ClearErrors
   ExecWait '"$INSTDIR\RegTypeLib64.exe" $arg1 "$typeLibFolder\CAPE-OPEN\CAPE-OPENv1-1-0.tlb"'
   IfErrors 0 +2
   DetailPrint "Registration (x64) of $typeLibFolder\CAPE-OPEN\CAPE-OPENv1-1-0.tlb failed..."
   ClearErrors
   Delete "$INSTDIR\RegTypeLib64.exe"
  ${Endif}

  ;FlowExchange registration
  StrCpy $arg1 "/S"
  StrCmp $MultiUser.InstallMode "AllUsers" 0 +2
  StrCpy $arg1 "/S /A"
  ClearErrors
  ExecWait '"$INSTDIR\RegisterFlowExchange_x86.exe" $arg1'
  IfErrors 0 +2
  DetailPrint "Registration (x86) failed..."
  ${If} ${RunningX64}
   ClearErrors  
   ExecWait '"$INSTDIR\RegisterFlowExchange_x64.exe" $arg1'
   IfErrors 0 +2
   DetailPrint "Registration (x64) failed..."
  ${Endif}

SectionEnd

Section "Source code" secSource
  SetOverwrite on
  StrCpy $SOURCEINSTALLED "true"
  SetOutPath "$INSTDIR\Source Code"
  File "..\FlowExchange.sln"

  SetOutPath "$INSTDIR\Source Code\Config File"
  File "..\Config File\properties.conf"

  SetOutPath "$INSTDIR\Source Code\Help"
  File "..\Help\FlowExchange.chm"

  SetOutPath "$INSTDIR\Source Code\FlowExchange"
  File "..\FlowExchange\*.vb"
  File "..\FlowExchange\CAPEOPEN110.dll"
  File "..\FlowExchange\*.resx"
  File "..\FlowExchange\FlowExchange.vbproj"
  SetOutPath "$INSTDIR\Source Code\FlowExchange\Resources"
  File "..\FlowExchange\Resources\*"
  SetOutPath "$INSTDIR\Source Code\FlowExchange\My Project"
  File "..\FlowExchange\My Project\*"

  SetOutPath "$INSTDIR\Source Code\RegisterFlowExchange"
  File "..\RegisterFlowExchange\*.vb"
  File "..\RegisterFlowExchange\*.vbproj"
  File "..\RegisterFlowExchange\*.ico"
  SetOutPath "$INSTDIR\Source Code\RegisterFlowExchange\My Project"
  File "..\RegisterFlowExchange\My Project\*"

  SetOutPath "$INSTDIR\Source Code\XFlowViewer"
  File "..\XFlowViewer\*.config"
  File "..\XFlowViewer\*.vb"
  File "..\XFlowViewer\*.vbproj"
  File "..\XFlowViewer\*.resx"
  SetOutPath "$INSTDIR\Source Code\XFlowViewer\Resources"
  File "..\XFlowViewer\Resources\*"
  SetOutPath "$INSTDIR\Source Code\XFlowViewer\My Project"
  File "..\XFlowViewer\My Project\*"

SectionEnd

Function .onSelChange
  # keep section 'secFlowExchange' selected
  SectionGetFlags ${secFlowExchange} $0
  IntOp $0 $0 | ${SF_SELECTED}
  SectionSetFlags ${secFlowExchange} $0
FunctionEnd

Function .onInit
 ;prevent multiple instances
 System::Call 'kernel32::CreateMutexA(i 0, i 0, t "FlowExchangeInstallMutex") i .r1 ?e'
 Pop $R0
 StrCmp $R0 0 +3
 MessageBox MB_OK|MB_ICONEXCLAMATION "The FlowExchange installer is already running."
 Abort
 ;init multi user stuff
 !insertmacro MULTIUSER_INIT
 ;check loaded modules
 SetOutPath "$TEMP"
 StrCpy $IGNOREMODULES "0"
 File "Utils\CheckModuleInUse.exe"
 !insertMacro CheckModule "$TEMP\CheckModuleInUse.exe" 32 FlowExchange.dll
 !insertMacro CheckModule "$TEMP\CheckModuleInUse.exe" 32 RegisterFlowExchange_x86.exe
 !insertMacro CheckModule "$TEMP\CheckModuleInUse.exe" 32 XFlowViewer.exe
 !insertMacro CheckModule "$TEMP\CheckModuleInUse.exe" 32 CAPEOPEN110.dll
 ${If} ${RunningX64}
   File "Utils\CheckModuleInUse64.exe"
   !insertMacro CheckModule "$TEMP\CheckModuleInUse64.exe" 64 RegisterFlowExchange_x64.exe
   !insertMacro CheckModule "$TEMP\CheckModuleInUse64.exe" 64 FlowExchange.dll
   !insertMacro CheckModule "$TEMP\CheckModuleInUse64.exe" 64 XFlowViewer.exe
   !insertMacro CheckModule "$TEMP\CheckModuleInUse64.exe" 64 CAPEOPEN110.dll
   Delete "$TEMP\CheckModuleInUse64.exe"
 ${Endif}
 Delete "$TEMP\CheckModuleInUse.exe"

 ;FlowExchange section is read-only
 IntOp $0 ${SF_SELECTED} | ${SF_RO}
 SectionSetFlags ${secFlowExchange} $0
 SectionSetFlags ${secSource} 0
 Call IsDotNETInstalled
 Pop $0
 StrCmp $0 1 DoneNetTest 0
 MessageBox MB_OK|MB_ICONEXCLAMATION "The .NET Framework is not installed; this is a prerequisite for the FlowExchange unit operation to work."
 Abort
DoneNetTest:

FunctionEnd

Function un.onInit 
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

Section "-hidden section"
  ;start menu entries
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$startMenuFolder"
  CreateShortCut "$SMPROGRAMS\$startMenuFolder\FlowExchange Documentation.lnk" "$INSTDIR\FlowExchange.chm"
  CreateShortCut "$SMPROGRAMS\$startMenuFolder\FlowExchange License.lnk" "$INSTDIR\License.pdf"
  StrCmp $SOURCEINSTALLED "false" +2 0
  CreateShortCut "$SMPROGRAMS\$startMenuFolder\Source code.lnk" "$INSTDIR\Source Code"
  !insertmacro MUI_STARTMENU_WRITE_END

  ;Store installation folder
  WriteRegStr SHELL_CONTEXT "${progKey}" "" $INSTDIR
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr SHELL_CONTEXT ${instKey} "DisplayName" "FlowExchange"
  WriteRegStr SHELL_CONTEXT ${instKey} "DisplayIcon" "$INSTDIR\Uninstall.exe,0"
  WriteRegStr SHELL_CONTEXT ${instKey} "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr SHELL_CONTEXT ${instKey} "InstallLocation" "$INSTDIR"
  WriteRegStr SHELL_CONTEXT ${instKey} "Publisher" "AmsterCHEM"
  WriteRegStr SHELL_CONTEXT ${instKey} "URLInfoAbout" "http://www.amsterchem.com/"

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_FLOWEXCHANGE ${LANG_ENGLISH} "Installs the FlowExchange unit operations, XFlow viewer, online help and CAPE-OPEN type libraries"
  LangString DESC_SOURCE ${LANG_ENGLISH} "Installs the source code"

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${secFlowExchange} $(DESC_FLOWEXCHANGE)
    !insertmacro MUI_DESCRIPTION_TEXT ${secSource} $(DESC_SOURCE)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;FlowExchange unregistration
  StrCpy $arg1 "/U /S"
  StrCmp $MultiUser.InstallMode "AllUsers" 0 +2
  StrCpy $arg1 "/U /S /A"
  ClearErrors
  ExecWait '"$INSTDIR\RegisterFlowExchange_x86.exe" $arg1'
  IfErrors 0 +2
  DetailPrint "Unregistration (x86) failed..."
  ${If} ${RunningX64}
   ClearErrors  
   ExecWait '"$INSTDIR\RegisterFlowExchange_x64.exe" $arg1'
   IfErrors 0 +2
   DetailPrint "Unregistration (x64) failed..."
  ${Endif}

  Delete "$INSTDIR\license.pdf"
  Delete "$INSTDIR\FlowExchange.chm"
  Delete "$INSTDIR\FlowExchange.dll"
  Delete "$INSTDIR\CAPEOPEN110.dll"
  Delete "$INSTDIR\properties.conf"
  Delete "$INSTDIR\RegisterFlowExchange_x64.exe"
  Delete "$INSTDIR\RegisterFlowExchange_x86.exe"
  Delete "$INSTDIR\XFlowViewer.exe"

  ;remove start menu
  !insertmacro MUI_STARTMENU_GETFOLDER Application $startMenuFolder

  Delete "$SMPROGRAMS\$startMenuFolder\FlowExchange Documentation.lnk"
  Delete "$SMPROGRAMS\$startMenuFolder\FlowExchange License.lnk"
  Delete "$SMPROGRAMS\$startMenuFolder\Source code.lnk"
  RMDir "$SMPROGRAMS\$startMenuFolder"
  ;remove uninstaller
  Delete "$INSTDIR\Uninstall.exe"

  ;delete folders
  RMDir /r "$INSTDIR\Source Code"
  RMDir "$INSTDIR"

  ;remove installer key
  DeleteRegKey /ifempty SHELL_CONTEXT "${progKey}"
  ; remove uninstall from Add/Remove programs
  DeleteRegKey SHELL_CONTEXT ${instKey}
SectionEnd



