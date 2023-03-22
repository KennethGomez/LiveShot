#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use crate::screenshot::ScreenshotCapturedPayload;

mod bitmap;
mod screenshot;

#[tauri::command]
fn capture_screenshots() -> ScreenshotCapturedPayload {
    let screens = screenshot::capture_all();

    println!("Found and captured {} monitor(s)", screens.len());

    screenshot::get_payload(screens)
}

fn main() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![capture_screenshots])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
