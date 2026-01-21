#include "stdafx.h"
#include "App.h"
#include "PreviewWnd.h"
#include "StringTools.h"

extern "C" __declspec(dllexport) void GetDuilibXmlDefinitionSize(const WCHAR* xmlPath, int* pWidth, int* pHeight)
{
    if (pWidth) *pWidth = 0;
    if (pHeight) *pHeight = 0;

    CDuiString filepath;
    filepath = xmlPath;
    filepath.Replace(_T("/"), _T("\\"));

    if (filepath.IsEmpty()) return;
    int n = filepath.ReverseFind('\\') + 1;

    CPreviewWnd* pPreviewForm = new CPreviewWnd();
    if (pPreviewForm == NULL) return;

    pPreviewForm->SetSkinFile(filepath.Right(filepath.GetLength() - n).GetData());
    pPreviewForm->SetSkinFolder(filepath.Left(n).GetData());

    // Create invisible popup window to parse XML
    if (!pPreviewForm->Create(NULL, _T("DuilibSizeFetcher"), WS_POPUP, 0, 0, 0, 0, 0))
    {
        delete pPreviewForm;
        return;
    }

    SIZE sz = pPreviewForm->GetInitSize();
    if (pWidth) *pWidth = sz.cx;
    if (pHeight) *pHeight = sz.cy;

    pPreviewForm->Close();
}

extern "C" __declspec(dllexport) HWND CreateDuilibPreview(HWND hParent, const WCHAR* xmlPath)
{
    CDuiString filepath;
    filepath = xmlPath;
    filepath.Replace(_T("/"), _T("\\"));

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
    
    // Calculate initial size based on parent
    RECT rcParent = {0};
    ::GetClientRect(hParent, &rcParent);
    int width = rcParent.right - rcParent.left;
    int height = rcParent.bottom - rcParent.top;
    if (width <= 0) width = 800;
    if (height <= 0) height = 600;

    // Create the window
    if (!pPreviewForm->Create(hParent, _T("DuilibPreviewWrapper"), dwStyle, 0, 0, 0, width, height))
    {
        delete pPreviewForm;
        return NULL;
    }
    ::ShowWindow(pPreviewForm->GetHWND(), SW_SHOW);
    ::UpdateWindow(pPreviewForm->GetHWND());

    // Resize to fit XML size with DPI scaling
    SIZE szInit = pPreviewForm->GetInitSize();
    
    // Debug DPI info
    /*
    CDuiString strMsg;
    strMsg.Format(_T("InitSize: %d, %d"), szInit.cx, szInit.cy);
    ::MessageBox(NULL, strMsg.GetData(), _T("Debug Size"), MB_OK);
    */

    if (szInit.cx > 0 && szInit.cy > 0)
    {
        HDC hDC = ::GetDC(pPreviewForm->GetHWND());
        int dpiX = ::GetDeviceCaps(hDC, LOGPIXELSX);
        ::ReleaseDC(pPreviewForm->GetHWND(), hDC);

        float scale = (float)dpiX / 96.0f;
        int scaledW = (int)(szInit.cx * scale);
        int scaledH = (int)(szInit.cy * scale);

        ::SetWindowPos(pPreviewForm->GetHWND(), NULL, 0, 0, scaledW, scaledH, SWP_NOMOVE | SWP_NOZORDER);
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
