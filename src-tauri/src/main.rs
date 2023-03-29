#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use crate::screenshot::ScreenshotCapturedPayload;
use crate::tray::{get_liveshot_system_tray, handle_system_tray_event};

mod bitmap;
mod screenshot;
mod tray;
mod capture;

#[tauri::command]
fn get_screenshots() -> ScreenshotCapturedPayload {
    capture_screenshots()
}

fn main() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![get_screenshots])
        .system_tray(get_liveshot_system_tray())
        .on_system_tray_event(handle_system_tray_event)
        .build(tauri::generate_context!())
        .expect("error while running tauri application")
        .run(|_app_handle, event| match event {
            tauri::RunEvent::ExitRequested { api, .. } => {
                api.prevent_exit();
            }
            _ => {}
        });
}

fn capture_screenshots() -> ScreenshotCapturedPayload {
    println!("Capture screenshot requested!");

    let screens = screenshot::capture_all();

    println!("Found and captured {} monitor(s)", screens.len());

    screenshot::get_payload(screens)
}
