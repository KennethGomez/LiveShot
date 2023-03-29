use std::ffi::c_void;

use tauri::Window;
use windows::Win32::{
    Foundation::{BOOL, HWND},
    Graphics::Dwm::{DwmSetWindowAttribute, DWMWA_TRANSITIONS_FORCEDISABLED},
};

#[cfg(target_os = "windows")]
pub unsafe fn disable_window_transitions(window: Window) {
    if let Ok(hwnd) = window.hwnd() {
        let _ = DwmSetWindowAttribute(
            HWND(hwnd.0),
            DWMWA_TRANSITIONS_FORCEDISABLED,
            &mut BOOL::from(true) as *mut _ as *mut c_void,
            std::mem::size_of::<BOOL>() as u32,
        );
    }
}
