!include "MUI.nsh"
!include "MUI2.nsh"
!include "nsDialogs.nsh"
!include "LogicLib.nsh"

; For environment variable code
!include "WinMessages.nsh"


!define MUI_ICON "logo.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "logo.png"
!define MUI_HEADERIMAGE_RIGHT


!define env_hklm 'HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"'

Name "EMV Logger"
!define VERSION ${VERSNO}
OutFile "EMVLogger-setup-${VERSION}.exe"

InstallDir "C:\EMVLogger"

;--------------------------------
; Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH


;--------------------------------
; Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
; Languages

!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Installer sections
Section "Files" SecInstall
	SectionIn RO
	SetOutPath "$INSTDIR"
	File /r ".\pack\*"
	
	CreateDirectory "$SMPROGRAMS\emvkernel"
	CreateShortCut "$SMPROGRAMS\emvkernel\EMV Logger.lnk" "$INSTDIR\EMVLogger.exe" \
	  "some command line parameters" "$INSTDIR\EMVLogger.exe" 2 SW_SHOWNORMAL \
	  ALT|CONTROL|SHIFT|F5 "a description"
	  
	CreateShortcut "$DESKTOP\EMV Logger.lnk" "$INSTDIR\EMVLogger.exe"

	WriteUninstaller "$INSTDIR\Uninstall.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "DisplayName" "EMV Logger"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "QuietUninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "HelpLink" "https://github.com/thearistotlemethod"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "URLInfoAbout" "https://github.com/thearistotlemethod"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "DisplayVersion" "${VERSION}"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "NoModify" "1"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\emvkernel" "NoRepair" "1"

	WriteRegExpandStr ${env_hklm} ASE_DIR $INSTDIR
	SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
SectionEnd

Section "Uninstall"
	RMDir /R "$INSTDIR"
	RMDir /R "$SMPROGRAMS\EMVLogger"
	Delete "$DESKTOP\EMV Logger.lnk"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EMVLogger"

	DeleteRegValue ${env_hklm} ASE_DIR
	SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
SectionEnd

LangString DESC_SecInstall ${LANG_ENGLISH} "The main installation."
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${SecInstall} $(DESC_SecInstall)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

