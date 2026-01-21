#include "stdafx.h"
#include "App.h"
#include "PreviewWnd.h"
#include "StringTools.h"

extern "C" __declspec(dllexport) HWND CreateDuilibPreview(HWND hParent, const WCHAR* xmlPath)
{
    CDuiString filepath;
    filepath = xmlPath;

	if(filepath.IsEmpty()) return NULL;
	int n = filepath.ReverseFind('\\')+1;
	
    CPreviewWnd* pPreviewForm = new CPreviewWnd();
    if( pPreviewForm == NULL ) return NULL; 

    // Use absolute path for skin folder
    pPreviewForm->SetSkinFile(filepath.Right(filepath.GetLength() - n).GetData());
    pPreviewForm->SetSkinFolder(filepath.Left(n).GetData());

    // Do not change Current Directory as it affects the whole process
    // SetCurrentDirectory(pPreviewForm->GetSkinFolder().GetData());

    // Create as Child Window
    // WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN
    DWORD dwStyle = WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN;
    
    // Create the window
    if (!pPreviewForm->Create(hParent, _T("DuilibPreviewWrapper"), dwStyle, 0))
    {
        delete pPreviewForm;
        return NULL;
    }
    
    // No need to Center or Run Loop
    return pPreviewForm->GetHWND();
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CPaintManagerUI::SetInstance(hModule);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
